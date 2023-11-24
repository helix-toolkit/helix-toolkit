using System.Collections.Concurrent;

namespace HelixToolkit.SharpDX;

internal sealed class AsyncActionWaitable : DisposeObject
{
    private readonly object waitable = new();
    private AsyncActionWaitable() { }
    private Action? action;

    public void SetAction(Action? action)
    {
        this.action = action;
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

    private static readonly ConcurrentBag<AsyncActionWaitable> pool = new();

    public static AsyncActionWaitable Get()
    {
        if (!pool.TryTake(out AsyncActionWaitable? obj))
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
