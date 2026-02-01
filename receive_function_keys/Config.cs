using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace receive_function_keys
{
    public class ConfigRoot
    {
        [JsonPropertyName("settings")]
        public Settings Settings { get; set; } = new Settings();

        [JsonPropertyName("actions")]
        public Dictionary<string, List<ActionStep>> Actions { get; set; } = new Dictionary<string, List<ActionStep>>();
    }

    public class Settings
    {
        [JsonPropertyName("logging_enabled")]
        public bool LoggingEnabled { get; set; } = false;

        [JsonPropertyName("key_hold_time")]
        public int KeyHoldTime { get; set; } = 50;
    }

    public class ActionStep
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public static class ConfigLoader
    {
        private const string ConfigFileName = "key_config.json";

        public static ConfigRoot Load()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
            if (!File.Exists(path))
            {
                // Create a default one if possible or just return empty
                return new ConfigRoot();
            }

            try
            {
                string json = File.ReadAllText(path);
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<ConfigRoot>(json, options);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定ファイルの読み込みに失敗しました。\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new ConfigRoot();
            }
        }
    }
}
