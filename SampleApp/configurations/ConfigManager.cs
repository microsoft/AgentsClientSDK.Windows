using Microsoft.Agents.Client.Wpf.Config;
using Microsoft.Agents.Client.Wpf.Services.Interfaces;
using System;
using System.IO;
using System.Text.Json;


namespace SampleApp
{
    public static class Config
    {
        public static readonly string AgentId = "";//"<AGENT_ID>";
        public static readonly string AgentSchemaName = "";//"<AGENT_SCHEMA_NAME>";
        public static readonly string AgentEnvironmentId = "";//"<AGENT_ENVIRONMENT_ID>";
        public static readonly string DirectLineSecret = "";//"<DIRECTLINE_SECRET>";
        public static readonly string DirectLineTokenUrl = "";//"<TOKEN_ENDPOINT>";

        public static readonly bool EnableSpeech = false; // Set to true to enable speech features
        public static readonly bool IsAnonymousAgent = true; // Set to false for DTE communication
        public static readonly string SpeechApiKey = "";//"<SPEECH_API_KEY>";
        public static readonly string SpeechRegion = "";//"<SPEECH_REGION>";
    }

    public class AppConfig
    {
        public string? TokenUrl { get; set; }
        public bool EnableSpeech { get; set; }

        public AppSettings AppSettings { get; set; } =
                                                    new AppSettings
                                                        {
                                                            SpeechApiKey = Config.SpeechApiKey,
                                                            SpeechRegion = Config.SpeechRegion,
                                                            DirectLineTokenEndpoint =  Config.DirectLineTokenUrl,
                                                            IsSpeechEnabled = Config.EnableSpeech,
                                                            AgentId = Config.AgentId,
                                                            AgentSchemaName = Config.AgentSchemaName,
                                                            AgentEnvironmentId = Config.AgentEnvironmentId,
                                                            IsAnonymousAgent = Config.IsAnonymousAgent
                                                    };

    public IClientSDKProperties ClientSDKProperties()
        {
            return new ClientSDKProperties(AppSettings);
        }
    }

    public static class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(
         Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
         "SampleApp",
         "config.json"
     );

        public static AppConfig? LoadConfig()
        {
            if (!File.Exists(ConfigPath))
                return null;

            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<AppConfig>(json);
        }

        public static void SaveConfig(AppConfig config)
        {
            var dir = Path.GetDirectoryName(ConfigPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
    }
}
