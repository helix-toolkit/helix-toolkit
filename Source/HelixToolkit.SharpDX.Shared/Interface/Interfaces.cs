// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interfaces.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
        int ReferenceCount { get; }

        /// <summary>
        /// Add reference counter;
        /// </summary>
        /// <returns>Current count</returns>
        int AddReference();

        /// <summary>
        /// Decrease reference counter. When counter reach 0, release all internal resources if release = true
        /// </summary>
        /// <returns>Current count</returns>
        int RemoveReference(bool release);
        /// <summary>
        /// Decrease reference counter. When counter reach 0, release all internal resources automatically
        /// </summary>
        /// <returns>Current count</returns>
        int RemoveReference();
    }




}
