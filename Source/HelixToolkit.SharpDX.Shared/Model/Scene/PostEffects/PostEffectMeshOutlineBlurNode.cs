/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    public class PostEffectMeshOutlineBlurNode : SceneNode
    {
        public string EffectName
        {
            set
            {
                (RenderCore as IPostEffectOutlineBlur).EffectName = value;
            }
            get
            {
                return (RenderCore as IPostEffectOutlineBlur).EffectName;
            }
        }

        public Color4 Color
        {
            set
            {
                (RenderCore as IPostEffectOutlineBlur).Color = value;
            }
            get
            {
                return (RenderCore as IPostEffectOutlineBlur).Color;
            }
        }

        public float ScaleX
        {
            set
            {
                (RenderCore as IPostEffectOutlineBlur).ScaleX = value;
            }
            get
            {
                return (RenderCore as IPostEffectOutlineBlur).ScaleX;
            }
        }

        public float ScaleY
        {
            set
            {
                (RenderCore as IPostEffectOutlineBlur).ScaleY = value;
            }
            get
            {
                return (RenderCore as IPostEffectOutlineBlur).ScaleY;
            }
        }

        public int NumberOfBlurPass
        {
            set
            {
                (RenderCore as IPostEffectOutlineBlur).NumberOfBlurPass = value;
            }
            get
            {
                return (RenderCore as IPostEffectOutlineBlur).NumberOfBlurPass;
            }
        }

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new PostEffectMeshOutlineBlurCore();
        }

        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="SceneNode.OnSetRenderTechnique" /> is set, then <see cref="SceneNode.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.PostEffectMeshOutlineBlur];
        }

        /// <summary>
        /// Determines whether this instance [can hit test] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanHitTest(IRenderContext context)
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
        /// <exception cref="NotImplementedException"></exception>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
