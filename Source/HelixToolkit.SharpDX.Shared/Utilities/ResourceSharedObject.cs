#define DEBUG_REF

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
#if DEBUG_REF
    using System.Diagnostics;
#endif
    using System.Threading;
    public abstract class ResourceSharedObject : DisposeObject, IResourceSharing
    {
        private int referenceCount = 0;
        /// <summary>
        /// Manually increase/decrease reference count. Used for resource sharing between multiple models.
        /// </summary>
        public int ReferenceCount
        {
            get { return referenceCount; }
        }

        /// <summary>
        /// Add reference counter;
        /// </summary>
        /// <returns>Current count</returns>
        public int AddReference()
        {
#if DEBUG_REF
            var r = Interlocked.Increment(ref referenceCount);
            Debug.WriteLine("AddReference, Ref = " + r);
            return r;
#else
            return Interlocked.Increment(ref referenceCount);
#endif
        }

        /// <summary>
        /// Decrease reference counter. When counter reach 0, release all internal resources.
        /// </summary>
        /// <returns>Current count</returns>
        public int RemoveReference(bool release)
        {
            int count = Interlocked.Decrement(ref referenceCount);
#if DEBUG_REF
            Debug.WriteLine("RemoveReference, Ref = " + count);
#endif
            if (count <= 0 && release)
            {
                DisposeAndClear();
                if (count < 0)
                {
                    Interlocked.Exchange(ref referenceCount, 0);
                }
            }
            return count;
        }

        /// <summary>
        /// Decrease reference counter. When counter reach 0, release all internal resources automatically
        /// </summary>
        /// <returns>Current count</returns>
        public int RemoveReference()
        {
            return RemoveReference(true);
        }
    }
}
