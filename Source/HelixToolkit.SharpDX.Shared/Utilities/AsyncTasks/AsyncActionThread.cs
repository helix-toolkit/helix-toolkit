using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    internal sealed class AsyncActionWaitable : DisposeObject
    {
        private readonly object waitable = new object();
        private AsyncActionWaitable() { }
        private Action action;

        public void SetAction(Action action)
        {
            lock (waitable)
            {
                this.action = action;
            }
        }

        public void Trigger()
        {
            lock (waitable)
            {
                var a = action;
                action = null;
                a?.Invoke();
                Monitor.Pulse(waitable);
            }
        }

        public void Wait()
        {
            lock (waitable)
            {
                if (action == null)
                {
                    return;
                }
                Monitor.Wait(waitable);
            }
        }

        private static readonly ConcurrentBag<AsyncActionWaitable> pool = new ConcurrentBag<AsyncActionWaitable>();

        public static AsyncActionWaitable Get()
        {
            if (!pool.TryTake(out AsyncActionWaitable obj))
            {
                obj = new AsyncActionWaitable
                {
                    AddBackToPool = Put
                };
            }
            obj.IncRef();
            return obj;
        }

        static void Put(DisposeObject obj)
        {
            if (obj is AsyncActionWaitable t)
            {
                t.action = null;
                pool.Add(t);
            }
        }
    }
    /// <summary>
    /// Used to run real-time non-rendering tasks in RenderHost.
    /// </summary>
    internal sealed class AsyncActionThread : IDisposable
    {
        private readonly Queue<AsyncActionWaitable> jobs = new Queue<AsyncActionWaitable>();

        private Thread jobThread;
        private volatile bool running = true;
        private bool disposedValue;

        public AsyncActionWaitable EnqueueAction(Action action)
        {
            if (!running)
            { return null; }
            var obj = AsyncActionWaitable.Get();
            obj.SetAction(action);
            lock (jobs)
            {
                jobs.Enqueue(obj);
                Monitor.Pulse(jobs);
            }
            return obj;
        }

        public void Start()
        {
            if (jobThread != null && jobThread.IsAlive)
            {
                return;
            }
            running = true;
            Clear();
            jobThread = new Thread(() =>
            {
                while (running)
                {
                    lock (jobs)
                    {
                        while (jobs.Count > 0 && running)
                        {
                            var job = jobs.Dequeue();
                            Monitor.Exit(jobs);
                            job.Trigger();
                            Monitor.Enter(jobs);
                        }
                        Monitor.Wait(jobs, 100);
                    }
                }
                Clear();
            });
            jobThread.Priority = ThreadPriority.AboveNormal;
            jobThread.Start();
        }

        public void Stop()
        {
            if (!running || jobThread == null)
            {
                return;
            }
            running = false;
            if (jobThread.IsAlive)
            {
                lock (jobs)
                {
                    Monitor.Pulse(jobs);
                }
                jobThread.Join();
                jobThread = null;
            }
        }

        private void Clear()
        {
            lock (jobs)
            {
                while (jobs.Count > 0)
                {
                    jobs.Dequeue().Dispose();
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AsyncActionThread()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}