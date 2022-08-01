using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace ZundaChan.Core.Aivoice
{
    public class AivoiceProxy : IProxy, IDisposable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private dynamic ttsControl;
        private Type hostStatus;
        private BlockingCollection<TalkTask> playTalkJobs = new BlockingCollection<TalkTask>();

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

        public int AddTalkTask(TalkTask task)
        {
            playTalkJobs.Add(task);
            return 0;
        }

        private async void ConsumePlayTalkJobs()
        {
            foreach (var task in playTalkJobs.GetConsumingEnumerable(CancellationToken.None))
            {
                try
                {
                    await PlayVoiceAsync(task);
                }
                catch (Exception ex)
                {
                    Logger.Error("ボイス再生でエラーが発生しました", ex);
                }
            }
        }

        private async Task PlayVoiceAsync(TalkTask task)
        {
            // マスターコントロールを取得する
            string controlJson = ttsControl.MasterControl;
            string backupText = ttsControl.Text;
            var control = JsonSerializer.Deserialize<MasterControl>(controlJson);
            if (control == null)
            {
                Logger.Error("マスターコントロールのJSONパースに失敗しました。", controlJson);
                throw new Exception("マスターコントロールのJSONパースに失敗しました。");
            }
            if (task.Tone != -1)
            {
                control.Pitch = task.Tone / 100;
            }
            if (task.Volume != -1)
            {
                control.Volume = task.Volume / 100;
            }
            if (task.Speed != -1)
            {
                control.Speed = task.Speed / 100;
            }
            ttsControl.Text = task.Text;

            ttsControl.MasterControl = JsonSerializer.Serialize(control);
            ttsControl.Play();

            // 再生完了イベントがないのでBusyでなくなるまで待つ
            while (hostStatus.GetEnumName(ttsControl.Status) == "Busy")
            {
                await Task.Delay(100);
            }
            // 変更した状態を元に戻す
            ttsControl.MasterControl = controlJson;
            ttsControl.Text = backupText;
        }
        public class MasterControl
        {
            public float Volume { get; set; }
            public float Speed { get; set; }
            public float Pitch { get; set; }
            public float PitchRange { get; set; }
            public long MiddlePause { get; set; }
            public long LongPause { get; set; }
            public long SentencePause { get; set; }
        }
    }
}