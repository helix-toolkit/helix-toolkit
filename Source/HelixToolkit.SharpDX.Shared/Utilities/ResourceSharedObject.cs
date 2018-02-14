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
    using System.Collections.Generic;
    using System.Diagnostics;
#if DEBUG_REF
    using System.Diagnostics;
#endif

    /// <summary>
    /// 
    /// </summary>
    public abstract class ResourceSharedObject : DisposeObject, IResourceSharing
    {
        private readonly HashSet<Guid> hashSet = new HashSet<Guid>();
        /// <summary>
        /// Attaches the specified model unique identifier.
        /// </summary>
        /// <param name="modelGuid">The model unique identifier.</param>
        public void Attach(Guid modelGuid)
        {
            lock (hashSet)
            {
                hashSet.Add(modelGuid);
            }
        }
        /// <summary>
        /// Detaches the specified model unique identifier. If no model is attached to this resource, resource will be disposed automatically
        /// </summary>
        /// <param name="modelGuid">The model unique identifier.</param>
        public void Detach(Guid modelGuid)
        {
            lock (hashSet)
            {
                hashSet.Remove(modelGuid);
                if (hashSet.Count == 0)
                {
                    this.Dispose();
                }
            }
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
#if DEBUG
            if (hashSet.Count > 0)
            {
                Debug.WriteLine($"ResourceSharedObject, called dispose but still attached to some model. Model Counts:{hashSet.Count}");
            }
#endif
            base.OnDispose(disposeManagedResources);
        }
    }
}
