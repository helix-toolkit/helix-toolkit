using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SharpDX;
using SharpDX.Direct3D11;
using System.Threading;

namespace HelixToolkit.Wpf.SharpDX
{
    using Model;
    using Utilities;
    using Core2D;
    /// <summary>
    /// Use to contain shared models for multiple viewports. 
    /// <para>Suggest to bind effects manager in viewmodel. Assign effect manager from code behind may cause memory leak</para>
    /// </summary>
    public class ModelContainer3DX : ItemsControl, IModelContainer
    {
        /// <summary>
        /// The EffectsManager property. Suggest to bind effects manager in viewmodel. Assign effect manager from code behind may cause memory leak
        /// </summary>
        public static readonly DependencyProperty EffectsManagerProperty = DependencyProperty.Register(
            "EffectsManager", typeof(IEffectsManager), typeof(ModelContainer3DX), new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.AffectsRender,
                (s, e) => ((ModelContainer3DX)s).EffectsManagerPropertyChanged()));
        /// <summary>
        /// The Render Technique property
        /// </summary>
        public static readonly DependencyProperty RenderTechniqueProperty = DependencyProperty.Register(
            "RenderTechnique", typeof(IRenderTechnique), typeof(ModelContainer3DX), new PropertyMetadata(null,
                (s, e) => ((ModelContainer3DX)s).RenderTechniquePropertyChanged()));


        /// <summary>
        /// Gets or sets the <see cref="EffectsManagerProperty"/>.
        /// <para>The EffectsManager property. Suggest bind effects manager in viewmodel. Assign effect manager from code behind may cause memory leak.</para>
        /// </summary>
        public IEffectsManager EffectsManager
        {
            get { return (IEffectsManager)GetValue(EffectsManagerProperty); }
            set { SetValue(EffectsManagerProperty, value); }
        }

        /// <summary>
        /// Gets or sets value for the shading model shading is used
        /// </summary>
        /// <value>
        /// <c>true</c> if deferred shading is enabled; otherwise, <c>false</c>.
        /// </value>
        public IRenderTechnique RenderTechnique
        {
            get { return (IRenderTechnique)this.GetValue(RenderTechniqueProperty); }
            set { this.SetValue(RenderTechniqueProperty, value); }
        }

        private readonly IList<IViewport3DX> viewports = new List<IViewport3DX>();
#pragma warning disable 0067
        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;
        public event EventHandler<Texture2D> OnNewRenderTargetTexture;
        public event EventHandler<bool> StartRenderLoop;
        public event EventHandler<bool> StopRenderLoop;
#pragma warning restore 0067

        public Guid GUID { get; } = Guid.NewGuid();

        public bool IsRendering { set; get; } = true;

        private int d3dCounter = 0;

        private IRenderHost currentRenderHost = null;
        public IRenderHost CurrentRenderHost
        {
            set
            {
                if (currentRenderHost != value)
                {
                    currentRenderHost = value;
                    currentRenderHost.SetDefaultRenderTargets(false);
                }
            }
            get
            {
                return currentRenderHost;
            }
        }
       
        /// <summary>
        /// Handles the change of the effects manager.
        /// </summary>
        private void EffectsManagerPropertyChanged()
        {
            foreach(var viewport in viewports)
            {
                viewport.EffectsManager = this.EffectsManager;
            }
        }

        /// <summary>
        /// Handles the change of the render technique        
        /// </summary>
        private void RenderTechniquePropertyChanged()
        {
            foreach (var viewport in viewports)
            {
                viewport.RenderTechnique = this.RenderTechnique;
            }
        }

        public void AttachViewport3DX(IViewport3DX viewport)
        {
            viewports.Add(viewport);
            viewport.RenderTechnique = this.RenderTechnique;
            viewport.EffectsManager = this.EffectsManager;
        }

        public void DettachViewport3DX(IViewport3DX viewport)
        {
            viewports.Remove(viewport);
        }

        public void InvalidateRender()
        {
            foreach(var v in viewports)
            {
                v.InvalidateRender();
            }
        }

        public void SetDefaultRenderTargets(bool clear = true)
        {
            CurrentRenderHost.SetDefaultRenderTargets(clear);
        }

        public IEnumerable<IRenderable> Renderables
        {
            get
            {
                foreach(IRenderable item in Items)
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        public Device Device
        {
            get
            {
                return this.EffectsManager != null ? this.EffectsManager.Device : null;
            }
        }
        /// <summary>
        /// Gets the device2d.
        /// </summary>
        /// <value>
        /// The device2d.
        /// </value>
        public global::SharpDX.Direct2D1.Device Device2D
        {
            get
            {
                return this.EffectsManager != null ? this.EffectsManager.Device2D : null;
            }
        }

        /// <summary>
        /// Gets or sets the color of the clear.
        /// </summary>
        /// <value>
        /// The color of the clear.
        /// </value>
        /// <exception cref="NotImplementedException"></exception>
        public Color4 ClearColor
        {
            get
            {
                return currentRenderHost != null ? currentRenderHost.ClearColor : Color.White;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsShadowMapEnabled
        {
            get
            {
                return currentRenderHost != null ? currentRenderHost.IsShadowMapEnabled : false;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public MSAALevel MSAA
        {
            set;get;
        }

        public IViewport3DX Viewport
        {
            set;get;
        }

        public IRenderContext RenderContext
        {
            get
            {
                return CurrentRenderHost.RenderContext;
            }
        }

        public bool IsBusy
        {
            get
            {
                return CurrentRenderHost.IsBusy;
            }
        }

        public int RenderCycles
        {
            set;get;
        }

        public bool EnableRenderFrustum
        {
            set;get;
        }

        public uint MaxFPS
        {
            set;get;
        }

        public bool IsDeferredLighting
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.IsDeferredLighting : false;
            }
        }

        public IModelContainer SharedModelContainer
        {
            set { } get { return this; }
        }

        public bool EnableSharingModelMode
        {
            set { }
            get { return true; }
        }

        public RenderTargetView RenderTargetBufferView
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.RenderTargetBufferView : null;
            }
        }

        public DepthStencilView DepthStencilBufferView
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.DepthStencilBufferView : null;
            }
        }

        public ID2DTargetProxy D2DTarget
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.D2DTarget : null;
            }
        }

        public IRenderStatistics RenderStatistics { get { return CurrentRenderHost != null ? CurrentRenderHost.RenderStatistics : null; } }

        public RenderDetail ShowRenderDetail
        {
            set;get;
        }
        public DX11RenderHostConfiguration RenderConfiguration { set; get; }

        public void Attach(IRenderHost host)
        {
            if (Interlocked.Increment(ref d3dCounter) == 1 && host.EffectsManager != null)
            {
                foreach (var renderable in Renderables)
                {
                    renderable.Attach(host);
                }
            }
        }

        public void Detach()
        {
            if (Interlocked.Decrement(ref d3dCounter) == 0)
            {
                foreach (var renderable in Renderables)
                {
                    renderable.Detach();
                }
            }
            else if (d3dCounter < 0)
            {
                throw new IndexOutOfRangeException("D3DCounter is negative.");
            }
        }

        public void StartD3D(double width, double height)
        {
            
        }

        public void EndD3D()
        {

        }

        public void UpdateAndRender()
        {
            
        }

        public void Resize(double width, double height)
        {
            
        }

        public void Dispose()
        {
            Detach();
            CurrentRenderHost = null;
            viewports.Clear();
        }

        public void ClearRenderTarget(DeviceContext context, bool clearBackBuffer, bool clearDepthStencilBuffer)
        {
            if (CurrentRenderHost != null)
            {
                CurrentRenderHost.ClearRenderTarget(context, clearBackBuffer, clearDepthStencilBuffer);
            }
        }
    }
}
