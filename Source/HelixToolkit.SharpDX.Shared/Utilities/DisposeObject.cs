/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    /// <summary>
    /// Modified version of DisposeCollector from SharpDX. Add null check in RemoveAndDispose(ref object)
    /// </summary>
    public abstract class DisposeObject : DisposeBase
    {
        private readonly HashSet<object> disposables = new HashSet<object>();

        /// <summary>
        /// Gets the number of elements to dispose.
        /// </summary>
        /// <value>The number of elements to dispose.</value>
        public int Count
        {
            get { return disposables.Count; }
        }

        /// <summary>
        /// Disposes all object collected by this class and clear the list. The collector can still be used for collecting.
        /// </summary>
        /// <remarks>
        /// To completely dispose this instance and avoid further dispose, use <see cref="Dispose"/> method instead.
        /// </remarks>
        public void DisposeAndClear()
        {
            foreach(var valueToDispose in disposables)
            {
                if (valueToDispose is IDisposable)
                {
                    ((IDisposable)valueToDispose).Dispose();
                }
                else
                {
                    global::SharpDX.Utilities.FreeMemory((IntPtr)valueToDispose);
                }
            }
            disposables.Clear();
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected override void Dispose(bool disposeManagedResources)
        {
            DisposeAndClear();
        }

        /// <summary>
        /// Adds a <see cref="IDisposable"/> object or a <see cref="IntPtr"/> allocated using <see cref="Utilities.AllocateMemory"/> to the list of the objects to dispose.
        /// </summary>
        /// <param name="toDispose">To dispose.</param>
        /// <exception cref="ArgumentException">If toDispose argument is not IDisposable or a valid memory pointer allocated by <see cref="Utilities.AllocateMemory"/></exception>
        public T Collect<T>(T toDispose)
        {
            if (!(toDispose is IDisposable || toDispose is IntPtr))
                throw new ArgumentException("Argument must be IDisposable or IntPtr");

            // Check memory alignment
            if (toDispose is IntPtr)
            {
                var memoryPtr = (IntPtr)(object)toDispose;
                if (!global::SharpDX.Utilities.IsMemoryAligned(memoryPtr))
                    throw new ArgumentException("Memory pointer is invalid. Memory must have been allocated with Utilties.AllocateMemory");
            }

            if (!Equals(toDispose, default(T)) && !disposables.Contains(toDispose))
            {
                disposables.Add(toDispose);
            }
            return toDispose;
        }

        /// <summary>
        /// Dispose a disposable object and set the reference to null. Removes this object from this instance..
        /// </summary>
        /// <param name="objectToDispose">Object to dispose.</param>
        public void RemoveAndDispose<T>(ref T objectToDispose)
        {
            if (objectToDispose != null)
            {
                Remove(objectToDispose);

                var disposableObject = objectToDispose as IDisposable;
                if (disposableObject != null)
                {
                    // Dispose the component
                    disposableObject.Dispose();
                }
                else
                {
                    var localData = (object)objectToDispose;
                    var dataPointer = (IntPtr)localData;
                    global::SharpDX.Utilities.FreeMemory(dataPointer);
                }
                objectToDispose = default(T);
            }
        }

        /// <summary>
        /// Removes a disposable object to the list of the objects to dispose.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDisposeArg">To dispose.</param>
        public void Remove<T>(T toDisposeArg)
        {
            if (disposables.Contains(toDisposeArg))
            {
                disposables.Remove(toDisposeArg);
            }
        }
    }
}
