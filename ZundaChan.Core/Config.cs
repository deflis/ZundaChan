using Tomlyn;
using Tomlyn.Model;

namespace ZundaChan.Core
{
    public class Config
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
        /// HTTPサーバの待受ポート
        /// </summary>
        public static int HttpPort
        {
            get
            {
                return (int)(long)Instance.configFile["http_port"];
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
            string appFilePath = System.Reflection.Assembly.GetEntryAssembly()!.Location;
            var tomlFile = Path.Combine(Path.GetDirectoryName(appFilePath)!, $"{Path.GetFileNameWithoutExtension(appFilePath)}.toml");
            Logger.Info($"load config from {tomlFile}");

            using var tomlStream = new FileStream(tomlFile, FileMode.Open);
            using var tomlReader = new StreamReader(tomlStream);
            return Toml.ToModel(tomlReader.ReadToEnd());

        }
    }
}
