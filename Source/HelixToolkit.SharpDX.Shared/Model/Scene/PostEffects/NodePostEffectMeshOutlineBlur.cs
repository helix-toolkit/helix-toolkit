/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
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
        public class NodePostEffectMeshOutlineBlur : SceneNode
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
                    (RenderCore as IPostEffectOutlineBlur).EffectName = value;
                }
                get
                {
                    return (RenderCore as IPostEffectOutlineBlur).EffectName;
                }
            }

            /// <summary>
            /// Gets or sets the color.
            /// </summary>
            /// <value>
            /// The color.
            /// </value>
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

            /// <summary>
            /// Gets or sets the scale x.
            /// </summary>
            /// <value>
            /// The scale x.
            /// </value>
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

            /// <summary>
            /// Gets or sets the scale y.
            /// </summary>
            /// <value>
            /// The scale y.
            /// </value>
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
                    (RenderCore as IPostEffectOutlineBlur).NumberOfBlurPass = value;
                }
                get
                {
                    return (RenderCore as IPostEffectOutlineBlur).NumberOfBlurPass;
                }
            }
            #endregion

            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                return new PostEffectMeshOutlineBlurCore();
            }

            protected override IRenderTechnique OnCreateRenderTechnique(IEffectsManager effectsManager)
            {
                return effectsManager[DefaultRenderTechniqueNames.PostEffectMeshOutlineBlur];
            }

            public sealed override bool HitTest(HitTestContext context, ref List<HitTestResult> hits)
            {
                return false;
            }

            protected sealed override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }
}