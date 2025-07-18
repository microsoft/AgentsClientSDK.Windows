using Microsoft.Agents.Client.Wpf;
using Microsoft.Agents.Client.Wpf.Exceptions;
using Microsoft.Agents.Client.Wpf.Models;
using Microsoft.Agents.Client.Wpf.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SampleApp.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Policy;
using System.Text.Json;
using System.Threading.Tasks;

namespace SampleApp
{
    public sealed partial class MainWindow : Window
    {

        public new static MainWindow Current { get; private set; } = null!;
        private readonly string[] _backgroundImages =
        {
            "ms-appx:///Assets/Background/hotel.jpg"
        };
        private AgentsClientSDK _clientSDK;
        private ChatView _chatView;

        public AgentsClientSDK ClientSDK => _clientSDK;

        public MainWindow()
        {
            this.InitializeComponent();
            SetRandomBackground();

            Current = this;
            this.Title = "Welcome to the TextClientApp";

            RootContent.Loaded += RootContent_Loaded;
        }
        public void ShowProgressBar()
        {
            ProgressRingPanel.Visibility = Visibility.Visible;
        }

        public void HideProgressBar()
        {
            ProgressRingPanel.Visibility = Visibility.Collapsed;
        }

        private void SetRandomBackground()
        {
            var random = new Random();
            int index = random.Next(_backgroundImages.Length);
            var image = new BitmapImage(new Uri(_backgroundImages[index]));
            BackgroundImage.ImageSource = image;
        }

        private async void RootContent_Loaded(object sender, RoutedEventArgs e)
        {

            AppConfig config = ConfigManager.LoadConfig();

            //if (config == null
            //    || string.IsNullOrWhiteSpace(config.AppSettings.AgentSchemaName)
            //    || string.IsNullOrWhiteSpace(config.AppSettings.AgentEnvironmentId))
            //{
            //var (tokenUrl, enableSpeech) = await AskUserForTokenUrlAsync();
            //bool flowControl = await PromptForAgentConfigAsync(config);
            //if (!flowControl)
            //{
            //    return;
            //}

            //}

            var configPath = Path.Combine(
             AppDomain.CurrentDomain.BaseDirectory,
             "appsettings.json"
             );

            if (!File.Exists(configPath))
                return;



            //config = new AppConfig();
            var json = File.ReadAllText(configPath);

            // Deserialize the JSON into AppConfig
            Dictionary<string, string> settings = AppSettingsReader.GetAppSettingsData(configPath);

            if (config == null)
                config = new AppConfig();

            config.AppSettings.AgentSchemaName = settings["schema"];
            config.AppSettings.AgentEnvironmentId = settings["url"];
            //config.AppSettings.AgentId = settings["environmentId"];
            config.AppSettings.IsAnonymousAgent = settings["IsAnonymousAgent"] == "0" ? false : true;

            await LoadChatViewAsync(config);
        }

        async Task<bool> PromptForAgentConfigAsync(AppConfig? config)
        {
            var (AgentSchemaName, EnvironmentName, confirmed) = await AskUserForAgentSchemaAndEnvironmentAsync();
            if (string.IsNullOrWhiteSpace(AgentSchemaName) || string.IsNullOrEmpty(EnvironmentName))
            {
                Application.Current.Exit();
                return false;
            }

            config = new AppConfig();
            config.AppSettings.AgentSchemaName = AgentSchemaName;
            config.AppSettings.AgentEnvironmentId = EnvironmentName;
            ConfigManager.SaveConfig(config);
            return true;
        }


        private async Task<(string TokenUrl, bool EnableSpeech)> AskUserForTokenUrlAsync()
        {
            var inputBox = new TextBox { Width = 300 };
            var speechSwitch = new ToggleSwitch
            {
                Header = "Enable Speech",
                IsOn = false
            };

            var dialog = new ContentDialog
            {
                Title = "Agent Configurations",
                Content = new StackPanel
                {
                    Spacing = 12,
                    Children =
                        {
                            new TextBlock { Text = "Please enter the token URL:" },
                            inputBox,
                            speechSwitch
                        }
                },
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
               return (inputBox.Text.Trim(), speechSwitch.IsOn);
            }

            return (Config.DirectLineTokenUrl, false); // Cancelled
        }

        private async Task LoadChatViewAsync(AppConfig config)
        {
            ShowProgressBar();
            try
            {
                _clientSDK = await AgentsClientSDK.CreateAsync(config.ClientSDKProperties());

                RootContent.Children.Clear();
                //User control with chat view

                AgentActivity welcomeMessage = await _clientSDK.GetWelcomeMessageAsync();

                _chatView = new ChatView(config, _clientSDK, welcomeMessage);
                RootContent.Children.Add(_chatView);
            }
            catch (SDKError ex)
            {

                // Optionally log the exception or show a dialog
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.ToLogMessage(),
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();

                // Check for 404 status code in InnerException (if it's a web exception)
                bool is404 = false;
                if (ex.InnerException is System.Net.Http.HttpRequestException httpEx)
                {
                    if (httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        is404 = true;
                    }
                }
                else if (ex.InnerException is System.Net.WebException webEx &&
                         webEx.Response is System.Net.HttpWebResponse response &&
                         response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    is404 = true;
                }

                if (is404)
                {
                    await PromptForAgentConfigAsync(config);
                }

            }
            finally
            {
                HideProgressBar();
            }
        }

        public static class AppSettingsReader
        {
            public static string ConvertToPowerPlatformUrl(string input, string environment)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return string.Empty;

                string normalized = input.ToLowerInvariant();

                if (environment.Equals("prod", StringComparison.OrdinalIgnoreCase))
                {
                    return $"{normalized}.environment.api.powerplatform.com";
                }
                else
                {
                    return $"{normalized}.environment.api.{environment}.powerplatform.com";
                }
            }

            public static string createEnvurl(string? envId, string? envName)
            {
                if (string.IsNullOrWhiteSpace(envId))
                    return string.Empty;

                string cleaned = envId.Replace("-", "");

                if (cleaned.Length <= 2)
                    return cleaned;

                if (envName == "test" || envName == "preprod")
                {
                    string prefix = cleaned.Substring(0, cleaned.Length - 1);
                    string suffix = cleaned.Substring(cleaned.Length - 1);
                    return $"{prefix}.{suffix}";
                }
                else
                {
                    string prefix = cleaned.Substring(0, cleaned.Length - 2);
                    string suffix = cleaned.Substring(cleaned.Length - 2);
                    return $"{prefix}.{suffix}";
                }
            }

            public static Dictionary<string, string> GetAppSettingsData(string jsonPath)
            {
                var json = File.ReadAllText(jsonPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                Dictionary<string, string> settings = new Dictionary<string, string>();

                // Try AppSettings path
                //if (root.TryGetProperty("AppSettings", out JsonElement appSettings))
                //{
                //    string schema = appSettings.GetProperty("AgentSchemaName").GetString();
                //    string envId = appSettings.GetProperty("AgentEnvironmentId").GetString();
                //    return (schema, envId);
                //}

                // Try user path
                
                    if (root.TryGetProperty("user", out JsonElement user))
                    {
                        string schema = user.GetProperty("schemaName").GetString();
                        string envId = user.GetProperty("environmentId").GetString();
                        string envName = user.GetProperty("environment").GetString();
                        bool isAuthEnabled = user.GetProperty("isAuthEnabled").GetBoolean();

                    string url = ConvertToPowerPlatformUrl(createEnvurl(envId, envName), envName);
                        settings["schema"] = schema;
                        settings["environmentId"] = envId;
                        settings["environment"] = envName;
                        settings["url"] = url;
                        settings["IsAnonymousAgent"] = !isAuthEnabled ? 1.ToString() : 0.ToString();
                }
                


                return settings;
            }
        }
        private async Task<(string AgentSchemaName, string EnvironmentName, bool Confirmed)> AskUserForAgentSchemaAndEnvironmentAsync()
        {
            var agentSchemaBox = new TextBox { Width = 300, PlaceholderText = "Agent Schema Name" };
            var environmentBox = new TextBox { Width = 300, PlaceholderText = "Environment Name" };

            var dialog = new ContentDialog
            {
                Title = "Agent Configuration",
                Content = new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new TextBlock { Text = "Please enter the Agent Schema Name:" },
                        agentSchemaBox,
                        new TextBlock { Text = "Please enter the Environment Name:" },
                        environmentBox
                    }
                },
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                return (agentSchemaBox.Text.Trim(), environmentBox.Text.Trim(), true);
            }

            return (string.Empty, string.Empty, false);
        }
    }
}

