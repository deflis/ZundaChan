using ZundaChan.Voicevox;
using NAudio.Wave;
using System.Collections.Concurrent;

namespace ZundaChan
{
    /// <summary>
    /// VOICEVOX ENGINE APIと棒読みちゃんAPIの間を取り持つ
    /// </summary>
    internal class Proxy
    {
        private BlockingCollection<Stream> playTalkJobs = new BlockingCollection<Stream>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Client client;

        /// <param name="client">VOICEVOXクライアント</param>
        public Proxy(Client client)
        {

            this.client = client;
            var thread = new Thread(new ThreadStart(OnStart));
            thread.IsBackground = true;
            thread.Start();

        }

        /// <summary>
        /// 読み上げ音声を登録する
        /// </summary>
        /// <param name="sTalkText">喋らせたい文章</param>
        /// <returns>読み上げタスクID。</returns>
        public int AddTalkTask(string text)
        {
            TalkTask(text);
            return 0;
        }

        private async void TalkTask(string text)
        {
            playTalkJobs.Add(await CreateVoiceAsync(text));
        }

        private async void OnStart()
        {
            foreach (var stream in playTalkJobs.GetConsumingEnumerable(CancellationToken.None))
            {
                await PlayVoiceAsync(stream);
            }
        }

        private async Task<Stream> CreateVoiceAsync(string text)
        {
            return await client.CreateAsync(text, Config.SpeakerId);
        }

        private async Task PlayVoiceAsync(Stream stream)
        {
            using (stream)
            using (var outputDevice = new WaveOutEvent() { DeviceNumber = Config.DeviceNumber })
            {
                var tcs = new TaskCompletionSource<string>();
                EventHandler<StoppedEventArgs>? h = null;
                h = (_, _) =>
                {
                    outputDevice.PlaybackStopped -= h;
                    tcs.SetResult("");
                };
                outputDevice.PlaybackStopped += h;
                Logger.Info($"Play Voice! JobCount:{playTalkJobs.Count}");
                outputDevice.Init(new WaveFileReader(stream));
                outputDevice.Play();
                await tcs.Task;
            }
        }
    }
}
