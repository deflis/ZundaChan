using AI.Talk.Editor.Api;
using System.Collections.Concurrent;

namespace ZundaChan.Core.Aivoice
{
    public class AivoiceProxy : IProxy, IDisposable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private TtsControl ttsControl;
        private BlockingCollection<string> playTalkJobs = new BlockingCollection<string>();

        public AivoiceProxy()
        {
            var thread = new Thread(new ThreadStart(OnStart));
            thread.IsBackground = true;
            thread.Start();

            ttsControl = new TtsControl();
            var hostnames= ttsControl.GetAvailableHostNames();
            ttsControl.Initialize(hostnames[0]);

            if (ttsControl.Status == HostStatus.NotRunning)
            {
                // ホストプログラムを起動する
                ttsControl.StartHost();
            }

            // ホストプログラムに接続する
            ttsControl.Connect();

            Logger.Info("ホストバージョン: " + ttsControl.Version);
            Logger.Info("ホストへの接続を開始しました。");

        }

        public void Dispose()
        {
            ttsControl.Disconnect();
        }

        public int AddTalkTask(string text)
        {
            TalkTask(text);
            return 0;
        }

        private void TalkTask(string text)
        {
            try
            {
                playTalkJobs.Add(text);
            }
            catch (Exception ex)
            {
                Logger.Error("ボイス生成でエラーが発生しました", ex);
            }
        }

        private async void OnStart()
        {
            foreach (var text in playTalkJobs.GetConsumingEnumerable(CancellationToken.None))
            {
                try
                {
                    await PlayVoiceAsync(text);
                }
                catch (Exception ex)
                {
                    Logger.Error("ボイス再生でエラーが発生しました", ex);
                }
            }
        }

        private async Task PlayVoiceAsync(string text)
        {
            ttsControl.Text = text;
            ttsControl.Play();

            while (ttsControl.Status == HostStatus.Busy)
            {
                await Task.Delay(100);
            }
        }
    }
}