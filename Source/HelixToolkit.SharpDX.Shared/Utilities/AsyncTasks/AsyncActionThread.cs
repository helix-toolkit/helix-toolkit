using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
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
    internal sealed class AsyncActionWaitable
    {
        private readonly object waitable = new object();
        private AsyncActionWaitable() { }
        public Action Action;

        public void Trigger()
        {
            lock (waitable)
            {
                Action?.Invoke();
                Action = null;
                Monitor.Pulse(waitable);
            }
        }

        public void Wait()
        {
            lock (waitable)
            {
                if (Action == null)
                {
                    return;
                }
                Monitor.Wait(waitable);
            }
        }

        private static readonly ConcurrentBag<AsyncActionWaitable> pool = new ConcurrentBag<AsyncActionWaitable>();

        public static AsyncActionWaitable Get()
        {
            return pool.TryTake(out var obj) ? obj : new AsyncActionWaitable();
        }

        public static void Put(AsyncActionWaitable obj)
        {
            obj.Action = null;
            pool.Add(obj);
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
            obj.Action = action;
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
                            AsyncActionWaitable.Put(job);
                            Monitor.Enter(jobs);
                        }
                        Monitor.Wait(jobs, 100);
                    }
                }
                Clear();
            });
            jobThread.Priority = ThreadPriority.Highest;
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
                    AsyncActionWaitable.Put(jobs.Dequeue());
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