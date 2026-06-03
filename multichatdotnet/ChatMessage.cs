/*
 * @brief ChatMessage and other class definitions
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-03
 */

using System;
using System.Collections.Generic;

namespace multichatdotnet
{
    internal enum ProviderEnum { Groq, HuggingFace, OpenRouter, Gemini }

    internal class ChatMessage
    {
        internal string Id { get; set; } = Guid.NewGuid().ToString();
        internal string Role { get; set; }      // "user" or "assistant"
        internal string Content { get; set; }
        internal string ModelUsed { get; set; }  // e.g., "groq/llama-3.3-70b-versatile"
        internal DateTime Timestamp { get; set; } = DateTime.Now;
    }

    internal class ChatThread 
    {
        internal string Id { get; set; } = Guid.NewGuid().ToString();
        internal string Title { get; set; } = "New Conversation";
        internal bool IsPinned { get; set; } = false;

        // Feature: Global context. Let the user define a custom system prompt per thread
        internal string SystemPrompt { get; set; } = "You are a helpful assistant.";

        internal string Model { get; set; } = ""; // e.g., "groq/llama-3.3-70b-versatile"
        internal bool Stream { get; set; } = false;

        // Track timestamps for proper chronological sorting in your sidebar list
        internal DateTime LastModified { get; set; } = DateTime.Now;
        internal DateTime CreatedAt { get; set; } = DateTime.Now;

        internal List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    internal class ModelInfo
    {
        internal string Id { get; set; } // The exact API string identifier (e.g., "llama-3.3-70b-versatile")
        internal string DisplayName { get; set; } // Clean UI string (e.g., "Llama 3.3 70B Versatile")
        internal ProviderEnum Provider { get; set; } // Back-reference link to its parent provider platform

        // Static limits (used as a defensive baseline/fallback tracker before calling the network)
        internal int MaxContextTokens { get; set; } // Max context window size (e.g., 128000)
        internal int MaxTokensPerMinute { get; set; } // Free tier TPM threshold (e.g., 12000 for Groq 70b)
        internal int MaxRequestsPerMinute { get; set; } // RPM Cap

        // Is it completely free or paid?
        internal bool IsFreeTier { get; set; } = false;
        internal string CostPerMillionInput { get; set; } = "$0.00";

        // Dynamic State Metrics (Updated continuously by your response header handler)
        internal long RemainingTokensThisMinute { get; set; } = -1;
        internal DateTime TimeWhenTokenBucketResets { get; set; } = DateTime.MinValue;

        public override string ToString() => DisplayName;
    }


    internal class Provider {
        internal ProviderEnum Name { get; set; }
        internal string DisplayName { get; set; }   // e.g., "Hugging Face" for clean UI strings
        internal string ApiKey { get; set; } = "";
        internal string BaseUrl { get; set; } = "";
        internal string RegistrationUrl { get; set; } = "";
        // Handy lookup helper pointing to all models tied specifically to this key profile
        internal List<ModelInfo> AvailableModels { get; set; } = new List<ModelInfo>();

        public override string ToString() => DisplayName;
    }
}
