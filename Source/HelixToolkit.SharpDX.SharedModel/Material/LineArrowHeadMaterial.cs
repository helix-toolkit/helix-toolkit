/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using  Windows.UI.Xaml;
using Media = Windows.UI;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Media = Windows.UI;
using HelixToolkit.SharpDX.Core.Model;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using Media = System.Windows.Media;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    public class LineArrowHeadMaterial : LineMaterial
    {
        public double ArrowSize
        {
            get
            {
                return (double)GetValue(ArrowSizeProperty);
            }
            set
            {
                SetValue(ArrowSizeProperty, value);
            }
        }

        public static readonly DependencyProperty ArrowSizeProperty =
            DependencyProperty.Register("ArrowSize", typeof(double), typeof(LineArrowHeadMaterial), new PropertyMetadata(0.1, (d, e) =>
            {
                ((d as LineMaterial).Core as LineArrowHeadMaterialCore).ArrowSize = (float)(double)e.NewValue;
            }));


        protected override MaterialCore OnCreateCore()
        {
            return new LineArrowHeadMaterialCore()
            {
                Name = Name,
                LineColor = Color.ToColor4(),
                Smoothness = (float)Smoothness,
                Thickness = (float)Thickness,
                EnableDistanceFading = EnableDistanceFading,
                FadingNearDistance = (float)FadingNearDistance,
                FadingFarDistance = (float)FadingFarDistance,
                Texture = Texture,
                TextureScale = (float)TextureScale,
                SamplerDescription = SamplerDescription,
                ArrowSize = (float)ArrowSize,
                FixedSize = FixedSize
            };
        }
    }

    public class LineArrowHeadTailMaterial : LineArrowHeadMaterial
    {
        protected override MaterialCore OnCreateCore()
        {
            return new LineArrowHeadTailMaterialCore()
            {
                Name = Name,
                LineColor = Color.ToColor4(),
                Smoothness = (float)Smoothness,
                Thickness = (float)Thickness,
                EnableDistanceFading = EnableDistanceFading,
                FadingNearDistance = (float)FadingNearDistance,
                FadingFarDistance = (float)FadingFarDistance,
                Texture = Texture,
                TextureScale = (float)TextureScale,
                SamplerDescription = SamplerDescription,
                ArrowSize = (float)ArrowSize,
                FixedSize = FixedSize
            };
        }
    }
}