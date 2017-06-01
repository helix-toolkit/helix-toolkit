using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HelixToolkit.Wpf.SharpDX.Model.Lights3D;
using HelixToolkit.Wpf.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// Use to contain shared models for multiple viewports.
    /// </summary>
    public class ModelContainer3DX : ItemsControl, IModelContainer
    {
        /// <summary>
        /// The EffectsManager property.
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
        /// Gets or sets the <see cref="IEffectsManager"/>.
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
        public RenderTechnique RenderTechnique
        {
            get { return (RenderTechnique)this.GetValue(RenderTechniqueProperty); }
            set { this.SetValue(RenderTechniqueProperty, value); }
        }

        private readonly IList<Viewport3DX> viewports = new List<Viewport3DX>();

        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;

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
            EffectsManager = new DefaultEffectsManager(new DefaultRenderTechniquesManager());
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
                return this.EffectsManager.Device;
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
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IRenderer Renderable
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
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
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
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
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public uint MaxFPS
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
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
    }
}
