// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanvasMock.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using HelixToolkit.Wpf.SharpDX.Model.Lights3D;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.Wpf.SharpDX.Tests.Controls
{
    class CanvasMock : IRenderHost
    {
        public CanvasMock()
        {
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Phong];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            Device = EffectsManager.Device;
        }
        private int renderCycles = 1;
        public int RenderCycles
        {
            set { renderCycles = value; }
            get { return renderCycles; }
        }
        public Device Device { get; private set; }
        public Color4 ClearColor { get; private set; }
        public bool IsShadowMapEnabled { get; private set; }
        public MSAALevel MSAA { get; set; }
        public IRenderer Renderable { get; set; }
        public RenderTechnique RenderTechnique { get; private set; }
        public double ActualHeight { get; private set; }
        public double ActualWidth { get; private set; }

        public IEffectsManager EffectsManager { get; set; }

        public IRenderTechniquesManager RenderTechniquesManager { get; set; }

        public bool IsBusy
        {
            get;private set;
        }

        private readonly Light3DSceneShared light3DSceneShared = new Light3DSceneShared();
        public Light3DSceneShared Light3DSceneShared
        {
            get
            {
                return light3DSceneShared;
            }
        }
        public RenderContext RenderContext { get; }
        public bool EnableRenderFrustum
        {
            set;get;
        }

        public uint MaxFPS
        {
            set;get;
        }

        public bool EnableSharingModelMode
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

        public IModelContainer SharedModelContainer
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
                throw new NotImplementedException();
            }
        }

        public void SetDefaultRenderTargets(bool clear = true)
        {
        }

        public void SetDefaultColorTargets(DepthStencilView dsv)
        {
        }

        public event EventHandler<SharpDX.Utilities.RelayExceptionEventArgs> ExceptionOccurred;

        public void InvalidateRender()
        {
        }
    }
}
