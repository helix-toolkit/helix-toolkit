/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
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

            public sealed override bool HitTest(RenderContext context, Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }

            protected sealed override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }

}