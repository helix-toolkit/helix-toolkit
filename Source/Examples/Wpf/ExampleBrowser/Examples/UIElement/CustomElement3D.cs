using HelixToolkit;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace UIElement;

public class CustomElement3D : UIElement3D
{
    public CustomElement3D()
    {
        var gm = new GeometryModel3D();
        var mb = new MeshBuilder();
        mb.AddSphere(new Vector3(0, 0, 0), 2, 100, 50);
        gm.Geometry = mb.ToMesh().ToWndMeshGeometry3D();
        gm.Material = Materials.Blue;

        Visual3DModel = gm;
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (Visual3DModel is GeometryModel3D gm)
            {
                gm.Material = gm.Material == Materials.Blue ? Materials.Red : Materials.Blue;
            }

            e.Handled = true;
        }
    }
}
