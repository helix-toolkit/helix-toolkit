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

        protected override IRenderCore OnCreateRenderCore()
        {
            return new CustomMeshCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as CustomMeshCore).ColorGradients = ColorGradient;
        }
    }
}
