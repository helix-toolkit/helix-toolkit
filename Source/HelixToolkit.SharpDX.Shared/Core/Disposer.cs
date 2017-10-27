// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Disposer.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Dispose an object instance and set the reference to null
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;

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