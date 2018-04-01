using System.Windows;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    using Model;
    using Model.Scene;

    public class OutLineMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static readonly DependencyProperty EnableOutlineProperty = DependencyProperty.Register("EnableOutline", typeof(bool), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshOutlineNode).EnableOutline = (bool)e.NewValue;
            }));

        public bool EnableOutline
        {
            set
            {
                SetValue(EnableOutlineProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableOutlineProperty);
            }
        }

        public static DependencyProperty OutlineColorProperty = DependencyProperty.Register("OutlineColor", typeof(Media.Color), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(Media.Colors.White,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshOutlineNode).OutlineColor = ((Media.Color)e.NewValue).ToColor4();
            }));

        public Media.Color OutlineColor
        {
            set
            {
                SetValue(OutlineColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(OutlineColorProperty);
            }
        }

        public static DependencyProperty IsDrawGeometryProperty = DependencyProperty.Register("IsDrawGeometry", typeof(bool), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) => {
                ((d as Element3DCore).SceneNode as MeshOutlineNode).IsDrawGeometry = (bool)e.NewValue;
            }));

        public bool IsDrawGeometry
        {
            set
            {
                SetValue(IsDrawGeometryProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDrawGeometryProperty);
            }
        }


        public static DependencyProperty OutlineFadingFactorProperty = DependencyProperty.Register("OutlineFadingFactor", typeof(double), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(1.5, (d, e) => {
                ((d as Element3DCore).SceneNode as MeshOutlineNode).OutlineFadingFactor = (float)(double)e.NewValue;
            }));

        public double OutlineFadingFactor
        {
            set
            {
                SetValue(OutlineFadingFactorProperty, value);
            }
            get
            {
                return (double)GetValue(OutlineFadingFactorProperty);
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new MeshOutlineNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            if(core is MeshOutlineNode c)
            {
                c.OutlineColor = this.OutlineColor.ToColor4();
                c.EnableOutline = this.EnableOutline;
                c.OutlineFadingFactor = (float)this.OutlineFadingFactor;
                c.IsDrawGeometry = this.IsDrawGeometry;
            }

            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}