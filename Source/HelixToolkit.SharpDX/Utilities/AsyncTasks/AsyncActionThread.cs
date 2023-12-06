namespace HelixToolkit.SharpDX;

/// <summary>
/// Used to run real-time non-rendering tasks in RenderHost.
/// </summary>
internal sealed class AsyncActionThread : IDisposable
{
    private readonly Queue<AsyncActionWaitable> jobs = new();

    private Thread? jobThread;
    private volatile bool running = true;
    private bool disposedValue;

    public bool Enabled { set; get; }

    public AsyncActionWaitable? EnqueueAction(Action action)
    {
        if (!running || !Enabled)
        {
            action.Invoke();
            return null;
        }
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
        })
        {
            Priority = ThreadPriority.AboveNormal
        };
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
