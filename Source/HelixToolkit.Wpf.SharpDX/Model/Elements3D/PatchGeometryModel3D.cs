// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatchGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using HelixToolkit.Wpf.SharpDX.Model.Scene;

    public class PatchGeometryModel3D : MeshGeometryModel3D
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new PatchMeshNode();
        }
    }
}