/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
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
    public class PostEffectBloomNode : SceneNode
    {        
        public string EffectName
        {
            set
            {
                (RenderCore as IPostEffectBloom).EffectName = value;
            }
            get
            {
                return (RenderCore as IPostEffectBloom).EffectName;
            }
        }

        public Color4 ThresholdColor
        {
            set
            {
                (RenderCore as IPostEffectBloom).ThresholdColor = value;
            }
            get
            {
                return (RenderCore as IPostEffectBloom).ThresholdColor;
            }
        }

        public int NumberOfBlurPass
        {
            set
            {
                (RenderCore as IPostEffectBloom).NumberOfBlurPass = value;
            }
            get
            {
                return (RenderCore as IPostEffectBloom).NumberOfBlurPass;
            }
        }

        public int MaximumDownSamplingStep
        {
            set
            {
                (RenderCore as IPostEffectBloom).MaximumDownSamplingStep = value;
            }
            get
            {
                return (RenderCore as IPostEffectBloom).MaximumDownSamplingStep;
            }
        }

        public float BloomExtractIntensity
        {
            set
            {
                (RenderCore as IPostEffectBloom).BloomExtractIntensity = value;
            }
            get
            {
                return (RenderCore as IPostEffectBloom).BloomExtractIntensity;
            }
        }

        public float BloomPassIntensity
        {
            set
            {
                (RenderCore as IPostEffectBloom).BloomPassIntensity = value;
            }
            get
            {
                return (RenderCore as IPostEffectBloom).BloomPassIntensity;
            }
        }

        public float BloomCombineIntensity
        {
            set
            {
                (RenderCore as IPostEffectBloom).BloomCombineIntensity = value;
            }
            get
            {
                return (RenderCore as IPostEffectBloom).BloomCombineIntensity;
            }
        }

        public float BloomCombineSaturation
        {
            set
            {
                (RenderCore as IPostEffectBloom).BloomCombineSaturation = value;
            }
            get
            {
                return (RenderCore as IPostEffectBloom).BloomCombineSaturation;
            }
        }


        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new PostEffectBloomCore();
        }

        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="SceneNode.OnSetRenderTechnique" /> is set, then <see cref="SceneNode.OnSetRenderTechnique" /> instead of <see cref="SceneNode.OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.PostEffectBloom];
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
