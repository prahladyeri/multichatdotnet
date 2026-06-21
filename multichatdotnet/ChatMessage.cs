/*
 * @brief ChatMessage and other class definitions
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-03
 */
using multichatdotnet.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace multichatdotnet
{
    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Role { get; set; }  // "user" or "assistant" or "system"
        public bool IsSystemInstruction => Role?.Equals("system", StringComparison.OrdinalIgnoreCase) ?? false;
        public string Content { get; set; }
        public string ModelId { get; set; }  // llama-3.3-70b-versatile
        public string ProviderId { get; set; } // groq
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsError { get; set; } = false;
        public string ErrorDetails { get; set; } = "";
    }

    public class ChatThread 
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "New Conversation";
        public bool IsPinned { get; set; } = false;

        // Feature: Global context. Let the user define a custom system prompt per thread
        public string SystemPrompt { get; set; } = "You are a helpful assistant.";

        public string ModelId { get; set; } = ""; // e.g., "groq/llama-3.3-70b-versatile"

        // Track timestamps for proper chronological sorting in your sidebar list
        public DateTime LastModified { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    public class ModelInfo
    {
        public string Id { get; set; } // The exact API string identifier (e.g., "llama-3.3-70b-versatile")
        public string DisplayName { get; set; } // Clean UI string (e.g., "Llama 3.3 70B Versatile")
        public string ProviderId { get; set; } // matches Provider's BaseUrl or a slug like "groq"

        public bool IsEnabled { get; set; } = false;

        public bool SupportsToolCalling { get; set; } = false;
        public bool SupportsVision { get; set; } = false;

        // Static limits (used as a defensive baseline/fallback tracker before calling the network)
        public int MaxContextTokens { get; set; } // Max context window size (e.g., 128000)
        public int MaxTokensPerMinute { get; set; } // Free tier TPM threshold (e.g., 12000 for Groq 70b)
        public int MaxRequestsPerMinute { get; set; } // RPM Cap

        [JsonIgnore]
        public Dictionary<TaskTypeEnum, int> TaskRanking { 
            get  => TaskRankingResolver.Resolve(this.Id);
        }

        // Is it completely free or paid?
        public bool IsFreeTier { get; set; } = false;
        public decimal CostPerMillionInputTokens { get; set; } = 0.00m;
        public decimal CostPerMillionOutputTokens { get; set; } = 0.00m;

        // Dynamic State Metrics (Updated continuously by your response header handler)
        [JsonIgnore]
        public long RemainingTokensThisMinute { get; set; } = -1;
        [JsonIgnore]
        public DateTime TimeWhenTokenBucketResets { get; set; } = DateTime.MinValue;

        public override string ToString() => DisplayName;
    }


    public class ProviderInfo 
    {
        public string Id { get; set; } // e.g. "groq", "openrouter", "gemini", etc.
        public string DisplayName { get; set; }   // e.g., "Hugging Face" for clean UI strings

        public bool IsEnabled { get; set; } = true;

        [JsonProperty("_apiKey")]
        private string _apiKey;
        [JsonIgnore]
        public string ApiKey 
        {
            get => string.IsNullOrEmpty(_apiKey) ? "" : DpapiSecretVault.Decrypt(_apiKey, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            set => _apiKey = string.IsNullOrEmpty(value) ? "" : DpapiSecretVault.Encrypt(value, System.Security.Cryptography.DataProtectionScope.CurrentUser);
        }

        public string BaseUrl { get; set; } = "";
        public string RegistrationUrl { get; set; } = "";
        public Dictionary<string, string> ExtraHeaders { get; set; } = new Dictionary<string, string>();

        public List<ModelInfo> AvailableModels { get; set; } = new List<ModelInfo>();

        public override string ToString() => DisplayName;
    }
}
