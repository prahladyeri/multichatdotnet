using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Reflection;

namespace multichatdotnet.Helpers
{
    public class DBAL
    {
        public static string ConnectionString { get; set; }

        // Caches for compiled delegates and metadata
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _paramPropertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, object> _cachedMappers = new ConcurrentDictionary<Type, object>();

        public int Execute(string sql, object parameters = null)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AddParameters(cmd, parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public List<T> Query<T>(string sql, object parameters = null)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AddParameters(cmd, parameters);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return MapReaderToList<T>(reader);
                    }
                }
            }
        }

        public T QueryScalar<T>(string sql, object parameters = null)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AddParameters(cmd, parameters);
                    var result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        return default(T);

                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
        }

        private void AddParameters(SQLiteCommand cmd, object parameters)
        {
            if (parameters == null) return;

            var type = parameters.GetType();
            // Cache the reflection lookups for parameter properties
            var properties = _paramPropertiesCache.GetOrAdd(type, t => t.GetProperties());

            foreach (var prop in properties)
            {
                var value = prop.GetValue(parameters, null);
                var param = cmd.CreateParameter();
                param.ParameterName = "@" + prop.Name;
                param.Value = value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
        }

        private List<T> MapReaderToList<T>(IDataReader reader)
        {
            var list = new List<T>();
            var type = typeof(T);

            if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
                type == typeof(DateTime) || type == typeof(Guid))
            {
                while (reader.Read())
                {
                    var val = reader.GetValue(0);
                    list.Add(val == DBNull.Value ? default(T) : (T)Convert.ChangeType(val, type));
                }
                return list;
            }

            // Get or create a compiled, high-performance row mapper for this type
            var mapper = (Action<IDataReader, Dictionary<string, int>, T>)_cachedMappers.GetOrAdd(type, t => CreateMapperDelegate<T>());

            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns[reader.GetName(i)] = i;
            }

            while (reader.Read())
            {
                var obj = Activator.CreateInstance<T>();
                mapper(reader, columns, obj);
                list.Add(obj);
            }

            return list;
        }

        // Generates an explicit block of assignments compiled straight to IL at runtime
        private Action<IDataReader, Dictionary<string, int>, T> CreateMapperDelegate<T>()
        {
            var readerParam = Expression.Parameter(typeof(IDataReader), "reader");
            var columnsParam = Expression.Parameter(typeof(Dictionary<string, int>), "columns");
            var instanceParam = Expression.Parameter(typeof(T), "instance");

            var expressions = new List<Expression>();
            var ordinalVar = Expression.Variable(typeof(int), "ordinal");

            var tryGetValueMethod = typeof(Dictionary<string, int>).GetMethod("TryGetValue");
            var getValueMethod = typeof(IDataReader).GetMethod("GetValue", new[] { typeof(int) });
            var changeTypeMethod = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });

            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanWrite) continue;

                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                // Match layout: if (columns.TryGetValue(propName, out ordinal)) { ... }
                var condition = Expression.Call(columnsParam, tryGetValueMethod, Expression.Constant(prop.Name), ordinalVar);

                // var rawValue = reader.GetValue(ordinal);
                var rawValueExpr = Expression.Call(readerParam, getValueMethod, ordinalVar);

                // Convert.ChangeType(rawValue, targetType)
                var convertedValueExpr = Expression.Convert(
                    Expression.Call(changeTypeMethod, rawValueExpr, Expression.Constant(targetType)),
                    prop.PropertyType
                );

                // instance.Property = ...
                var assignment = Expression.IfThen(
                    Expression.NotEqual(rawValueExpr, Expression.Constant(DBNull.Value)),
                    Expression.Call(instanceParam, prop.GetSetMethod(), convertedValueExpr)
                );

                expressions.Add(Expression.IfThen(condition, assignment));
            }

            var block = Expression.Block(new[] { ordinalVar }, expressions);
            return Expression.Lambda<Action<IDataReader, Dictionary<string, int>, T>>(block, readerParam, columnsParam, instanceParam).Compile();
        }
    }
}

/* Example:
 var activeUsers = db.Query<User>(
    "SELECT * FROM Users WHERE IsActive = @IsActive AND Age > @MinAge", 
    new { IsActive = true, MinAge = 18 }
);
 */