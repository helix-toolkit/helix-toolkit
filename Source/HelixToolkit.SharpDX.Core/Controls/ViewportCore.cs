/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using global::SharpDX;

namespace HelixToolkit.SharpDX.Core.Controls
{   
    using Cameras;
    using Model.Scene;
    using Model.Scene2D;
    using Render;


    public sealed class ViewportCore : IViewport3DX
    {
        /// <summary>
        /// Occurs when [on start rendering].
        /// </summary>
        public event EventHandler StartRendering;
        /// <summary>
        /// Occurs when [on stop rendering].
        /// </summary>
        public event EventHandler StopRendering;
        /// <summary>
        /// Occurs when [on error occurred].
        /// </summary>
        public event EventHandler<Exception> ErrorOccurred;
        #region Properties
        /// <summary>
        /// Gets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        public IRenderHost RenderHost { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is shadow mapping enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is shadow mapping enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsShadowMappingEnabled { set; get; }

        private IEffectsManager effectsManager;
        /// <summary>
        /// Gets or sets the effects manager.
        /// </summary>
        /// <value>
        /// The effects manager.
        /// </value>
        public IEffectsManager EffectsManager
        {
            get { return effectsManager; }
            set
            {
                if (effectsManager != value)
                {
                    effectsManager = value;
                    if (RenderHost != null)
                    {
                        RenderHost.EffectsManager = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the camera core.
        /// </summary>
        /// <value>
        /// The camera core.
        /// </value>
        public CameraCore CameraCore
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the renderables.
        /// </summary>
        /// <value>
        /// The renderables.
        /// </value>
        public IEnumerable<SceneNode> Renderables => Items.ItemsInternal;
        /// <summary>
        /// Gets the d2 d renderables.
        /// </summary>
        /// <value>
        /// The d2 d renderables.
        /// </value>
        public IEnumerable<SceneNode2D> D2DRenderables => Items2D.ItemsInternal;
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public GroupNode Items { get; } = new GroupNode();
        /// <summary>
        /// Gets the items2 d.
        /// </summary>
        /// <value>
        /// The items2 d.
        /// </value>
        public SceneNode2D Items2D { get; } = new OverlayNode2D() { EnableBitmapCache = false };
        /// <summary>
        /// Gets or sets a value indicating whether [show FPS].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show FPS]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowFPS
        {
            set
            {
                if (value)
                {
                    RenderHost.ShowRenderDetail |= RenderDetail.FPS;
                }
                else
                {
                    RenderHost.ShowRenderDetail &= ~RenderDetail.FPS;
                }
            }
            get
            {
                return (RenderHost.ShowRenderDetail & ~RenderDetail.FPS) != 0;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show render detail].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show render detail]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowRenderDetail
        {
            set
            {
                if (value)
                {
                    RenderHost.ShowRenderDetail |= RenderDetail.Statistics;
                }
                else
                {
                    RenderHost.ShowRenderDetail &= ~RenderDetail.Statistics;
                }
            }
            get
            {
                return (RenderHost.ShowRenderDetail & ~RenderDetail.Statistics) != 0;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [render d2d].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render d2d]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderD2D
        {
            set => RenderHost.RenderConfiguration.RenderD2D = value;
            get => RenderHost.RenderConfiguration.RenderD2D;
        }
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public Color4 BackgroundColor
        {
            set => RenderHost.ClearColor = value;
            get => RenderHost.ClearColor; 
        }
        /// <summary>
        /// Gets or sets the FXAA level.
        /// </summary>
        /// <value>
        /// The FXAA level.
        /// </value>
        public FXAALevel FXAALevel
        {
            set => RenderHost.RenderConfiguration.FXAALevel = value;
            get => RenderHost.RenderConfiguration.FXAALevel;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable render frustum].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable render frustum]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRenderFrustum
        {
            set => RenderHost.EnableRenderFrustum = value;
            get => RenderHost.EnableRenderFrustum;
        }
        /// <summary>
        /// Gets the viewport rectangle.
        /// </summary>
        /// <value>
        /// The viewport rectangle.
        /// </value>
        public Rectangle ViewportRectangle { get { return new Rectangle(0, 0, (int)RenderHost.ActualWidth, (int)RenderHost.ActualHeight); } }
        /// <summary>
        /// Gets the render context.
        /// </summary>
        /// <value>
        /// The render context.
        /// </value>
        public RenderContext RenderContext { get => RenderHost.RenderContext; }
        /// <summary>
        /// Gets the render stat.
        /// </summary>
        /// <value>
        /// The render stat.
        /// </value>
        public Utilities.IRenderStatistics RenderStat { get => RenderHost.RenderStatistics; }
        /// <summary>
        /// Gets or sets a value indicating whether [enable vertical synchronize].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable vertical synchronize]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableVSync
        {
            set => RenderHost.RenderConfiguration.EnableVSync = value;
            get => RenderHost.RenderConfiguration.EnableVSync;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable ssao].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable ssao]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSSAO
        {
            set => RenderHost.RenderConfiguration.EnableSSAO = value;
            get => RenderHost.RenderConfiguration.EnableSSAO;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable render order].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable render order]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRenderOrder
        {
            set => RenderHost.RenderConfiguration.EnableRenderOrder = value;
            get => RenderHost.RenderConfiguration.EnableRenderOrder;
        }
        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        public double ActualWidth { private set; get; }
        /// <summary>
        /// Gets the actual height.
        /// </summary>
        /// <value>
        /// The actual height.
        /// </value>
        public double ActualHeight { private set; get; }
        #endregion

        private List<HitTestResult> hits = new List<HitTestResult>();

        private SceneNode currentNode;
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
                    ShowRenderDetail = RenderDetail.Statistics | RenderDetail.FPS,
                    Viewport = this,
                };
            }
            else
            {
                RenderHost = new SwapChainRenderHost(nativeWindowPointer)
                {
                    ShowRenderDetail = RenderDetail.Statistics | RenderDetail.FPS,
                    Viewport = this,
                };
            }
            BackgroundColor = Color.Black;
            RenderHost.StartRenderLoop += RenderHost_StartRenderLoop;
            RenderHost.StopRenderLoop += RenderHost_StopRenderLoop;
            RenderHost.ExceptionOccurred += (s, e) => { HandleExceptionOccured(e.Exception); };
            Items2D.ItemsInternal.Add(new FrameStatisticsNode2D());
        }

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

        public void Render()
        {
            RenderHost.UpdateAndRender();
        }

        public void StartD3D(int width, int height)
        {
            RenderHost.StartD3D(width, height);
        }

        public void EndD3D()
        {
            RenderHost.EndD3D();
        }

        public void Attach(IRenderHost host)
        {
            Items.Attach(host);
            Items2D.Attach(host);
        }

        public void Detach()
        {
            Items.Detach();
            Items2D.Detach();
        }

        public void InvalidateRender()
        {
            RenderHost.InvalidateRender();
        }

        public void InvalidateSceneGraph()
        {
            RenderHost.InvalidateSceneGraph();
        }

        public void MouseDown(Vector2 position)
        {
            hits.Clear();
            if(this.FindHitsInFrustum(position, ref hits) && hits.Count > 0 && hits[0].ModelHit is SceneNode node)
            {
                currentNode = node;
                currentNode.RaiseMouseDownEvent(this, position, hits[0]);
            }
            else
            {
                currentNode = null;
            }
        }

        public void MouseMove(Vector2 position)
        {
            if(currentNode != null && hits.Count > 0)
            {
                currentNode.RaiseMouseMoveEvent(this, position, hits[0]);
            }
        }

        public void MouseUp(Vector2 position)
        {
            if(currentNode != null && hits.Count > 0)
            {
                currentNode.RaiseMouseUpEvent(this, position, hits[0]);
            }
            hits.Clear();
            currentNode = null;
        }

        public void Update(TimeSpan timeStamp)
        {
            
        }

        public void Resize(int width, int height)
        {
            ActualWidth = width;
            ActualHeight = height;
            RenderHost.Resize(width, height);
        }
    }
}
