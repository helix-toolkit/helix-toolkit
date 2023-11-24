using System.Collections.Concurrent;

namespace HelixToolkit.SharpDX.Utilities;

public sealed class IdHelper
{
    private int maxId_ = 0;
    private readonly ConcurrentStack<int> freedIds_ = new();
    public int GetNextId()
    {
        return freedIds_.TryPop(out var id) ? id : Interlocked.Increment(ref maxId_);
    }

    public int MaxId => Interlocked.CompareExchange(ref maxId_, 0, 0);

    public int Count => MaxId - freedIds_.Count;

    public void ReleaseId(int id)
    {
        freedIds_.Push(id);
    }
}
