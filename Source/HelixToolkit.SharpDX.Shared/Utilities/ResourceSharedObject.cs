/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUG_REF

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    using System;
#if DEBUG_REF
    using System.Diagnostics;
#endif
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public abstract class ResourceSharedObject : DisposeObject, IResourceSharing
    {
        /// <summary>
        /// Start with reference = 1.
        /// </summary>
        private int referenceCount = 1;
        /// <summary>
        /// Manually increase/decrease reference count. Used for resource sharing between multiple models.
        /// </summary>
        public int ReferenceCount
        {
            get { return referenceCount; }
        }

        /// <summary>
        /// Increment reference counter by 1.
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
        /// Decrease the reference counter. If counter reach zero, dispose the actual contents
        /// </summary>
        public override void Dispose()
        {
            if(Interlocked.Decrement(ref referenceCount) == 0)
            {
                base.Dispose();
            }
        }
    }
}
