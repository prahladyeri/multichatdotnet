/*
 * @brief Main Form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-03
 */
using multichatdotnet.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
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
            Program.DefaultProviders = InitializeProviders();

            menuStrip1.ImageList = imageList1;
            mnuNewChat.Image = imageList1.Images["message"];
            mnuExit.Image = imageList1.Images["exit"];
            mnuAbout.Image = imageList1.Images["info"];
            mnuSettings.Image = imageList1.Images["edit"];
            mnuProviders.Image = imageList1.Images["mainframe"];
            mnuViewLog.Image = imageList1.Images["clock"];

            int cnt = Convert.ToInt32( DBAL.FetchScalar("select count(*) as cnt from providers") );
            if (cnt == 0) {
                new AddProvider().ShowDialog(this);
            }
        }

        internal List<ProviderInfo> InitializeProviders()
        {
            return new List<ProviderInfo>
            {
                new ProviderInfo {
                    Id = "gemini",
                    DisplayName = "Google Gemini",
                    BaseUrl = "https://generativelanguage.googleapis.com/v1beta/openai/", // Gemini's native OpenAI endpoint
                    RegistrationUrl = "https://aistudio.google.com/"
                },
                new ProviderInfo {
                    Id = "cerebras",
                    DisplayName = "Cerebras",
                    BaseUrl = "https://api.cerebras.ai/v1",
                    RegistrationUrl = "https://cloud.cerebras.ai"
                },
                new ProviderInfo {
                    Id = "nvidia",
                    DisplayName = "NVIDIA NIM",
                    BaseUrl = "https://integrate.api.nvidia.com/v1",
                    RegistrationUrl = "https://build.nvidia.com"
                },
                new ProviderInfo {
                    Id = "openrouter",
                    DisplayName = "OpenRouter",
                    BaseUrl = "https://openrouter.ai/api/v1",
                    RegistrationUrl = "https://openrouter.ai/keys"
                },
                new ProviderInfo { 
                    Id = "github",
                    DisplayName = "Github Models",
                    BaseUrl = "https://models.github.ai/inference",
                    RegistrationUrl = "https://github.com/marketplace?type=models"
                },
                new ProviderInfo {
                    Id = "deepseek",
                    DisplayName = "DeepSeek",
                    BaseUrl = "https://api.deepseek.com/v1",
                    RegistrationUrl = "https://platform.deepseek.com/api_keys"
                },
                new ProviderInfo {
                    Id = "mistral",
                    DisplayName = "Mistral AI",
                    BaseUrl = "https://api.mistral.ai/v1",
                    RegistrationUrl = "https://console.mistral.ai"
                },

                //new ProviderInfo { // - annoyingly low free quota of just 0.1 USD per month
                //    Id = "huggingface",
                //    DisplayName = "Hugging Face",
                //    BaseUrl = "https://router.huggingface.co/v1",
                //    RegistrationUrl = "https://huggingface.co/settings/tokens"
                //},

                //new ProviderInfo { // - no free
                //    Id= "groq",
                //    DisplayName = "Groq",
                //    BaseUrl = "https://api.groq.com/openai/v1",
                //    RegistrationUrl = "https://console.groq.com/keys"
                //},


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
