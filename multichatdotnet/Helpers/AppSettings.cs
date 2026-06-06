/**
 * AppSettings.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-04
 */
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace multichatdotnet.Helpers
{
    internal class AppSettings
    {
        private static string _filePath = "settings.json";

        internal bool StreamByDefault { get; set; } = true;
        internal bool RenderMarkdown { get; set; } = true;
        internal string DefaultModelId { get; set; } = ""; // llama-3.3-70b-versatile
        internal string DefaultProviderId { get; set; } = ""; // groq
        internal int MaxHistoryMessages { get; set; } = 100; // context window management
        internal List<ProviderInfo> Providers { get; set; } = new List<ProviderInfo>();

        public static AppSettings Load()
        {
            if (!File.Exists(_filePath))
                return new AppSettings();

            string content = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<AppSettings>(content);
        }

        public static void Save(AppSettings settings)
        {
            //TODO: Add validate section or method
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
