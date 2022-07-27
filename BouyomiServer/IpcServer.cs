using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using FNF.Utility;
using NAudio.Wave;
using ZundaChan.Voicevox;
using System.Reactive.Linq;

namespace ZundaChan.BouyomiServer
{
    internal class IpcServer : IDisposable
    {
        private readonly BouyomiChanRemoting ShareIpcObject;
        private readonly IpcServerChannel IpcCh;
        private readonly Proxy proxy;

        public IpcServer(Proxy proxy)
        {
            ShareIpcObject = new BouyomiChanRemoting(this);
            IpcCh = new IpcServerChannel("BouyomiChan");
            IpcCh.IsSecured = false;

            ChannelServices.RegisterChannel(IpcCh, false);
            RemotingServices.Marshal(ShareIpcObject, "Remoting", typeof(FNF.Utility.BouyomiChanRemoting));
            this.proxy = proxy;
        }

        public void Dispose()
        {
            RemotingServices.Disconnect(ShareIpcObject);
            IpcCh.StopListening(null);
            ChannelServices.UnregisterChannel(IpcCh);
        }

        public int AddTalkTask(string sTalkText)
        {
            return proxy.AddTalkTask(sTalkText);
        }
    }
}
