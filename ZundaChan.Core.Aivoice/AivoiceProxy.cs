using System.Collections.Concurrent;
using System.Reflection;

namespace ZundaChan.Core.Aivoice
{
    public class AivoiceProxy : IProxy, IDisposable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private dynamic ttsControl;
        private Type hostStatus;
        private BlockingCollection<string> playTalkJobs = new BlockingCollection<string>();

        public AivoiceProxy()
        {
            // MEMO: 実装はこちらを参考にした https://gist.github.com/ksasao/10e298a1adcd772d59eec1b43e3f701b

            string path =
                Environment.ExpandEnvironmentVariables("%ProgramW6432%")
                + @"\AI\AIVoice\AIVoiceEditor\AI.Talk.Editor.Api.dll";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("最新のA.I.VOICE Editor をインストールしてください");
            }

            Assembly assembly = Assembly.LoadFrom(path);
            hostStatus = assembly.GetType("AI.Talk.Editor.Api.HostStatus");
            Type type = assembly.GetType("AI.Talk.Editor.Api.TtsControl");

            ttsControl = Activator.CreateInstance(type, new object[] { });
            var hostnames = ttsControl.GetAvailableHostNames();
            ttsControl.Initialize(hostnames[0]);

            if (hostStatus.GetEnumName(ttsControl.Status) == "NotRunning")
            {
                // ホストプログラムを起動する
                ttsControl.StartHost();
            }

            // ホストプログラムに接続する
            ttsControl.Connect();

            Logger.Info("ホストバージョン: " + ttsControl.Version);
            Logger.Info("ホストへの接続を開始しました。");

            Task.Run(ConsumePlayTalkJobs);
        }

        public void Dispose()
        {
            ttsControl.Disconnect();
        }

        public int AddTalkTask(string text)
        {
            playTalkJobs.Add(text);
            return 0;
        }

        private async void ConsumePlayTalkJobs()
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

            while (hostStatus.GetEnumName(ttsControl.Status) == "Busy")
            {
                await Task.Delay(100);
            }
        }
    }
}