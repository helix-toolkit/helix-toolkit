/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

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
            get { return (RenderCore as PostEffectMeshOutlineBlurCore).DrawMode; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NodePostEffectBorderHighlight"/> class.
        /// </summary>
        public NodePostEffectBorderHighlight()
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
