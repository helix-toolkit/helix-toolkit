/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.IO;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;

    public class EnvironmentMapNode : SceneNode
    {
        public Stream Texture
        {
            set
            {
                (RenderCore as ISkyboxRenderParams).CubeTexture = value;
            }
            get
            {
                return (RenderCore as ISkyboxRenderParams).CubeTexture;
            }
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new SkyBoxRenderCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.Skybox];
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
