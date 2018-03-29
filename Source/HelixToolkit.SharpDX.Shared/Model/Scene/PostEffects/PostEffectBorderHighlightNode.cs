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
    public class PostEffectBorderHighlightNode : PostEffectMeshOutlineBlurNode
    {
        public PostEffectBorderHighlightNode()
        {
            EffectName = DefaultRenderTechniqueNames.PostEffectMeshBorderHighlight;
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
            return host.EffectsManager[DefaultRenderTechniqueNames.PostEffectMeshBorderHighlight];
        }
    }
}
