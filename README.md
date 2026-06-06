# multichat.net

A fast, lightweight, and utilitarian multi-model desktop LLM chat client built natively with the legacy WinForms on .NET Framework to achieve maximum speed with minimum footprint. Designed for power users and developers who value close-to-the-metal performance, privacy, and streamlined text workflows over web-bloat.

## Core Architecture
* **Engine:** Native C# WinForms (.NET Framework 4.x) providing instantaneous startup times and minimal RAM usage.
* **Rendering:** WebView2 integration strictly used as a sandboxed, high-performance Markdown and SVG text-rendering viewport.
* **Storage:** Zero database overhead. Fast, secure, and transparent local flat-file JSON datastore (one file per thread) managed via `Newtonsoft.Json`.
* **API Layer:** Uniform OpenAI-compatible schema routing targeting Groq, OpenRouter, Hugging Face, and Google Gemini API endpoints.

---

## Project Milestones & Features Roadmap

### 🟩 Phase 1: Core Foundation & API Plumbing
- [x] Implement secure `ServicePointManager` handling forced TLS 1.2/1.3 network handshakes.
- [x] Model contracts (`ChatMessage`, `ChatThread`, `Provider`) setup and local serialization pipeline.
- [ ] Multi-provider API routing manager supporting Groq, OpenRouter, Hugging Face, and Gemini endpoints.
- [ ] Dynamic runtime model discovery (`GET /v1/models`) populating native UI dropdown configurations.
- [ ] Provider configuration dialog featuring automated click-to-register URLs and token management.

### 🟨 Phase 2: Lightweight Thread & UI Architecture
- [ ] Initialize standard WinForms application shell alongside isolated rich-text WebView2 display canvas.
- [ ] Implement non-blocking asynchronous text streaming utilizing JavaScript DOM insertion (`insertAdjacentText`).
- [ ] Implement local directory "Shallow Scanning" pattern to fetch thread headers without deserializing complete chat logs.
- [ ] Design sidebar list interface sorted chronologically using `LastModified` attributes.

### 🟦 Phase 3: Utilitarian Power Features
- [ ] **Thread Pinning:** Support toggle flag `IsPinned` to dynamically lock priority workspaces to the top of the sidebar.
- [ ] **Global Thread Search:** Rapid string-filtering across thread titles with lazy-loading fallback for message history contents.
- [ ] **Mid-Thread Model Switching:** Allow users to dynamically hot-swap models/providers mid-conversation without breaking context.
- [ ] **Model Attributions:** Metadata badges explicitly binding the underlying generating model to every discrete message bubble.
- [ ] **System Prompt Profiles:** Core manager template to pre-seed, store, and apply custom system behavior presets.
- [ ] **Conversation Forking:** Instantly clone an active `List<ChatMessage>` array into a standalone branched workspace file.
- [ ] **One-Click Scaffolder (Download):** Add a context-menu item ("Extract Project to Disk") to automatically parse `<file path="...">` blocks and write directories/files directly to a workspace folder.
- [ ] **One-Click Scaffolder (Upload):** Add an attachment utility ("Attach Code Files/Folder") next to the input box to recursively serialize chosen text assets into the outgoing `<file path="...">` context payload.

### 🟧 Phase 4: Data Portability & Extensions
- [ ] **Markdown Export Engine:** Single-click streaming compilation tool to dump full conversation logs to standard `.md` layouts.
- [ ] **Message Isolation:** Context-menu tool to isolate and export individual specific chat message arrays.
- [ ] Natively handle real-time vector image generation and layout parsing directly inside the WebView2 framework.

---

## Provider Platform Implementation Roadmap
- [ ] **Nvidia NIM**
- [ ] **Google Gemini**
- [ ] **Groq**
- [ ] **Mistral AI**
- [ ] **Cohere**
- [ ] **Open Router**

---

## Data Structure Contracts

The local datastore layout is explicitly decoupled from provider definitions, avoiding hardcoded models and optimizing for zero-allocation stream writing.

### Thread Record Schema
```json
{
  "Id": "7a8b9c1d-1234-5678-abcd-ef0123456789",
  "Title": "C# Optimization Techniques",
  "IsPinned": true,
  "SystemPrompt": "You are a concise, close-to-the-metal C# optimization assistant.",
  "Model": "groq/llama-3.3-70b-versatile",
  "Stream": true,
  "CreatedAt": "2026-06-03T09:30:00Z",
  "LastModified": "2026-06-03T09:35:00Z",
  "Messages": [
    {
      "Role": "user",
      "Content": "How can I avoid allocations during JSON file streaming?",
      "ModelUsed": null,
      "Timestamp": "2026-06-03T09:31:00Z"
    },
    {
      "Role": "assistant",
      "Content": "Utilize `JsonSerializer` directly against a `StreamReader`...",
      "ModelUsed": "groq/llama-3.3-70b-versatile",
      "Timestamp": "2026-06-03T09:31:05Z"
    }
  ]
}
```

## License

Distributed under the MIT License. See `LICENSE` for details.

Copyright (c) 2026 Prahlad Yeri [prahladyeri@yahoo.com](mailto:prahladyeri@yahoo.com)