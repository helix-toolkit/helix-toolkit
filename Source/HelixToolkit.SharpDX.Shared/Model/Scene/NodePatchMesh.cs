#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class NodePatchMesh : NodeMesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodePatchMesh"/> class.
        /// </summary>
        public NodePatchMesh()
        {
            EnableTessellation = true;
        }
    }
}
