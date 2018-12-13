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
        public event EventHandler OnStartRendering;
        public event EventHandler OnStopRendering;
        public event EventHandler<Exception> OnErrorOccurred;
        public IRenderHost RenderHost { get; }

        public bool IsShadowMappingEnabled { set; get; }

        private IEffectsManager effectsManager;
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

        private IRenderTechnique renderTechnique;
        public IRenderTechnique RenderTechnique
        {
            get => renderTechnique;
            set
            {
                if (renderTechnique != value)
                {
                    renderTechnique = value;
                    if (RenderHost != null)
                    {
                        RenderHost.RenderTechnique = value;
                    }
                }
            }
        }

        public CameraCore CameraCore
        {
            get;
            set;
        }

        public IEnumerable<SceneNode> Renderables => Items.ItemsInternal;

        public IEnumerable<SceneNode2D> D2DRenderables => Items2D.ItemsInternal;

        public GroupNode Items { get; } = new GroupNode();

        public SceneNode2D Items2D { get; } = new OverlayNode2D() { EnableBitmapCache = false };

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

        public bool RenderD2D
        {
            set => RenderHost.RenderConfiguration.RenderD2D = value;
            get => RenderHost.RenderConfiguration.RenderD2D;
        }

        public Color4 BackgroundColor
        {
            set => RenderHost.ClearColor = value;
            get => RenderHost.ClearColor; 
        }

        public FXAALevel FXAALevel
        {
            set => RenderHost.RenderConfiguration.FXAALevel = value;
            get => RenderHost.RenderConfiguration.FXAALevel;
        }

        public bool EnableRenderFrustum
        {
            set => RenderHost.EnableRenderFrustum = value;
            get => RenderHost.EnableRenderFrustum;
        }

        public Rectangle ViewportRectangle { get { return new Rectangle(0, 0, (int)RenderHost.ActualWidth, (int)RenderHost.ActualHeight); } }

        public RenderContext RenderContext { get => RenderHost.RenderContext; }

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


        public bool EnableSSAO
        {
            set => RenderHost.RenderConfiguration.EnableSSAO = value;
            get => RenderHost.RenderConfiguration.EnableSSAO;
        }

        public bool EnableRenderOrder
        {
            set => RenderHost.RenderConfiguration.EnableRenderOrder = value;
            get => RenderHost.RenderConfiguration.EnableRenderOrder;
        }

        public int Width { private set; get; }
        public int Height { private set; get; }

        private List<HitTestResult> hits = new List<HitTestResult>();

        private SceneNode currentNode;

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
            OnErrorOccurred?.Invoke(this, exception);
        }

        private void RenderHost_StopRenderLoop(object sender, EventArgs e)
        {
            OnStopRendering?.Invoke(this, EventArgs.Empty);
        }

        private void RenderHost_StartRenderLoop(object sender, EventArgs e)
        {
            OnStartRendering?.Invoke(this, EventArgs.Empty);
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
            if(this.FindHits(position, ref hits) && hits.Count > 0 && hits[0].ModelHit is SceneNode node)
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
            Width = width;
            Height = height;
            RenderHost.Resize(width, height);
        }
    }
}
