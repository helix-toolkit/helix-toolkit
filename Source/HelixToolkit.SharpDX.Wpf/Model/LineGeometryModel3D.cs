namespace HelixToolkit.SharpDX.Wpf
{
    using System.Windows;

    using Color = global::SharpDX.Color;

    public class LineGeometryModel3D : Model3D
    {
        public LineGeometry3D Geometry
        {
            get
            {
                return (LineGeometry3D)this.GetValue(GeometryProperty);
            }
            set
            {
                this.SetValue(GeometryProperty, value);
            }
        }

        public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register(
            "Geometry", typeof(LineGeometry3D), typeof(LineGeometryModel3D), new UIPropertyMetadata(null));

        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(LineGeometryModel3D), new UIPropertyMetadata(Color.Red));

        public double Thickness
        {
            get { return (double)this.GetValue(ThicknessProperty); }
            set { this.SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LineGeometryModel3D), new UIPropertyMetadata(0.0));

        
    }
}