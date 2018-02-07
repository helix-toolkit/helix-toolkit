using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Windows;
using Media = System.Windows.Media;
using System;
using HelixToolkit.Wpf.SharpDX.Core;

namespace HelixToolkit.Wpf.SharpDX
{
    public class XRayMeshGeometryModel3D : OutLineMeshGeometryModel3D
    {
        protected override IRenderCore OnCreateRenderCore()
        {
            return new MeshXRayRenderCore();
        }
    }
}
