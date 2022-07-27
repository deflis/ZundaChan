
using ZundaChan.BouyomiServer;

namespace FNF.Utility
{
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
            return null;
        }

        public void AddTalkTask(string sTalkText)
        {
            Logger.Info($"AddTalkTask({sTalkText})");
            ipcServer.AddTalkTask(sTalkText);
        }
        public void AddTalkTask(string sTalkText, int iSpeed, int iVolume, int vType)
        {
            Logger.Info($"AddTalkTask({sTalkText},{iSpeed},{iVolume},{vType})");
            ipcServer.AddTalkTask(sTalkText);
        }
        public void AddTalkTask(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            Logger.Info($"AddTalkTask({sTalkText},{iSpeed},{iTone},{iVolume},{vType})");
            ipcServer.AddTalkTask(sTalkText);
        }
        public int AddTalkTask2(string sTalkText)
        {
            Logger.Info($"AddTalkTask2({sTalkText})");
            return ipcServer.AddTalkTask(sTalkText);
        }
        public int AddTalkTask2(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            Logger.Info($"AddTalkTask2({sTalkText},{iSpeed},{iTone},{iVolume},{vType})");
            return ipcServer.AddTalkTask(sTalkText);
        }
        public void ClearTalkTasks()
        {
            Logger.Info($"ClearTalkTasks");
        }
        public void SkipTalkTask()
        {
            Logger.Info($"SkipTalkTask");
        }

        public int TalkTaskCount { get; internal set; }
        public int NowTaskId { get; internal set; }
        public bool NowPlaying { get; internal set; }
        public bool Pause { get; internal set; }

    }

}
