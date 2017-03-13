using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DataTemplateDemo
{
    public class GenericUIElement3D : UIElement3D
    {
        protected GeometryModel3D Model { get; set; }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Color), typeof(GenericUIElement3D), new UIPropertyMetadata((s, e) => ((GenericUIElement3D)s).ColorChanged()));

        public static readonly DependencyProperty MaterialProperty = DependencyProperty.Register(
            nameof(Material), typeof(Material), typeof(GenericUIElement3D), new PropertyMetadata(null));

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public Material Material
        {
            get { return (Material)GetValue(MaterialProperty); }
            set { SetValue(MaterialProperty, value); }
        }

        public GenericUIElement3D()
        {
            Model = new GeometryModel3D();
            BindingOperations.SetBinding(Model, GeometryModel3D.MaterialProperty, new Binding(nameof(Material)) { Source = this });
            Visual3DModel = Model;
            SetGeometry();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            MessageBox.Show("OnMouseDown raised. " + Color.ToString());
        }

        private void SetGeometry()
        {
            MeshBuilder meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddBox(new Point3D(0, 0, 0), 0.5, 0.5, 0.5);
            Model.Geometry = meshBuilder.ToMesh();
        }

        private void ColorChanged()
        {
            Material = MaterialHelper.CreateMaterial(Color);
        }
    }
}
