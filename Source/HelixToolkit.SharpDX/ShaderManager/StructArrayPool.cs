using HelixToolkit.SharpDX.Utilities;
using Microsoft.Extensions.Logging;

namespace HelixToolkit.SharpDX;

/// <summary>
/// A pool contains various of binary buffers defined by struct size.
/// </summary>
public sealed class StructArrayPool : DisposeObject, IStructArrayPool
{
    private sealed class ArrayPoolStorage : ReferenceCountedDictionaryPool<int, ArrayStorage, int>
    {
        private static readonly ILogger logger = Logger.LogManager.Create<ArrayPoolStorage>();
        public ArrayPoolStorage() : base(true)
        {
        }

        protected override bool CanCreate(ref int key, ref int argument)
        {
            return argument > 0;
        }

        protected override ArrayStorage OnCreate(ref int key, ref int argument)
        {
            logger.LogInformation("Creating new struct array with size {0}", argument);
            return new ArrayStorage(argument);
        }
    }

    private ArrayPoolStorage? storage;

    public StructArrayPool()
    {
        storage = new ArrayPoolStorage();
    }

    public ArrayStorage? Register(int structSize)
    {
        if (storage is null)
        {
            return null;
        }

        return storage.TryCreateOrGet(structSize, structSize, out var s) ? s : null;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        if (disposeManagedResources)
        {
            RemoveAndDispose(ref storage);
        }
        base.OnDispose(disposeManagedResources);
    }
}
