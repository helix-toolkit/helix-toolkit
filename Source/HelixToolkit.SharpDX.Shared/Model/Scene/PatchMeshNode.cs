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
    /// <summary>
    /// 
    /// </summary>
    public class PatchMeshNode : MeshNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatchMeshNode"/> class.
        /// </summary>
        public PatchMeshNode()
        {
            EnableTessellation = true;
        }
    }
}
