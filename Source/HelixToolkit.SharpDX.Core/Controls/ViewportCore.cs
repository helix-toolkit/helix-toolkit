/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
using System;

namespace HelixToolkit.SharpDX.Core.Controls
{
    using Cameras;
    using Model.Scene;
    using Render;


    public partial class ViewportCore : DisposeObject, IViewport3DX
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewportCore"/> class.
        /// </summary>
        /// <param name="nativeWindowPointer">The native window pointer.</param>
        /// <param name="deferred">if set to <c>true</c> [deferred].</param>
        public ViewportCore(IntPtr nativeWindowPointer, bool deferred = false)
        {
            if (deferred)
            {
                RenderHost = new SwapChainRenderHost(nativeWindowPointer,
                    (device) => { return new DeferredContextRenderer(device, new AutoRenderTaskScheduler()); })
                {
                    Viewport = this,
                };
            }
            else
            {
                RenderHost = new SwapChainRenderHost(nativeWindowPointer)
                {
                    Viewport = this,
                };
            }
            BackgroundColor = Color.Black;
            RenderHost.StartRenderLoop += RenderHost_StartRenderLoop;
            RenderHost.StopRenderLoop += RenderHost_StopRenderLoop;
            RenderHost.ExceptionOccurred += (s, e) => { HandleExceptionOccured(e.Exception); };
            Items2D.ItemsInternal.Add(frameStatisticsNode);
        }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        public void Render()
        {
            RenderHost.UpdateAndRender();
        }
        /// <summary>
        /// Starts the d3 d.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void StartD3D(int width, int height)
        {
            RenderHost.StartD3D(width, height);
        }
        /// <summary>
        /// Ends the d3 d.
        /// </summary>
        public void EndD3D()
        {
            RenderHost.EndD3D();
        }
        /// <summary>
        /// Attaches the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void Attach(IRenderHost host)
        {
            Items.Attach(host);
            ViewCube.Attach(host);
            CoordinateSystem.Attach(host);
            Items2D.Attach(host);
        }
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            Items.Detach();
            ViewCube.Detach();
            CoordinateSystem.Detach();
            Items2D.Detach();
        }
        /// <summary>
        /// Invalidates the render.
        /// </summary>
        public void InvalidateRender()
        {
            RenderHost.InvalidateRender();
        }
        /// <summary>
        /// Invalidates the scene graph.
        /// </summary>
        public void InvalidateSceneGraph()
        {
            RenderHost.InvalidateSceneGraph();
        }
        /// <summary>
        /// Mouses down.
        /// </summary>
        /// <param name="position">The position.</param>
        public void MouseDown(Vector2 position)
        {
            currentNode = null;
            hits.Clear();
            if(!this.UnProject(position, out var ray))
            {
                return;
            }
            else if(ViewCubeHitTest(ref ray, ref position))
            {
                return;
            }
            else if(this.FindHits(position, ref hits) && hits.Count > 0 && hits[0].ModelHit is SceneNode node)
            {
                currentNode = node;
                currentNode.RaiseMouseDownEvent(this, position, hits[0]);
                NodeHitOnMouseDown?.Invoke(this, new SceneNodeMouseDownArgs(this, position, currentNode, hits[0]));
            }
        }
        /// <summary>
        /// Mouses the move.
        /// </summary>
        /// <param name="position">The position.</param>
        public void MouseMove(Vector2 position)
        {
            if(currentNode != null && hits.Count > 0)
            {
                currentNode.RaiseMouseMoveEvent(this, position, hits[0]);
                NodeHitOnMouseMove?.Invoke(this, new SceneNodeMouseMoveArgs(this, position, currentNode, hits[0]));
            }
        }
        /// <summary>
        /// Mouses up.
        /// </summary>
        /// <param name="position">The position.</param>
        public void MouseUp(Vector2 position)
        {
            if(currentNode != null && hits.Count > 0)
            {
                currentNode.RaiseMouseUpEvent(this, position, hits[0]);
                NodeHitOnMouseUp?.Invoke(this, new SceneNodeMouseUpArgs(this, position, currentNode, hits[0]));
            }
            hits.Clear();
            currentNode = null;
        }
        /// <summary>
        /// </summary>
        /// <param name="timeStamp"></param>
        public void Update(TimeSpan timeStamp)
        {
            
        }
        /// <summary>
        /// Resizes the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Resize(int width, int height)
        {
            ActualWidth = width;
            ActualHeight = height;
            RenderHost.Resize(width, height);
        }
        #region Private Methods

        private void HandleExceptionOccured(Exception exception)
        {
            ErrorOccurred?.Invoke(this, exception);
        }

        private void RenderHost_StopRenderLoop(object sender, EventArgs e)
        {
            StopRendering?.Invoke(this, EventArgs.Empty);
        }

        private void RenderHost_StartRenderLoop(object sender, EventArgs e)
        {
            StartRendering?.Invoke(this, EventArgs.Empty);
        }

        private bool ViewCubeHitTest(ref Ray ray, ref Vector2 position)
        {
            if (ViewCube.HitTest(RenderContext, ray, ref hits))
            {
                ViewCube.RaiseMouseDownEvent(this, position, hits[0]);
                var normal = hits[0].NormalAtHit;
                if (Vector3.Cross(normal, ModelUpDirection).LengthSquared() < 1e-5)
                {
                    var vecLeft = new Vector3(-normal.Y, -normal.Z, -normal.X);
                    ViewCubeClicked(hits[0].NormalAtHit, vecLeft);
                }
                else
                {
                    ViewCubeClicked(hits[0].NormalAtHit, ModelUpDirection);
                }
                return true;
            }
            return false;
        }

        private void ViewCubeClicked(Vector3 lookDirection, Vector3 upDirection)
        {
            var target = CameraCore.Position + CameraCore.LookDirection;
            float distance = CameraCore.LookDirection.Length();
            lookDirection *= distance;
            var newPosition = target - lookDirection;
            CameraCore.AnimateTo(newPosition, lookDirection, upDirection, 500);
        }
        #endregion

        protected override void OnDispose(bool disposeManagedResources)
        {
            Detach();
            base.OnDispose(disposeManagedResources);
        }
    }
}
