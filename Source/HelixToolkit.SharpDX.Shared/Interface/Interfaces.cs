/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public interface IGUID
    {
        Guid GUID { get; }
    }

    public interface IResourceSharing : IDisposable
    {
        /// <summary>
        /// Get reference count
        /// </summary>
        int ReferenceCount { get; }

        /// <summary>
        /// Add reference counter;
        /// </summary>
        /// <returns>Current count</returns>
        int AddReference();
    }
}
