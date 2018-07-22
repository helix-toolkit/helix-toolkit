using System;
using System.Collections.Generic;
using HelixToolkit.Mathematics;

namespace HelixToolkit.SharpDX.Core.Controls
{   
    using UWP;
    using UWP.Cameras;
    using UWP.Model.Scene;
    using UWP.Model.Scene2D;
    using UWP.Render;


    public sealed class ViewportCore : IViewport3DX
    {
        public event EventHandler OnStartRendering;
        public event EventHandler OnStopRendering;
        public event EventHandler<Exception> OnErrorOccurred;
        public IRenderHost RenderHost { private set; get; }

        public bool IsShadowMappingEnabled { set; get; }

        private IEffectsManager effectsManager;
        public IEffectsManager EffectsManager
        {
            get { return effectsManager; }
            set
            {
                if(effectsManager != value)
                {
                    effectsManager = value;
                    if(RenderHost != null)
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
                if(renderTechnique != value)
                {
                    renderTechnique = value;
                    if(RenderHost != null)
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

        public IEnumerable<SceneNode> Renderables => Items;

        public IEnumerable<SceneNode2D> D2DRenderables => Items2D;

        public List<SceneNode> Items { get; } = new List<SceneNode>();

        public List<SceneNode2D> Items2D { get; } = new List<SceneNode2D>();

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
            set
            {
                RenderHost.RenderConfiguration.RenderD2D = value;
            }
            get
            {
                return RenderHost.RenderConfiguration.RenderD2D;
            }
        }

        public Color4 BackgroundColor
        {
            set
            {
                RenderHost.ClearColor = value;
            }
            get
            {
                return RenderHost.ClearColor;
            }
        }

        public FXAALevel FXAALevel
        {
            set
            {
                RenderHost.RenderConfiguration.FXAALevel = value;
            }
            get
            {
                return RenderHost.RenderConfiguration.FXAALevel;
            }
        }

        private SceneNode2D root2D = new OverlayNode2D() { EnableBitmapCache = false };

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
            root2D.Items.Add(new FrameStatisticsNode2D());
            Items2D.Add(root2D);
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

        public void StartD3D(double width, double height)
        {
            RenderHost.StartD3D(width, height);
        }

        public void EndD3D()
        {
            RenderHost.EndD3D();
        }

        public void Attach(IRenderHost host)
        {
            foreach(var model in Items)
            {
                model.Attach(host);
            }
            foreach(var model in Items2D)
            {
                model.Attach(host);
            }
        }

        public void Detach()
        {
            foreach (var model in Items)
            {
                model.Detach();
            }
            foreach(var model in Items2D)
            {
                model.Detach();
            }
        }

        public void InvalidateRender()
        {
            RenderHost?.InvalidateRender();
        }

        public void InvalidateSceneGraph()
        {
            RenderHost?.InvalidateSceneGraph();
        }

        public void Update(TimeSpan timeStamp)
        {
            
        }

        public void Resize(int width, int height)
        {
            RenderHost?.Resize(width, height);
        }
    }
}
