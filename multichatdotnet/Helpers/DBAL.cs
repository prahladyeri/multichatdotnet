/*
 * @brief SQLite Database Access Layer
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-27
 */
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace multichatdotnet.Helpers
{
    public class DBAL
    {
        private static string _connectionString;
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _paramPropertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static bool Init() {
            string fname = Path.Combine(Application.StartupPath, "db.sqlite3");
            _connectionString = $"Data Source={fname};Version=3;Journal Mode=WAL;Cache=Shared;";
            bool isnew = (!File.Exists(fname));
            if (isnew)
            {
                string sql = FileHelper.ReadEmbeddedResource(typeof(Program).Namespace + ".init.sql");
                DBAL.Execute(sql);
            }
            if (DateTime.Now.Date.Day == 1)
            {
                DBAL.Execute("vacuum");
            }
            return isnew;
        }

        public static int Execute(string sql, object parameters = null)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AddParameters(cmd, parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable Fetch(string sql, object parameters = null)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AddParameters(cmd, parameters);
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        return dt;
                    }
                }
            }
        }

        public static object FetchScalar(string sql, object parameters = null)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AddParameters(cmd, parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        private static void AddParameters(SQLiteCommand cmd, object parameters)
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

    }
}

/* Example:
 var activeUsers = db.Fetch(
    "SELECT * FROM Users WHERE IsActive = @IsActive AND Age > @MinAge", 
    new { IsActive = true, MinAge = 18 }
);
 */