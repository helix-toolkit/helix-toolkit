// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DemoElement3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace UIElementDemo
{
    public class DemoElement3D : UIElement3D
    {
        public DemoElement3D()
        {
            var gm = new GeometryModel3D();
            var mb = new MeshBuilder();
            mb.AddSphere(new Point3D(0, 0, 0), 2, 100, 50);
            gm.Geometry = mb.ToMesh();
            gm.Material = Materials.Blue;

            Visual3DModel = gm;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var gm = Visual3DModel as GeometryModel3D;
                gm.Material = gm.Material == Materials.Blue ? Materials.Red : Materials.Blue;
                e.Handled = true;
            }
        }
    }
}