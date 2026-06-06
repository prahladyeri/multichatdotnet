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
    
    internal class ChatMessage
    {
        internal string Id { get; set; } = Guid.NewGuid().ToString();
        internal string Role { get; set; }  // "user" or "assistant" or "system"
        internal bool IsSystemInstruction => Role?.Equals("system", StringComparison.OrdinalIgnoreCase) ?? false;
        internal string Content { get; set; }
        internal string ModelId { get; set; }  // llama-3.3-70b-versatile
        internal string ProviderId { get; set; } // groq
        internal DateTime Timestamp { get; set; } = DateTime.Now;

        internal bool IsError { get; set; } = false;
        internal string ErrorDetails { get; set; } = "";
    }

    internal class ChatThread 
    {
        internal string Id { get; set; } = Guid.NewGuid().ToString();
        internal string Title { get; set; } = "New Conversation";
        internal bool IsPinned { get; set; } = false;

        // Feature: Global context. Let the user define a custom system prompt per thread
        internal string SystemPrompt { get; set; } = "You are a helpful assistant.";

        internal string ModelId { get; set; } = ""; // e.g., "groq/llama-3.3-70b-versatile"

        // Track timestamps for proper chronological sorting in your sidebar list
        internal DateTime LastModified { get; set; } = DateTime.Now;
        internal DateTime CreatedAt { get; set; } = DateTime.Now;

        internal List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    internal class ModelInfo
    {
        internal string Id { get; set; } // The exact API string identifier (e.g., "llama-3.3-70b-versatile")
        internal string DisplayName { get; set; } // Clean UI string (e.g., "Llama 3.3 70B Versatile")
        internal string ProviderId { get; set; } // matches Provider's BaseUrl or a slug like "groq"

        // Static limits (used as a defensive baseline/fallback tracker before calling the network)
        internal int MaxContextTokens { get; set; } // Max context window size (e.g., 128000)
        internal int MaxTokensPerMinute { get; set; } // Free tier TPM threshold (e.g., 12000 for Groq 70b)
        internal int MaxRequestsPerMinute { get; set; } // RPM Cap

        // Is it completely free or paid?
        internal bool IsFreeTier { get; set; } = false;
        internal decimal CostPerMillionInputTokens { get; set; } = 0.00m;
        internal decimal CostPerMillionOutputTokens { get; set; } = 0.00m;

        // Dynamic State Metrics (Updated continuously by your response header handler)
        [JsonIgnore]
        internal long RemainingTokensThisMinute { get; set; } = -1;
        [JsonIgnore]
        internal DateTime TimeWhenTokenBucketResets { get; set; } = DateTime.MinValue;

        public override string ToString() => DisplayName;
    }


    internal class ProviderInfo 
    {
        internal string Id { get; set; } // e.g. "groq", "openrouter", "gemini", etc.
        internal string DisplayName { get; set; }   // e.g., "Hugging Face" for clean UI strings

        [JsonProperty("_apiKey")]
        private string _apiKey;
        [JsonIgnore]
        internal string ApiKey 
        {
            get => string.IsNullOrEmpty(_apiKey) ? "" : DpapiSecretVault.Decrypt(_apiKey, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            set => _apiKey = string.IsNullOrEmpty(value) ? "" : DpapiSecretVault.Encrypt(value, System.Security.Cryptography.DataProtectionScope.CurrentUser);
        }

        internal string BaseUrl { get; set; } = "";
        internal string RegistrationUrl { get; set; } = "";
        // Handy lookup helper pointing to all models tied specifically to this key profile
        internal List<ModelInfo> AvailableModels { get; set; } = new List<ModelInfo>();

        public override string ToString() => DisplayName;
    }
}
