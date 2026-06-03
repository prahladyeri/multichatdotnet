/*
 * @brief Main Form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-03
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace multichatdotnet
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeProviders();
        }

        internal List<Provider> InitializeProviders()
        {
            return new List<Provider>
            {
                new Provider {
                    Name = ProviderEnum.Groq,
                    DisplayName = "Groq",
                    BaseUrl = "https://api.groq.com/openai/v1",
                    RegistrationUrl = "https://console.groq.com/keys"
                },
                new Provider {
                    Name = ProviderEnum.OpenRouter,
                    DisplayName = "OpenRouter",
                    BaseUrl = "https://openrouter.ai/api/v1",
                    RegistrationUrl = "https://openrouter.ai/keys"
                },
                new Provider {
                    Name = ProviderEnum.HuggingFace,
                    DisplayName = "Hugging Face",
                    BaseUrl = "https://router.huggingface.co/v1",
                    RegistrationUrl = "https://huggingface.co/settings/tokens"
                },
                new Provider {
                    Name = ProviderEnum.Gemini,
                    DisplayName = "Google Gemini",
                    BaseUrl = "https://generativelanguage.googleapis.com/v1beta/openai/", // Gemini's native OpenAI endpoint
                    RegistrationUrl = "https://aistudio.google.com/"
                }
            };
        }

    }
}
