using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using FNF.Utility;
using NAudio.Wave;
using ZundaChan.Voicevox;
using System.Reactive.Linq;

namespace ZundaChan.BouyomiServer
{
    public class IpcServer : IDisposable
    {
        private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private readonly BouyomiChanRemoting ShareIpcObject = new BouyomiChanRemoting();
        private readonly IpcServerChannel IpcCh;
        private readonly Client client;
        private readonly int deviceNumber;
        private readonly int speackerId;
        private readonly IDisposable subject;

        public IpcServer(Client client, int deviceNumber, int speackerId)
        {
            IpcCh = new IpcServerChannel("BouyomiChan");
            IpcCh.IsSecured = false;
            this.client = client;
            this.deviceNumber = deviceNumber;
            this.speackerId = speackerId;

            IObservable<string> observable = Observable.FromEvent<BouyomiChanRemoting.AddTalkTaskEventHandler, string>(
                (target) => ShareIpcObject.OnAddTalkTask += target, (target) => ShareIpcObject.OnAddTalkTask -= target);
            subject = observable.Subscribe(async (e) => await AddTalkTask(e));
            ShareIpcObject.OnClearTalkTask += IPCClearTalkTask;
            ShareIpcObject.OnSkipTalkTask += IPCSkipTalkTask;

            ChannelServices.RegisterChannel(IpcCh, false);
            RemotingServices.Marshal(ShareIpcObject, "Remoting", typeof(FNF.Utility.BouyomiChanRemoting));
        }

        public void Dispose()
        {
            RemotingServices.Disconnect(ShareIpcObject);
            IpcCh.StopListening(null);
            ChannelServices.UnregisterChannel(IpcCh);
            subject.Dispose();
            Semaphore.Dispose();
        }

        public void SetTaskId(int Id)
        {
        }

        private async Task AddTalkTask(string TalkText)
        {
            using (var outputDevice = new WaveOutEvent() { DeviceNumber = deviceNumber })
            using (var stream = await client.CreateAsync(TalkText, speackerId))
            {
                var tcs = new TaskCompletionSource<string>();
                EventHandler<StoppedEventArgs>? h = null;
                h = (_, _) =>
                {
                    outputDevice.PlaybackStopped -= h;
                    tcs.SetResult("");
                };
                outputDevice.PlaybackStopped += h;

                await Semaphore.WaitAsync();
                try
                {
                    outputDevice.Init(new WaveFileReader(stream));
                    outputDevice.Play();
                    await tcs.Task;
                }
                finally
                {
                    Semaphore.Release();
                }
            }
        }


        private void IPCClearTalkTask()
        {
        }

        private void IPCSkipTalkTask()
        {
        }
        class SizeQueue<T>
        {
            private readonly Queue<T> queue = new Queue<T>();
            private readonly int maxSize;
            public SizeQueue(int maxSize) { this.maxSize = maxSize; }

            public void Enqueue(T item)
            {
                lock (queue)
                {
                    while (queue.Count >= maxSize)
                    {
                        Monitor.Wait(queue);
                    }
                    queue.Enqueue(item);
                    if (queue.Count == 1)
                    {
                        // wake up any blocked dequeue
                        Monitor.PulseAll(queue);
                    }
                }
            }
            public T Dequeue()
            {
                lock (queue)
                {
                    while (queue.Count == 0)
                    {
                        Monitor.Wait(queue);
                    }
                    T item = queue.Dequeue();
                    if (queue.Count == maxSize - 1)
                    {
                        // wake up any blocked enqueue
                        Monitor.PulseAll(queue);
                    }
                    return item;
                }
            }
        }
    }
}
