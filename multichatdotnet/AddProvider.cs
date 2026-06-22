/*
 * @brief Add Provider Form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-20
 */
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace multichatdotnet
{
    public partial class AddProvider : Form
    {
        MainForm mainForm;

        public AddProvider()
        {
            InitializeComponent();
        }

        private void AddProvider_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Owner;
            cboProvider.ValueMember = "Key";
            cboProvider.DisplayMember = "Value";
            for (int i = 0; i < Program.DefaultProviders.Count; i++) {
                ProviderInfo pi = Program.DefaultProviders[i];
                ProviderInfo found= Program.Settings.Providers.FirstOrDefault(
                    p => p.Id == pi.Id);
                if (found == null) {
                    cboProvider.Items.Add(
                        new KeyValuePair<string, string> ( pi.Id, pi.DisplayName )
                        );
                }
            }
            if (cboProvider.Items.Count > 0) cboProvider.SelectedIndex = 0;
        }

        private async void btnAddProvider_Click(object sender, EventArgs e)
        {
            //cboProvider.SelectedValue; // should return 'groq'
            if (cboProvider.SelectedItem is KeyValuePair<string, string> kvp) {
                switch (kvp.Key) {
                    case "gemini":
                        break;
                    default:
                        MessageBox.Show("Not implemented.");
                        return;
                }
                var apikey = Microsoft.VisualBasic.Interaction.InputBox("Enter API Key:");
                if (string.IsNullOrEmpty(apikey))
                    return;
                
                ProviderInfo template= Program.DefaultProviders.FirstOrDefault(p => p.Id == kvp.Key); //groq
                                                                                                      // Clone — don't mutate the shared template
                ProviderInfo pi = new ProviderInfo
                {
                    Id = template.Id,
                    DisplayName = template.DisplayName,
                    BaseUrl = template.BaseUrl,
                    RegistrationUrl = template.RegistrationUrl,
                    ApiKey = apikey,
                };
                btnAddProvider.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    var models = await FetchModelsAsync(pi);
                    if (models == null || models.Count == 0)
                    {
                        MessageBox.Show("Couldn't validate this API key — check it and try again.",
                            "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    pi.AvailableModels = models;
                    Program.Settings.Providers.Add(pi);
                    Program.Settings.Save(); // persist changes to disk.
                }
                finally
                {
                    btnAddProvider.Enabled = true;
                    this.Cursor = Cursors.Default;
                }
            }
        }

        // The validation call. A 200 here with parseable model data is what proves the key works —
        // there's no separate "is this key valid" endpoint on any of these providers.
        private async Task<List<ModelInfo>> FetchModelsAsync(ProviderInfo pi)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", pi.ApiKey);

                foreach (var header in pi.ExtraHeaders ?? new Dictionary<string, string>())
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

                string url = pi.BaseUrl.TrimEnd('/') + "/models";

                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync(url);
                }
                catch (Exception) // covers timeout (TaskCanceledException) and network failure (HttpRequestException)
                {
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                    return null; // 401/403 = bad key; other codes = provider issue, treat the same for now

                string json = await response.Content.ReadAsStringAsync();
                return ParseModelsResponse(json, pi.Id);
            }
        }

        private List<ModelInfo> ParseModelsResponse(string json, string providerId)
        {
            var result = new List<ModelInfo>();
            var root = JObject.Parse(json);
            var data = root["data"] as JArray; // OpenAI-compatible /models wraps the list under "data"
            if (data == null) return result;

            foreach (var item in data)
            {
                string id = item["id"]?.ToString();
                if (string.IsNullOrEmpty(id)) continue;

                result.Add(new ModelInfo
                {
                    Id = id,
                    DisplayName = id, // raw id for now — prettify display names later if you want
                    ProviderId = providerId,
                    MaxContextTokens = item["context_window"]?.ToObject<int>() ?? 0,
                });
            }
            return result;
        }

    }
}
