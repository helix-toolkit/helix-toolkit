/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX.Model
#endif
{
    using Scene;
    /// <summary>
    /// 
    /// </summary>
#if NETFX_CORE
    public abstract class Element3DCore : IDisposable, IRenderable, IGUID, ITransform, INotifyPropertyChanged
#else
    public abstract class Element3DCore : FrameworkContentElement, IDisposable, INotifyPropertyChanged
#endif
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GUID
        {
            get { return SceneNode.GUID; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix TotalModelMatrix
        {
            get
            {
                return SceneNode.TotalModelMatrix;
            }
        }


        public bool Visible
        { 
            get { return SceneNode.Visible; }
        }

        #region Scene Node
        private readonly object sceneNodeLock = new object();
        private SceneNode sceneNode;
        public SceneNode SceneNode
        {
            get
            {
                if (sceneNode == null)
                {
                    lock (sceneNodeLock)
                    {
                        if (sceneNode == null)
                        {
                            sceneNode = OnCreateSceneNode();
                            AssignDefaultValuesToSceneNode(sceneNode);
                            sceneNode.HitTestSource = this;
                        }
                    }
                }
                return sceneNode;
            }
        }
        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected abstract SceneNode OnCreateSceneNode();

        protected virtual void AssignDefaultValuesToSceneNode(SceneNode node) { }
        #endregion

        #region Hit Test        
        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        public virtual bool HitTest(IRenderContext context, Ray ray, ref List<HitTestResult> hits)
        {
            return SceneNode.HitTest(context, ray, ref hits);
        }
        #endregion

        #region IBoundable        
        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public BoundingBox Bounds
        {
            get { return SceneNode.Bounds; }
        }
        /// <summary>
        /// Gets the bounds with transform.
        /// </summary>
        /// <value>
        /// The bounds with transform.
        /// </value>
        public BoundingBox BoundsWithTransform
        {
            get { return SceneNode.BoundsWithTransform; }
        }
        /// <summary>
        /// Gets the bounds sphere.
        /// </summary>
        /// <value>
        /// The bounds sphere.
        /// </value>
        public BoundingSphere BoundsSphere
        {
            get { return SceneNode.BoundsSphere; }
        }
        /// <summary>
        /// Gets the bounds sphere with transform.
        /// </summary>
        /// <value>
        /// The bounds sphere with transform.
        /// </value>
        public BoundingSphere BoundsSphereWithTransform
        {
            get { return SceneNode.BoundsSphereWithTransform; }
        }
        #endregion

        public void InvalidateRender()
        {
            SceneNode.InvalidateRender();
        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private bool disablePropertyChangedEvent = false;
        /// <summary>
        /// Gets or sets a value indicating whether [disable property changed event].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable property changed event]; otherwise, <c>false</c>.
        /// </value>
        public bool DisablePropertyChangedEvent
        {
            set
            {
                if (disablePropertyChangedEvent == value)
                {
                    return;
                }
                disablePropertyChangedEvent = value;
                RaisePropertyChanged();
            }
            get
            {
                return disablePropertyChangedEvent;
            }
        }
        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!DisablePropertyChangedEvent)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Sets the specified backing field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField">The backing field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// Sets the specified backing field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField">The backing field.</param>
        /// <param name="value">The value.</param>
        /// <param name="raisePropertyChanged">if set to <c>true</c> [raise property changed].</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, bool raisePropertyChanged, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            if (raisePropertyChanged)
            { this.RaisePropertyChanged(propertyName); }
            return true;
        }
#endregion

#region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disposer.RemoveAndDispose(ref sceneNode);
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Element3DCore() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public static implicit operator SceneNode(Element3DCore core)
        {
            return core.SceneNode;
        }
    }
}
