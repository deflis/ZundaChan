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

        /// <summary>
        /// VOICEVOXの音声を再生するデバイス番号
        /// </summary>
        public static int DeviceNumber
        {
            get
            {
                return (int)(long)Instance.configFile["device"];
            }
        }

        /// <summary>
        /// VOICEVOXのスピーカーID
        /// </summary>
        public static int SpeakerId
        {
            get
            {
                return (int)(long)Instance.configFile["speaker"];
            }
        }

        /// <summary>
        /// VOICEVOX ENGINEのURL
        /// </summary>
        public static string BaseUrl
        {
            get
            {
                return (string)Instance.configFile["voicevox_engine"];
            }
        }

        /// <summary>
        /// 設定を再読込する
        /// </summary>
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
