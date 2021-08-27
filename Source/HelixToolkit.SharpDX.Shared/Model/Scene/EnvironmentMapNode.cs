/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.IO;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
    {
        using Core;
        /// <summary>
        /// 
        /// </summary>
        public class EnvironmentMapNode : SceneNode
        {
            /// <summary>
            /// Gets or sets the environment texture. Must be 3D cube texture
            /// </summary>
            /// <value>
            /// The texture.
            /// </value>
            public TextureModel Texture
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

            private readonly bool UseSkyDome = false;
            /// <summary>
            /// Initializes a new instance of the <see cref="EnvironmentMapNode"/> class. Default is using SkyBox. To use SkyDome, pass true into the constructor
            /// </summary>
            public EnvironmentMapNode()
            {
                RenderOrder = 1000;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="EnvironmentMapNode"/> class. Default is using SkyBox. To use SkyDome, pass true into the constructor
            /// </summary>
            /// <param name="useSkyDome">if set to <c>true</c> [use sky dome].</param>
            public EnvironmentMapNode(bool useSkyDome)
            {
                UseSkyDome = useSkyDome;
                RenderOrder = 1000;
            }
            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                if (UseSkyDome)
                {
                    return new SkyDomeRenderCore();
                }
                else
                {
                    return new SkyBoxRenderCore();
                }
            }
            /// <summary>
            /// Called when [create render technique].
            /// </summary>
            /// <param name="host">The host.</param>
            /// <returns></returns>
            protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
            {
                return host.EffectsManager[DefaultRenderTechniqueNames.Skybox];
            }

            public sealed override bool HitTest(HitTestContext context, ref List<HitTestResult> hits)
            {
                return false;
            }

            protected sealed override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
            {
                return false;
            }

            protected override bool CanRender(RenderContext context)
            {
                if (!base.CanRender(context))
                {
                    context.SharedResource.EnvironementMap = null;
                    return false;
                }
                return true;
            }
        }
    }

}
