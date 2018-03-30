namespace HelixToolkit.Wpf.SharpDX
{
    using Model.Scene;
    /// <summary>
    /// Highlight the border of meshes
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.Element3D" />
    public class PostEffectMeshBorderHighlight : PostEffectMeshOutlineBlur
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new NodePostEffectMeshOutlineBlur();
        }
    }
}
