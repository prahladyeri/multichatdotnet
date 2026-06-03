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
    internal class ChatMessage
    {
        internal string Role { get; set; }      // "user" or "assistant"
        internal string Content { get; set; }
        internal string ModelUsed { get; set; }  // e.g., "groq/llama-3.3-70b-versatile"
        internal DateTime Timestamp { get; set; } = DateTime.Now;
    }

    internal class ChatThread 
    {
        internal string Model { get; set; } = ""; // e.g., "groq/llama-3.3-70b-versatile"
        internal bool Stream { get; set; } = false;
        internal List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    internal enum ProviderEnum { Groq, HuggingFace, OpenRouter, Gemini }

    internal class Provider {
        internal ProviderEnum Name { get; set; }
        internal string DisplayName { get; set; }   // e.g., "Hugging Face" for clean UI strings
        internal string ApiKey { get; set; } = "";
        internal string BaseUrl { get; set; } = "";
        internal string RegistrationUrl { get; set; } = "";

        public override string ToString() => DisplayName;
    }
}
