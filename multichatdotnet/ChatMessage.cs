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


    internal class Provider {
        internal ProviderEnum Name { get; set; }
        internal string DisplayName { get; set; }   // e.g., "Hugging Face" for clean UI strings
        internal string ApiKey { get; set; } = "";
        internal string BaseUrl { get; set; } = "";
        internal string RegistrationUrl { get; set; } = "";

        public override string ToString() => DisplayName;
    }
}
