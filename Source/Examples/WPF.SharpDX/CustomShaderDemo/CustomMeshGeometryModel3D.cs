using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System.Windows;

namespace CustomShaderDemo
{
    public class CustomMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static readonly DependencyProperty ColorGradientProperty = DependencyProperty.Register("ColorGradient", typeof(Color4Collection),
            typeof(CustomMeshGeometryModel3D),
            new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3D).SceneNode as CustomMeshNode).ColorGradients = (Color4Collection)e.NewValue;
            }));

        public Color4Collection ColorGradient
        {
            set
            {
                SetValue(ColorGradientProperty, value);
            }
            get
            {
                return (Color4Collection)GetValue(ColorGradientProperty);
            }
        }
        public static readonly DependencyProperty HeightScaleProperty = DependencyProperty.Register("HeightScale", typeof(double),
            typeof(CustomMeshGeometryModel3D),
            new PropertyMetadata(5.0, (d, e) =>
            {
                ((d as Element3D).SceneNode as CustomMeshNode).HeightScale = (float)(double)e.NewValue;
            }));

        public double HeightScale
        {
            set
            {
                SetValue(HeightScaleProperty, value);
            }
            get
            {
                return (double)GetValue(HeightScaleProperty);
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new CustomMeshNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            base.AssignDefaultValuesToSceneNode(core);
            (core as CustomMeshNode).ColorGradients = ColorGradient;
            (core as CustomMeshNode).HeightScale = (float)HeightScale;
        }
    }
}
