using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using FNF.Utility;

namespace ZundaChan.Core.BouyomiIpc
{
    public class IpcServer : IDisposable
    {
        private readonly BouyomiChanRemoting ShareIpcObject;
        private readonly IpcServerChannel IpcCh;
        private readonly IProxy proxy;

        public IpcServer(IProxy proxy)
        {
            // IPC接続を行う
            ShareIpcObject = new BouyomiChanRemoting(this);
            IpcCh = new IpcServerChannel("BouyomiChan");
            IpcCh.IsSecured = false;

            ChannelServices.RegisterChannel(IpcCh, false);
            RemotingServices.Marshal(ShareIpcObject, "Remoting", typeof(BouyomiChanRemoting));
            this.proxy = proxy;
        }

        public void Dispose()
        {
            RemotingServices.Disconnect(ShareIpcObject);
            IpcCh.StopListening(null);
            ChannelServices.UnregisterChannel(IpcCh);
        }

        /// <summary>
        /// 棒読みちゃんに音声合成タスクを追加します。
        /// </summary>
        /// <param name="sTalkText">喋らせたい文章</param>
        /// <returns>読み上げタスクID。</returns>
        public int AddTalkTask(TalkTask task)
        {
            return proxy.AddTalkTask(task);
        }
    }
}
