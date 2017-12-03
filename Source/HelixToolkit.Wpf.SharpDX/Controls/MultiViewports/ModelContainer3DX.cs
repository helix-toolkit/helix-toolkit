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
            "RenderTechnique", typeof(RenderTechnique), typeof(ModelContainer3DX), new PropertyMetadata(null,
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

        private readonly IList<Viewport3DX> viewports = new List<Viewport3DX>();

        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;

        public bool IsRendering { set; get; } = true;

        private int d3dCounter = 0;

        private IRenderHost currentRenderHost = null;
        public IRenderHost CurrentRenderHost
        {
            set
            {
                if (currentRenderHost != value)
                {
                    if (currentRenderHost != null)
                    { currentRenderHost.ExceptionOccurred -= ExceptionOccurred; }
                    currentRenderHost = value;
                    currentRenderHost.ExceptionOccurred += ExceptionOccurred;
                    currentRenderHost.SetDefaultRenderTargets(false);
                }
            }
            get
            {
                return currentRenderHost;
            }
        }

        public IRenderTechniquesManager RenderTechniquesManager { get { return EffectsManager != null ? EffectsManager.RenderTechniquesManager : null; } }

        public ModelContainer3DX()
        {
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

        public void AttachViewport3DX(Viewport3DX viewport)
        {
            viewports.Add(viewport);
            viewport.RenderTechnique = this.RenderTechnique;
            viewport.EffectsManager = this.EffectsManager;
        }

        public void DettachViewport3DX(Viewport3DX viewport)
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

        public void SetDefaultColorTargets(DepthStencilView dsv)
        {
            CurrentRenderHost.SetDefaultColorTargets(dsv);
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

        public Device Device
        {
            get
            {
                return this.EffectsManager != null ? this.EffectsManager.Device : null;
            }
        }

        public Color4 ClearColor
        {
            get
            {
                return CurrentRenderHost.ClearColor;
            }
        }

        public bool IsShadowMapEnabled
        {
            get
            {                
                return CurrentRenderHost != null ? CurrentRenderHost.IsShadowMapEnabled : false;
            }
        }

        public MSAALevel MSAA
        {
            set;get;
        }

        public IRenderer Renderable
        {
            set;get;
        }

        public RenderContext RenderContext
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

        public Light3DSceneShared Light3DSceneShared
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.Light3DSceneShared : null;
            }
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

        public RenderTargetView ColorBufferView
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.ColorBufferView : null;
            }
        }

        public DepthStencilView DepthStencilBufferView
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.DepthStencilBufferView : null;
            }
        }

        public D2DControlWrapper D2DControls
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.D2DControls : null;
            }
        }

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
    }
}
