/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
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
        public class NodePostEffectBorderHighlight : NodePostEffectMeshOutlineBlur
        {
            /// <summary>
            /// Gets or sets the draw mode.
            /// </summary>
            /// <value>
            /// The draw mode.
            /// </value>
            public OutlineMode DrawMode
            {
                set
                {
                    (RenderCore as PostEffectMeshOutlineBlurCore).DrawMode = value;
                }
                get
                {
                    return (RenderCore as PostEffectMeshOutlineBlurCore).DrawMode;
                }
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="NodePostEffectBorderHighlight"/> class.
            /// </summary>
            public NodePostEffectBorderHighlight()
            {
                EffectName = DefaultRenderTechniqueNames.PostEffectMeshBorderHighlight;
            }

            protected override IRenderTechnique OnCreateRenderTechnique(IEffectsManager effectsManager)
            {
                return effectsManager[DefaultRenderTechniqueNames.PostEffectMeshBorderHighlight];
            }

            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                return new PostEffectMeshOutlineBlurCore(false);
            }
        }
    }
}
