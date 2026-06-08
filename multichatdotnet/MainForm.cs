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
        private List<ProviderInfo> _defaultProviders;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _defaultProviders = InitializeProviders();
            
            menuStrip1.ImageList = imageList1;
            mnuNewChat.Image = imageList1.Images["message"];
            mnuExit.Image = imageList1.Images["exit"];
            mnuAbout.Image = imageList1.Images["info"];
            mnuSettings.Image = imageList1.Images["edit"];
            mnuProviders.Image = imageList1.Images["mainframe"];
            mnuViewLog.Image = imageList1.Images["clock"];
        }

        internal List<ProviderInfo> InitializeProviders()
        {
            return new List<ProviderInfo>
            {
                new ProviderInfo {
                    Id= "groq",
                    DisplayName = "Groq",
                    BaseUrl = "https://api.groq.com/openai/v1",
                    RegistrationUrl = "https://console.groq.com/keys"
                },
                new ProviderInfo {
                    Id = "openrouter",
                    DisplayName = "OpenRouter",
                    BaseUrl = "https://openrouter.ai/api/v1",
                    RegistrationUrl = "https://openrouter.ai/keys"
                },
                new ProviderInfo {
                    Id = "huggingface",
                    DisplayName = "Hugging Face",
                    BaseUrl = "https://router.huggingface.co/v1",
                    RegistrationUrl = "https://huggingface.co/settings/tokens"
                },
                new ProviderInfo {
                    Id = "gemini",
                    DisplayName = "Google Gemini",
                    BaseUrl = "https://generativelanguage.googleapis.com/v1beta/openai/", // Gemini's native OpenAI endpoint
                    RegistrationUrl = "https://aistudio.google.com/"
                }
            };
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
