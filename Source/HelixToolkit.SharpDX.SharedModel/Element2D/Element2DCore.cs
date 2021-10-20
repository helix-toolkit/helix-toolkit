/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if DEBUG
//#define DEBUGDRAWING
//#define DISABLEBITMAPCACHE
#endif
#if !CORE
using SharpDX;
using System;
#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene2D;
namespace HelixToolkit.WinUI.Core2D
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene2D;
#endif
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
#if !COREWPF && !WINUI
    using Model.Scene2D;
#endif
    /// <summary>
    /// External Wrapper core to be used for different platform
    /// </summary>
#if NETFX_CORE || WINUI
    public abstract partial class Element2DCore : FrameworkElement, IDisposable
#else
    public abstract partial class Element2DCore : FrameworkContentElement, IDisposable
#endif
    {
        public sealed class SceneNode2DCreatedEventArgs : EventArgs
        {
            public SceneNode2D Node
            {
                private set; get;
            }
            public SceneNode2DCreatedEventArgs(SceneNode2D node)
            {
                Node = node;
            }
        }
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid GUID
        {
            get
            {
                return SceneNode.GUID;
            }
        }


        public bool IsAttached
        {
            get
            {
                return SceneNode.IsAttached;
            }
        }

        #region Scene Node
        private readonly object sceneNodeLock = new object();
        private SceneNode2D sceneNode;
        public SceneNode2D SceneNode
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
                            sceneNode.WrapperSource = this;
                            sceneNode.Attached += SceneNode_OnAttached;
                            sceneNode.Detached += SceneNode_OnDetached;
                            sceneNode.UpdateRequested += SceneNode_OnUpdate;
                            OnSceneNodeCreated?.Invoke(this, new SceneNode2DCreatedEventArgs(sceneNode));
                        }
                    }
                }
                return sceneNode;
            }
        }

        private void SceneNode_OnUpdate(object sender, SceneNode2D.UpdateEventArgs e)
        {
            OnUpdate(e.Context);
        }

        private void SceneNode_OnDetached(object sender, EventArgs e)
        {
#if NETFX_CORE || WINUI
            if(Dispatcher != null)
            {
                if (Dispatcher.HasThreadAccess)
                {
                    OnDetached();
                }
            }

#else
            if (this.Dispatcher != null && this.Dispatcher.Thread.IsAlive)
            {
                if (this.Dispatcher.CheckAccess())
                {
                    OnDetached();
                }
                else
                {
                    Dispatcher.Invoke(() => { OnDetached(); });
                }
            }
#endif
        }

        private void SceneNode_OnAttached(object sender, EventArgs e)
        {
            OnAttached();
        }

        protected virtual void OnAttached()
        {

        }

        protected virtual void OnDetached()
        {

        }

        protected virtual void OnUpdate(RenderContext2D context)
        {

        }

        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected abstract SceneNode2D OnCreateSceneNode();

        protected virtual void AssignDefaultValuesToSceneNode(SceneNode2D node)
        {
        }
        #endregion
        #region Events        
        /// <summary>
        /// Occurs when [on scene node created]. Make sure to hook up this event at the top of constructor of class, otherwise may miss the event.
        /// </summary>
        public event EventHandler<SceneNode2DCreatedEventArgs> OnSceneNodeCreated;
        #endregion

        public virtual bool HitTest(Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            return SceneNode.HitTest(mousePoint, out hitResult);
        }

        public void InvalidateRender()
        {
            SceneNode.InvalidateRender();
        }
#if !NETFX_CORE && !WINUI
        public void InvalidateMeasure()
        {
            SceneNode.InvalidateMeasure();
        }

        public void InvalidateArrange()
        {
            SceneNode.InvalidateArrange();
        }
#else
        public new void InvalidateMeasure()
        {
            SceneNode.InvalidateMeasure();
        }

        public new void InvalidateArrange()
        {
            SceneNode.InvalidateArrange();
        }
#endif


        public static implicit operator SceneNode2D(Element2DCore e)
        {
            return e.SceneNode;
        }

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
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Element2DCore() {
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
    }
}
#endif