namespace HelixToolkit.SharpDX.Wpf
{
    using System.Windows;

    public class MeshGeometryModel3D : Model3D
    {
        public MeshGeometry3D Geometry
        {
            get { return (MeshGeometry3D)this.GetValue(GeometryProperty); }
            set { this.SetValue(GeometryProperty, value); }
        }

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(MeshGeometry3D), typeof(MeshGeometryModel3D), new UIPropertyMetadata(null));

        public Material Material
        {
            get { return (Material)this.GetValue(MaterialProperty); }
            set { this.SetValue(MaterialProperty, value); }
        }

        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(MeshGeometryModel3D), new UIPropertyMetadata(null));

        public override void Attach(IRenderHost host)
        {
            base.Attach(host);
            if (Geometry != null)
                Geometry.Attach(host);
        }
        public override void Detach()
        {
            base.Detach();
            if (Geometry != null)
                Geometry.Detach();
        }

        public override void Render(RenderContext context)
        {
            base.Render(context);
            if (Geometry != null)
                Geometry.Render(context);
        }
    }
}