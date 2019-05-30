/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    using System;
    /// <summary>
    /// 
    /// </summary>
    public static class Disposer
    {
        /// <summary>
        /// Dispose an object instance and set the reference to null
        /// </summary>
        /// <typeparam name="T">The type of object to dispose</typeparam>
        /// <param name="resource">A reference to the instance for disposal</param>
        /// <remarks>This method hides any thrown exceptions that might occur during disposal of the object (by design)</remarks>
        public static void RemoveAndDispose<T>(ref T resource) where T : class, IDisposable
        {
            if (resource == null)
                return;

            try
            {
                resource.Dispose();
            }
            catch
            {
            }

            resource = null;
        }
    }    
}