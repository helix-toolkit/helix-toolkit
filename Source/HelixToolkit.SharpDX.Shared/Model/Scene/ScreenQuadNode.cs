/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.IO;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;

    public class ScreenQuadNode : SceneNode
    {
        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Stream Texture
        {
            set { (RenderCore as DrawScreenQuadCore).Texture = value; }
            get { return (RenderCore as DrawScreenQuadCore).Texture; }
        }
        /// <summary>
        /// Gets or sets the sampler.
        /// </summary>
        /// <value>
        /// The sampler.
        /// </value>
        public SamplerStateDescription Sampler
        {
            set { (RenderCore as DrawScreenQuadCore).SamplerDescription = value; }
            get { return (RenderCore as DrawScreenQuadCore).SamplerDescription; }
        }

        private float depth = 1f;
        public float Depth
        {
            set
            {
                if(SetAffectsRender(ref depth, value))
                {
                    var core = RenderCore as DrawScreenQuadCore;
                    core.ModelStruct.TopLeft.Z = core.ModelStruct.TopRight.Z = core.ModelStruct.BottomLeft.Z = core.ModelStruct.BottomRight.Z = value;
                }
            }
            get { return depth; }
        }

        public ScreenQuadNode()
        {
            IsHitTestVisible = false;
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new DrawScreenQuadCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return EffectsManager[DefaultRenderTechniqueNames.ScreenQuad];
        }

        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
