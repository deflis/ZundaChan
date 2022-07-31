using NAudio.Wave;
using System.Collections.Concurrent;
using System.Text.Json;

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
            Task.Run(ConsumePlayTalkJobs);

        }

        /// <summary>
        /// 読み上げ音声を登録する
        /// </summary>
        /// <param name="sTalkText">喋らせたい文章</param>
        /// <returns>読み上げタスクID。</returns>
        public int AddTalkTask(TalkTask task)
        {
            TalkTask(task);
            return 0;
        }

        private async void TalkTask(TalkTask task)
        {
            try
            {
                var speakerId = Config.SpeakerId;
                var rawQuery = await client.BuildAudioQueryJsonAsync(task.Text, speakerId);
                var query = Parse(rawQuery);
                if (task.Volume != -1)
                {
                    query.volumeScale = (double)query.volumeScale * task.Volume / 100;
                }
                if (task.Tone != -1)
                {
                    query.pitchScale = ((double)task.Tone / 100) - 1;
                }
                if (task.Speed != -1)
                {
                    query.speedScale = (double)query.speedScale * task.Speed / 100;
                }
                Logger.Debug(JsonSerializer.Serialize(query));
                playTalkJobs.Add(await client.SynthesisAsync( JsonSerializer.Serialize(query), speakerId));
            }
            catch (Exception ex)
            {
                Logger.Error("ボイス生成でエラーが発生しました", ex);
            }
        }

        private async void ConsumePlayTalkJobs()
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

        private async Task PlayVoiceAsync(Stream stream)
        {
            using (stream)
            {
                using var outputDevice = new WaveOutEvent() { DeviceNumber = Config.DeviceNumber };

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

        static dynamic Parse(string json)
        {
            using var document = JsonDocument.Parse(json);
            return toExpandoObject(document.RootElement);

            static object? propertyValue(JsonElement elm) =>
                elm.ValueKind switch
                {
                    JsonValueKind.Null => null,
                    JsonValueKind.Number => elm.GetDecimal(),
                    JsonValueKind.String => elm.GetString(),
                    JsonValueKind.False => false,
                    JsonValueKind.True => true,
                    JsonValueKind.Array => elm.EnumerateArray().Select(m => propertyValue(m)).ToArray(),
                    JsonValueKind.Undefined => null,
                    JsonValueKind.Object => toExpandoObject(elm),
                    _ => toExpandoObject(elm),
                };

            static System.Dynamic.ExpandoObject toExpandoObject(JsonElement elm) =>
                elm.EnumerateObject()
                .Aggregate(
                    new System.Dynamic.ExpandoObject(),
                    (exo, prop) => { ((IDictionary<string, object>)exo).Add(prop.Name, propertyValue(prop.Value)); return exo; });
        }
    }
}
