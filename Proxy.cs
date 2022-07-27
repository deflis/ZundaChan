using ZundaChan.Voicevox;
using NAudio.Wave;
using System.Collections.Concurrent;

namespace ZundaChan
{
    internal class Proxy
    {
        private BlockingCollection<Stream> playTalkJobs = new BlockingCollection<Stream>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Client client;

        public Proxy(Client client)
        {

            this.client = client;
            var thread = new Thread(new ThreadStart(OnStart));
            thread.IsBackground = true;
            thread.Start();

        }
        public int AddTalkTask(string text)
        {
            TalkTask(text);
            return 0;
        }

        public async void TalkTask(string text)
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
