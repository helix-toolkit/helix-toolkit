/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !CORE
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#if NETFX_CORE
using  Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    namespace Model
    {
#if !COREWPF && !WINUI
        using Scene;
#endif
        /// <summary>
        /// External Wrapper core to be used for different platform
        /// </summary>
#if NETFX_CORE || WINUI
        public abstract class Element3DCore : Control, IDisposable
#else
        public abstract class Element3DCore : FrameworkContentElement, IDisposable
#endif
        {
            public sealed class SceneNodeCreatedEventArgs : EventArgs
            {
                public SceneNode Node
                {
                    private set; get;
                }
                public SceneNodeCreatedEventArgs(SceneNode node)
                {
                    Node = node;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public Guid GUID
            {
                get
                {
                    return SceneNode.GUID;
                }
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
                get
                {
                    return SceneNode.Visible;
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
                                sceneNode.WrapperSource = this;
                                OnSceneNodeCreated?.Invoke(this, new SceneNodeCreatedEventArgs(sceneNode));
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

            protected virtual void AssignDefaultValuesToSceneNode(SceneNode node)
            {
            }
            #endregion
            #region Events        
            /// <summary>
            /// Occurs when [on scene node created]. Make sure to hook up this event at the top of constructor of class, otherwise may miss the event.
            /// </summary>
            public event EventHandler<SceneNodeCreatedEventArgs> OnSceneNodeCreated;
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
                get
                {
                    return SceneNode.Bounds;
                }
            }
            /// <summary>
            /// Gets the bounds with transform.
            /// </summary>
            /// <value>
            /// The bounds with transform.
            /// </value>
            public BoundingBox BoundsWithTransform
            {
                get
                {
                    return SceneNode.BoundsWithTransform;
                }
            }
            /// <summary>
            /// Gets the bounds sphere.
            /// </summary>
            /// <value>
            /// The bounds sphere.
            /// </value>
            public BoundingSphere BoundsSphere
            {
                get
                {
                    return SceneNode.BoundsSphere;
                }
            }
            /// <summary>
            /// Gets the bounds sphere with transform.
            /// </summary>
            /// <value>
            /// The bounds sphere with transform.
            /// </value>
            public BoundingSphere BoundsSphereWithTransform
            {
                get
                {
                    return SceneNode.BoundsSphereWithTransform;
                }
            }
            #endregion

            #region Hit Test        
            /// <summary>
            /// Hits the test.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="hits">The hits.</param>
            /// <returns></returns>
            public virtual bool HitTest(HitTestContext context, ref List<HitTestResult> hits)
            {
                return SceneNode.HitTest(context, ref hits);
            }
            #endregion

            public void InvalidateRender()
            {
                SceneNode.InvalidateRender();
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

            public static explicit operator SceneNode(Element3DCore core)
            {
                return core.SceneNode;
            }
        }
    }
}
#endif