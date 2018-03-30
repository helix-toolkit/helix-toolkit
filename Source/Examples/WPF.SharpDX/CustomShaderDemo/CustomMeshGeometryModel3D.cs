using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CustomShaderDemo
{
    public class CustomMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static readonly DependencyProperty ColorGradientProperty = DependencyProperty.Register("ColorGradient", typeof(Color4Collection),
            typeof(CustomMeshGeometryModel3D),
            new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3D).RenderCore as CustomMeshCore).ColorGradients = (Color4Collection)e.NewValue;
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
                ((d as Element3D).RenderCore as CustomMeshCore).DataHeightScale = (float)(double)e.NewValue;
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

        protected override RenderCore OnCreateRenderCore()
        {
            return new CustomMeshCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[CustomShaderNames.DataSampling];
        }

        protected override void AssignDefaultValuesToCore(RenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as CustomMeshCore).ColorGradients = ColorGradient;
            (core as CustomMeshCore).DataHeightScale = (float)HeightScale;
        }
    }
}
