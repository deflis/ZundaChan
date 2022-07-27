using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomlyn;
using Tomlyn.Model;

namespace ZundaChan
{
    internal class Config
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static Config instance = new Config();
        private TomlTable configFile;

        private static Config Instance
        {
            get
            {
                return instance;
            }
        }

        public static int DeviceNumber
        {
            get
            {
                return (int)(long)Instance.configFile["device"];
            }
        }

        public static int SpeakerId
        {
            get
            {
                return (int)(long)Instance.configFile["speaker"];
            }
        }

        public static string BaseUrl
        {
            get
            {
                return (string)Instance.configFile["voicevox_engine"];
            }
        }

        public static void Reload()
        {
            Instance.configFile = LoadConfig();
        }

        private Config() => configFile = LoadConfig();
        private static TomlTable LoadConfig()
        {
            string appFilePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var tomlFile = System.Text.RegularExpressions.Regex.Replace(
                appFilePath,
                "\\.exe|dll$",
                ".toml",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Logger.Info($"load config from {tomlFile}");

            string toml;
            using (var tomlStream = new FileStream(tomlFile, FileMode.Open))
            using (var tomlReader = new StreamReader(tomlStream))
            {
                toml = tomlReader.ReadToEnd();
            }
            return Toml.ToModel(toml);
        }
    }
}
