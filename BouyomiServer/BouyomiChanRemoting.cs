using System;

namespace FNF.Utility
{
    class BouyomiChanRemoting : MarshalByRefObject
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override object InitializeLifetimeService()
        {
            return null;
        }
        public delegate void AddTalkTaskEventHandler(string sTalkText);
        public delegate void SimpleEventHandler();

        public event AddTalkTaskEventHandler OnAddTalkTask;
        public event SimpleEventHandler OnClearTalkTask;
        public event SimpleEventHandler OnSkipTalkTask;

        public void AddTalkTask(string sTalkText) {
            Logger.Info($"AddTalkTask({sTalkText})");
            OnAddTalkTask(sTalkText);
        }
        public void AddTalkTask(string sTalkText, int iSpeed, int iVolume, int vType)
        {
            Logger.Info($"AddTalkTask({sTalkText},{iSpeed},{iVolume},{vType})");
            OnAddTalkTask(sTalkText);
        }
        public void AddTalkTask(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            Logger.Info($"AddTalkTask({sTalkText},{iSpeed},{iTone},{iVolume},{vType})");
            OnAddTalkTask(sTalkText);
        }
        public int AddTalkTask2(string sTalkText)
        {
            Logger.Info($"AddTalkTask2({sTalkText})"); 
            OnAddTalkTask(sTalkText);
            return 0;
        }
        public int AddTalkTask2(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            Logger.Info($"AddTalkTask2({sTalkText},{iSpeed},{iTone},{iVolume},{vType})");
            OnAddTalkTask(sTalkText);
            return 0;
        }
        public void ClearTalkTasks()
        {
            Logger.Info($"ClearTalkTasks");
            OnClearTalkTask();
        }
        public void SkipTalkTask()
        {
            Logger.Info($"SkipTalkTask");
            OnSkipTalkTask();
        }

        public int TalkTaskCount { get; internal set; }
        public int NowTaskId { get; internal set; }
        public bool NowPlaying { get; internal set; }
        public bool Pause { get; internal set; }

    }

}
