using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Media = System.Windows.Media;

namespace CustomShaderDemo.Materials
{
    public class CustomPointMaterial : PointMaterial
    {
         protected override MaterialCore OnCreateCore()
        {
            return new CustomPointMaterialCore()
            {
                PointColor = Color.ToColor4(),
                Width = (float)Size.Width,
                Height = (float)Size.Height,
                Figure = Figure,
                FigureRatio = (float)FigureRatio,
                Name = Name,
                EnableDistanceFading = EnableDistanceFading,
                FadingNearDistance = (float)FadingNearDistance,
                FadingFarDistance = (float)FadingFarDistance
            };
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return new CustomPointMaterial()
            {
                Name = Name
            };
        }
#endif
    }
}
