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
    using System.Windows;
    using Core;

    public class PatchGeometryModel3D : MeshGeometryModel3D
    {


        protected override RenderCore OnCreateRenderCore()
        {
            return new PatchMeshRenderCore() { EnableTessellation = true };
        }
    }
}