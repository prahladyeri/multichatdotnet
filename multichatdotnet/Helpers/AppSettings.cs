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
    public class AppSettings
    {
        private static string _filePath = "settings.json";

        public bool StreamByDefault { get; set; } = true;
        public bool RenderMarkdown { get; set; } = true;
        public string DefaultModelId { get; set; } = ""; // llama-3.3-70b-versatile
        public string DefaultProviderId { get; set; } = ""; // groq
        public int MaxHistoryMessages { get; set; } = 100; // context window management
        public List<ProviderInfo> Providers { get; set; } = new List<ProviderInfo>();

        public static AppSettings Load()
        {
            if (!File.Exists(_filePath))
                return new AppSettings();

            string content = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<AppSettings>(content);
        }

        public void Save()
        {
            //TODO: Add validate section or method
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
