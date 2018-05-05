#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model.Scene;
    /// <summary>
    /// Highlight the border of meshes
    /// </summary>
    public class PostEffectMeshBorderHighlight : PostEffectMeshOutlineBlur
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new NodePostEffectMeshOutlineBlur();
        }
    }
}
