using ZundaChan.Core;
using ZundaChan.Core.BouyomiIpc;

namespace FNF.Utility
{
    /// <summary>
    /// .NET Remotingのためのクラス。棒読みちゃんのサンプルソースをベースに作っています。
    /// </summary>
    class BouyomiChanRemoting : MarshalByRefObject
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IpcServer ipcServer;

        public BouyomiChanRemoting(IpcServer ipcServer)
        {
            this.ipcServer = ipcServer;
        }

        public override object? InitializeLifetimeService()
        {
            // nullで無期限となる
            return null;
        }

        /// <summary>
        /// 棒読みちゃんに音声合成タスクを追加します。
        /// </summary>
        /// <param name="sTalkText">喋らせたい文章</param>
        public void AddTalkTask(string sTalkText)
        {
            Logger.Info($"AddTalkTask({sTalkText})");
            ipcServer.AddTalkTask(new TalkTask(sTalkText));
        }

        /// <summary>
        /// 棒読みちゃんに音声合成タスクを追加します。(音程指定無し版。以前のバージョンとの互換性の為に残しています。)
        /// </summary>
        /// <param name="sTalkText">喋らせたい文章</param>
        /// <param name="iSpeed"   >再生。(-1で棒読みちゃん側の画面で選んでいる速度)</param>
        /// <param name="iVolume"  >音量。(-1で棒読みちゃん側の画面で選んでいる音量)</param>
        /// <param name="vType"    >声の種類。(Defaultで棒読みちゃん側の画面で選んでいる声)</param>
        public void AddTalkTask(string sTalkText, int iSpeed, int iVolume, int vType)
        {
            Logger.Info($"AddTalkTask({sTalkText},{iSpeed},{iVolume},{vType})");
            ipcServer.AddTalkTask(new TalkTask(sTalkText) { Speed = iSpeed, Volume = iVolume, Type = vType });
        }

        /// <summary>
        /// 棒読みちゃんに音声合成タスクを追加します。
        /// </summary>
        /// <param name="sTalkText">喋らせたい文章</param>
        /// <param name="iSpeed"   >速度。(-1で棒読みちゃん側の画面で選んでいる速度)</param>
        /// <param name="iTone"    >音程。(-1で棒読みちゃん側の画面で選んでいる音程)</param>
        /// <param name="iVolume"  >音量。(-1で棒読みちゃん側の画面で選んでいる音量)</param>
        /// <param name="vType"    >声の種類。(Defaultで棒読みちゃん側の画面で選んでいる声)</param>
        public void AddTalkTask(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            Logger.Info($"AddTalkTask({sTalkText},{iSpeed},{iTone},{iVolume},{vType})");
            ipcServer.AddTalkTask(new TalkTask(sTalkText) { Speed = iSpeed, Tone = iTone, Volume = iVolume, Type = vType });
        }

        /// <summary>
        /// 棒読みちゃんに音声合成タスクを追加します。読み上げタスクIDを返します。
        /// </summary>
        /// <param name="sTalkText">喋らせたい文章</param>
        /// <returns>読み上げタスクID。</returns>
        public int AddTalkTask2(string sTalkText)
        {
            Logger.Info($"AddTalkTask2({sTalkText})");
            return ipcServer.AddTalkTask(new TalkTask(sTalkText));
        }

        /// <summary>
        /// 棒読みちゃんに音声合成タスクを追加します。読み上げタスクIDを返します。
        /// </summary>
        /// <param name="sTalkText">喋らせたい文章</param>
        /// <param name="iSpeed"   >速度。(-1で棒読みちゃん側の画面で選んでいる速度)</param>
        /// <param name="iTone"    >音程。(-1で棒読みちゃん側の画面で選んでいる音程)</param>
        /// <param name="iVolume"  >音量。(-1で棒読みちゃん側の画面で選んでいる音量)</param>
        /// <param name="vType"    >声の種類。(Defaultで棒読みちゃん側の画面で選んでいる声)</param>
        /// <returns>読み上げタスクID。</returns>
        public int AddTalkTask2(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            Logger.Info($"AddTalkTask2({sTalkText},{iSpeed},{iTone},{iVolume},{vType})");
            return ipcServer.AddTalkTask(new TalkTask(sTalkText) { Speed = iSpeed, Tone = iTone, Volume = iVolume, Type = vType });
        }

        /// <summary>
        /// 棒読みちゃんの残りのタスクを全て消去します。
        /// </summary>
        public void ClearTalkTasks()
        {
            Logger.Info($"ClearTalkTasks");
        }

        /// <summary>
        /// 棒読みちゃんの現在のタスクを中止して次の行へ移ります。
        /// </summary>
        public void SkipTalkTask()
        {
            Logger.Info($"SkipTalkTask");
        }


        /// <summary>
        /// 棒読みちゃんの現在のタスク数（再生待ち行数）を取得します。
        /// </summary>
        public int TalkTaskCount { get; internal set; }

        /// <summary>
        /// 棒読みちゃんの現在再生中のタスクIDを取得します。
        /// </summary>
        public int NowTaskId { get; internal set; }

        /// <summary>
        /// 棒読みちゃんが現在、音声を再生している最中かどうかを取得します。
        /// </summary>
        public bool NowPlaying { get; internal set; }

        /// <summary>
        /// 棒読みちゃんが一時停止中かどうかを取得・設定します。
        /// ※現在の行の再生が終了するまで停止しません。
        /// </summary>
        public bool Pause { get; internal set; }

    }

}
