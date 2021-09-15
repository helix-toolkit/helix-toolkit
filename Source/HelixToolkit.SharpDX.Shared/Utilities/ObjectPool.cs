using System;
using System.Collections.Concurrent;

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
    namespace Utilities
    {
        public sealed class ObjectPool<T>
        {
            private ConcurrentBag<T> _objects;
            private Func<T> _objectGenerator;
            private readonly int MaxCapacity;

            public int Count { get { return _objects.Count; } }

            public ObjectPool(Func<T> objectGenerator, int maxCapacity = int.MaxValue/2)
            {
                if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
                _objects = new ConcurrentBag<T>();
                _objectGenerator = objectGenerator;
                MaxCapacity = maxCapacity;
            }

            public T GetObject()
            {
                T item;
                if (_objects.TryTake(out item)) return item;
                return _objectGenerator();
            }

            public void PutObject(T item)
            {
                if(Count < MaxCapacity)
                    _objects.Add(item);
            }
        }
    }


}
