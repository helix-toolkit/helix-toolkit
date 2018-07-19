/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System.Collections.Generic;
using System.IO;
using Matrix = System.Numerics.Matrix4x4;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
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
        /// <summary>
        /// Determines whether this instance [can hit test] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanHitTest(RenderContext context)
        {
            return false;
        }
        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
