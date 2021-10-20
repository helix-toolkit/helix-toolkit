/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpDX.Toolkit
{
    /// <summary>
    /// A disposable component base class.
    /// </summary>
    public abstract class Component : ComponentBase, IDisposable
    {
        /// <summary>
        /// Gets or sets the disposables.
        /// </summary>
        /// <value>The disposables.</value>
        protected DisposeCollector DisposeCollector
        {
            get; set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        protected internal Component()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Component" /> class with an immutable name.
        /// </summary>
        /// <param name="name">The name.</param>
        protected Component(string name) : base(name)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is attached to a collector.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is attached to a collector; otherwise, <c>false</c>.
        /// </value>
        internal bool IsAttached
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        protected internal bool IsDisposed
        {
            get; private set;
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal bool IsDisposing
        {
            get; private set;
        }

        /// <summary>
        /// Occurs when when Dispose is called.
        /// </summary>
        public event EventHandler<EventArgs> Disposing;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposing = true;

                // Call the disposing event.
                Disposing?.Invoke(this, EventArgs.Empty);
                Dispose(true);
                IsDisposed = true;
                //GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                // Dispose all ComObjects
                if (DisposeCollector != null)
                    DisposeCollector.Dispose();
                DisposeCollector = null;
            }
        }

        /// <summary>
        /// Adds a disposable object to the list of the objects to dispose.
        /// </summary>
        /// <param name="toDisposeArg">To dispose.</param>
        protected internal T ToDispose<T>(T toDisposeArg)
        {
            if (!ReferenceEquals(toDisposeArg, null))
            {
                if (DisposeCollector == null)
                    DisposeCollector = new DisposeCollector();
                return DisposeCollector.Collect(toDisposeArg);
            }
            return default(T);
        }

        /// <summary>
        /// Dispose a disposable object and set the reference to null. Removes this object from the ToDispose list.
        /// </summary>
        /// <param name="objectToDispose">Object to dispose.</param>
        protected internal void RemoveAndDispose<T>(ref T objectToDispose)
        {
            if (!ReferenceEquals(objectToDispose, null) && DisposeCollector != null)
                DisposeCollector.RemoveAndDispose(ref objectToDispose);
        }

        /// <summary>
        /// Removes a disposable object to the list of the objects to dispose.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDisposeArg">To dispose.</param>
        protected internal void RemoveToDispose<T>(T toDisposeArg)
        {
            if (!ReferenceEquals(toDisposeArg, null) && DisposeCollector != null)
                DisposeCollector.Remove(toDisposeArg);
        }
    }
}