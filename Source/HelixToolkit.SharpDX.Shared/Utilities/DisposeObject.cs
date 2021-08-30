/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    /// <summary>
    /// Modified version of DisposeCollector from SharpDX. Add null check in RemoveAndDispose(ref object)
    /// </summary>
    public abstract class DisposeObject : IDisposable
    {
        /// <summary>
        /// Occurs when this instance is starting to be disposed.
        /// </summary>
        public event EventHandler<BoolArgs> Disposing;

        /// <summary>
        /// Occurs when this instance is fully disposed.
        /// </summary>
        public event EventHandler<BoolArgs> Disposed;


        private readonly Dictionary<IDisposable, int> disposables = new Dictionary<IDisposable, int>();

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
        /// To completely dispose this instance and avoid further dispose, use <see cref="OnDispose"/> method instead.
        /// </remarks>
        public virtual void DisposeAndClear()
        {
            var arr = disposables.ToArray();
            foreach(var valueToDispose in arr)
            {
                /// Must dispose N times to properly decrement reference counter inside <see cref="ReferenceCountDisposeObject"/> type objects.
                for (int i = 0; i < valueToDispose.Value; ++i)
                {
                    valueToDispose.Key.Dispose();
                }
            }
            disposables.Clear();
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected virtual void OnDispose(bool disposeManagedResources)
        {
            DisposeAndClear();
        }

        /// <summary>
        /// Adds a <see cref="IDisposable"/> object or a <see cref="IntPtr"/> allocated using <see cref="global::SharpDX.Utilities.AllocateMemory"/> to the list of the objects to dispose.
        /// <para><see cref="ReferenceCountDisposeObject"/> type object can be collected multiple times, but make sure its reference counter has been incremented accordingly before collecting.</para>
        /// </summary>
        /// <param name="toDispose">To dispose.</param>
        /// <exception cref="InvalidOperationException">Throws when object is not a <see cref="ReferenceCountDisposeObject"/> and being collected more than once.</exception>
        /// <exception cref="ArgumentException">If toDispose argument is not IDisposable or a valid memory pointer allocated by <see cref="global::SharpDX.Utilities.AllocateMemory"/></exception>
        public T Collect<T>(T toDispose)
        {
            if(toDispose == null)
            { return default(T); }
            else if(toDispose is IDisposable disposable)
            {
                if (!Equals(toDispose, default(T)))
                {
                    bool hasObj = disposables.TryGetValue(disposable, out var count);
                    if (!hasObj) // If object is not in collections, simply adds it.
                    {
                        disposables.Add(disposable, 1);
                    }
                    else if (toDispose is ReferenceCountDisposeObject) // Increment the counter, so object will be disposed N times during DisposeAndClear.
                    {
                        disposables[disposable] = ++count;
                    }
                    else
                    {
                        throw new InvalidOperationException("Object has already been added to disposable collection.");
                    }
                }
                return toDispose;
            }
            else
            {
                throw new ArgumentException("Argument must be IDisposable");
            }
        }

        /// <summary>
        /// Dispose a disposable object and set the reference to null. Removes this object from this instance..
        /// </summary>
        /// <param name="objectToDispose">Object to dispose.</param>
        public void RemoveAndDispose<T>(ref T objectToDispose) where T : IDisposable
        {
            if (Remove(objectToDispose) >= 0)
            {
                (objectToDispose as IDisposable).Dispose();
                objectToDispose = default(T);
            }
        }

        /// <summary>
        /// Removes a disposable object to the list of the objects to dispose.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDisposeArg">To dispose.</param>
        private int Remove<T>(T toDisposeArg) where T : IDisposable
        {
            if (toDisposeArg is IDisposable disposable && disposables.TryGetValue(disposable, out int count))
            {
                --count;
                if (count == 0) // Removes it only counter becomes 0.
                {
                    disposables.Remove(disposable);
                }
                else
                {
                    disposables[disposable] = count;
                }
                return count;
            }
            return -1;
        }

        #region Idisposable

        ///// <summary>
        ///// Releases unmanaged resources and performs other cleanup operations before the
        ///// <see cref="DisposeObject"/> is reclaimed by garbage collection.
        ///// </summary>
        //[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
        //~DisposeObject()
        //{
        //    // Finalizer calls Dispose(false)
        //    Dispose(false);
        //}

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // TODO Should we throw an exception if this method is called more than once?
            if (!IsDisposed)
            {
                Disposing?.Invoke(this, disposing ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);

                OnDispose(disposing);
                //GC.SuppressFinalize(this);

                IsDisposed = true;

                Disposed?.Invoke(this, disposing ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);
                Disposing = null;
                Disposed = null;
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }
            backingField = value;
            return true;
        }
    }

    public abstract class ReferenceCountDisposeObject : IDisposable
    {
        /// <summary>
        /// Occurs when this instance is starting to be disposed.
        /// </summary>
        public event EventHandler<BoolArgs> Disposing;

        /// <summary>
        /// Occurs when this instance is fully disposed.
        /// </summary>
        public event EventHandler<BoolArgs> Disposed;


        private readonly Dictionary<IDisposable, int> disposables = new Dictionary<IDisposable, int>();

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
        /// To completely dispose this instance and avoid further dispose, use <see cref="OnDispose"/> method instead.
        /// </remarks>
        public virtual void DisposeAndClear()
        {
            var arr = disposables.ToArray();
            foreach (var valueToDispose in arr)
            {
                for (int i = 0; i < valueToDispose.Value; ++i)
                {
                    valueToDispose.Key.Dispose();
                }
            }
            disposables.Clear();
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected virtual void OnDispose(bool disposeManagedResources)
        {
            DisposeAndClear();
        }

        /// <summary>
        /// Adds a <see cref="IDisposable"/> object or a <see cref="IntPtr"/> allocated using <see cref="global::SharpDX.Utilities.AllocateMemory"/> to the list of the objects to dispose.
        /// <para><see cref="ReferenceCountDisposeObject"/> type object can be collected multiple times, but make sure its reference counter has been incremented accordingly before collecting.</para>
        /// </summary>
        /// <param name="toDispose">To dispose.</param>
        /// <exception cref="InvalidOperationException">Throws when object is not a <see cref="ReferenceCountDisposeObject"/> and being collected more than once.</exception>
        /// <exception cref="ArgumentException">If toDispose argument is not IDisposable or a valid memory pointer allocated by <see cref="global::SharpDX.Utilities.AllocateMemory"/></exception>
        public T Collect<T>(T toDispose)
        {
            if (toDispose == null)
            { return default(T); }
            else if (toDispose is IDisposable disposable)
            {
                if (!Equals(toDispose, default(T)))
                {
                    bool hasObj = disposables.TryGetValue(disposable, out var count);
                    if (!hasObj)
                    {
                        disposables.Add(disposable, 1);
                    }
                    else if (toDispose is ReferenceCountDisposeObject)
                    {
                        disposables[disposable] = ++count;
                    }
                    else
                    {
                        throw new InvalidOperationException("Object has already been added to disposable collection.");
                    }
                }
                return toDispose;
            }
            else
            {
                throw new ArgumentException("Argument must be IDisposable");
            }
        }

        /// <summary>
        /// Dispose a disposable object and set the reference to null. Removes this object from this instance..
        /// </summary>
        /// <param name="objectToDispose">Object to dispose.</param>
        public void RemoveAndDispose<T>(ref T objectToDispose) where T : IDisposable
        {
            if (Remove(objectToDispose) >= 0)
            {
                (objectToDispose as IDisposable).Dispose();
                objectToDispose = default(T);
            }
        }

        /// <summary>
        /// Removes a disposable object to the list of the objects to dispose.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDisposeArg">To dispose.</param>
        /// <returns></returns>
        private int Remove<T>(T toDisposeArg) where T : IDisposable
        {
            if (toDisposeArg is IDisposable disposable && disposables.TryGetValue(disposable, out int value))
            {
                --value;
                if (value == 0)
                {
                    disposables.Remove(disposable);
                }
                else
                {
                    disposables[disposable] = value;
                }
                return value;
            }
            return -1;
        }

        #region Idisposable

        ///// <summary>
        ///// Releases unmanaged resources and performs other cleanup operations before the
        ///// <see cref="ReferenceCountDisposeObject"/> is reclaimed by garbage collection.
        ///// </summary>
        //[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
        //~ReferenceCountDisposeObject()
        //{
        //    // Finalizer calls Dispose(false)
        //    Dispose(false);
        //}

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// Flag if this object is in object pool. Set to true to avoid disposing and return it to the pool
        /// </summary>
        internal bool IsPooled = false;
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
        public void Dispose()
        {
            Dispose(true);
        }

        private int refCounter = 1;

        internal int IncRef()
        {
            return Interlocked.Increment(ref refCounter);
        }
        /// <summary>
        /// Forces the dispose.
        /// </summary>
        internal void ForceDispose()
        {
            IsPooled = false;
            Interlocked.Exchange(ref refCounter, 1);
            Dispose();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (IsPooled)
            {
                OnPutBackToPool();
            }
            else if (Interlocked.Decrement(ref refCounter) == 0 && !IsDisposed)
            {
                Disposing?.Invoke(this, disposing ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);

                OnDispose(disposing);
                //GC.SuppressFinalize(this);

                IsDisposed = true;

                Disposed?.Invoke(this, disposing ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);
                Disposing = null;
                Disposed = null;
            }
        }

        protected virtual void OnPutBackToPool() {}
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            return true;
        }
    }
}
