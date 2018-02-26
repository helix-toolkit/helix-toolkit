namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// Highlight the border of meshes
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.Element3D" />
    public class PostEffectMeshBorderHighlight : PostEffectMeshOutlineBlur
    {
        public PostEffectMeshBorderHighlight()
        {
            EffectName = DefaultRenderTechniqueNames.PostEffectMeshBorderHighlight;
        }

        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="Element3DCore.OnSetRenderTechnique" /> is set, then <see cref="Element3DCore.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
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
