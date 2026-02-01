using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;

namespace receive_function_keys
{
    [DataContract]
    public class ConfigRoot
    {
        [DataMember(Name = "settings")]
        public Settings Settings { get; set; } = new Settings();

        [DataMember(Name = "actions")]
        public Dictionary<string, List<ActionStep>> Actions { get; set; } = new Dictionary<string, List<ActionStep>>();
    }

    [DataContract]
    public class Settings
    {
        [DataMember(Name = "logging_enabled")]
        public bool LoggingEnabled { get; set; } = false;

        [DataMember(Name = "key_hold_time")]
        public int KeyHoldTime { get; set; } = 50;
    }

    [DataContract]
    public class ActionStep
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "value")]
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
                return new ConfigRoot();
            }

            try
            {
                string json = File.ReadAllText(path);
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(ConfigRoot), new DataContractJsonSerializerSettings
                    {
                        UseSimpleDictionaryFormat = true
                    });
                    return (ConfigRoot)serializer.ReadObject(ms);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定ファイルの読み込みに失敗しました。\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new ConfigRoot();
            }
        }
    }
}
