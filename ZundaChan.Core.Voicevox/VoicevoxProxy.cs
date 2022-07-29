using NAudio.Wave;
using System.Collections.Concurrent;

namespace ZundaChan.Core.Voicevox
{
    /// <summary>
    /// VOICEVOX ENGINE APIと棒読みちゃんAPIの間を取り持つ
    /// </summary>
    public class VoicevoxProxy : IProxy
    {
        private BlockingCollection<Stream> playTalkJobs = new BlockingCollection<Stream>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly VoicevoxClient client;

        /// <param name="client">VOICEVOXクライアント</param>
        public VoicevoxProxy(VoicevoxClient client)
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
            try
            {
                playTalkJobs.Add(await CreateVoiceAsync(text));
            }
            catch (Exception ex)
            {
                Logger.Error("ボイス生成でエラーが発生しました", ex);
            }
        }

        private async void OnStart()
        {
            foreach (var stream in playTalkJobs.GetConsumingEnumerable(CancellationToken.None))
            {
                try
                {
                    await PlayVoiceAsync(stream);
                }
                catch (Exception ex)
                {
                    Logger.Error("ボイス再生でエラーが発生しました", ex);
                }
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
            return;
        }
    }
}
