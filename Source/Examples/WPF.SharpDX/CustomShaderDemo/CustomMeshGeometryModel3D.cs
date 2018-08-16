using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System.Windows;

namespace CustomShaderDemo
{
    public class CustomMeshGeometryModel3D : MeshGeometryModel3D
    {
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
            (core as CustomMeshNode).HeightScale = (float)HeightScale;
        }
    }
}
