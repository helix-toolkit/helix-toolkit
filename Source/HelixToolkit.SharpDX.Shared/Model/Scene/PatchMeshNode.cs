#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    public class PatchMeshNode : MeshNode
    {
        public PatchMeshNode()
        {
            EnableTessellation = true;
        }
    }
}
