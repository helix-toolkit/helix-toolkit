using System.Collections.Concurrent;

namespace HelixToolkit.SharpDX.Utilities;

public sealed class ObjectPool<T>
{
    private readonly ConcurrentBag<T> _objects;
    private readonly Func<T> _objectGenerator;
    private readonly int MaxCapacity;

    public int Count
    {
        get
        {
            return _objects.Count;
        }
    }

    public ObjectPool(Func<T> objectGenerator, int maxCapacity = int.MaxValue / 2)
    {
        _objects = new ConcurrentBag<T>();
        _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
        MaxCapacity = maxCapacity;
    }

    public T GetObject()
    {
        if (_objects.TryTake(out T? item))
            return item;
        return _objectGenerator();
    }

    public void PutObject(T item)
    {
        if (Count < MaxCapacity)
            _objects.Add(item);
    }
}
