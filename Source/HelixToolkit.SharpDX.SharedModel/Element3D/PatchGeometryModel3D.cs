// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatchGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model.Scene;

    public class PatchGeometryModel3D : MeshGeometryModel3D
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new PatchMeshNode();
        }
    }
}