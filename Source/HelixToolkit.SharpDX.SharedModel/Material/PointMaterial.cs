/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using Windows.Foundation;
using Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;
using Media = Windows.UI;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    public class PointMaterial : Material
    {
        #region Dependency Properties
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(PointMaterial),
                new PropertyMetadata(Media.Colors.Black, (d, e) =>
                {
                    ((d as PointMaterial).Core as PointMaterialCore).PointColor = ((Media.Color)e.NewValue).ToColor4();
                }));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(PointMaterial), new PropertyMetadata(new Size(1.0, 1.0),
                (d, e) =>
                {
                    var size = (Size)e.NewValue;
                    ((d as PointMaterial).Core as PointMaterialCore).Width = (float)size.Width;
                    ((d as PointMaterial).Core as PointMaterialCore).Height = (float)size.Height;
                }));

        public static readonly DependencyProperty FigureProperty =
            DependencyProperty.Register("Figure", typeof(PointFigure), typeof(PointMaterial), new PropertyMetadata(PointFigure.Rect,
                (d, e) =>
                {
                    ((d as PointMaterial).Core as PointMaterialCore).Figure = (PointFigure)e.NewValue;
                }));

        public static readonly DependencyProperty FigureRatioProperty =
            DependencyProperty.Register("FigureRatio", typeof(double), typeof(PointMaterial), new PropertyMetadata(0.25,
                (d, e) =>
                {
                    ((d as PointMaterial).Core as PointMaterialCore).FigureRatio = (float)(double)e.NewValue;
                }));

        public Media.Color Color
        {
            get { return (Media.Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public Size Size
        {
            get { return (Size)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        public PointFigure Figure
        {
            get { return (PointFigure)this.GetValue(FigureProperty); }
            set { this.SetValue(FigureProperty, value); }
        }

        public double FigureRatio
        {
            get { return (double)this.GetValue(FigureRatioProperty); }
            set { this.SetValue(FigureRatioProperty, value); }
        }
        #endregion

        protected override MaterialCore OnCreateCore()
        {
            return new PointMaterialCore()
            {
                PointColor = Color.ToColor4(),
                Width = (float)Size.Width,
                Height = (float)Size.Height,
                Figure = Figure,
                FigureRatio = (float)FigureRatio,
                Name = Name
            };
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return new PointMaterial()
            {
                Name = Name
            };
        }
#endif
    }
}
