/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

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
    public class NodePostEffectBloom : SceneNode
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the effect.
        /// </summary>
        /// <value>
        /// The name of the effect.
        /// </value>
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
        /// <summary>
        /// Gets or sets the color of the threshold.
        /// </summary>
        /// <value>
        /// The color of the threshold.
        /// </value>
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
        /// <summary>
        /// Gets or sets the number of blur pass.
        /// </summary>
        /// <value>
        /// The number of blur pass.
        /// </value>
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
        /// <summary>
        /// Gets or sets the maximum down sampling step.
        /// </summary>
        /// <value>
        /// The maximum down sampling step.
        /// </value>
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
        /// <summary>
        /// Gets or sets the bloom extract intensity.
        /// </summary>
        /// <value>
        /// The bloom extract intensity.
        /// </value>
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
        /// <summary>
        /// Gets or sets the bloom pass intensity.
        /// </summary>
        /// <value>
        /// The bloom pass intensity.
        /// </value>
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
        /// <summary>
        /// Gets or sets the bloom combine intensity.
        /// </summary>
        /// <value>
        /// The bloom combine intensity.
        /// </value>
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
        /// <summary>
        /// Gets or sets the bloom combine saturation.
        /// </summary>
        /// <value>
        /// The bloom combine saturation.
        /// </value>
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
        #endregion

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
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}