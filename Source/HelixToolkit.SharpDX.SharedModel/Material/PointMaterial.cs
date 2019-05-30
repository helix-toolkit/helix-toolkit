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

        public static readonly DependencyProperty EnableDistanceFadingProperty =
            DependencyProperty.Register("EnableDistanceFading", typeof(bool), typeof(PointMaterial), new PropertyMetadata(false,
                (d, e) =>
                {
                    ((d as PointMaterial).Core as PointMaterialCore).EnableDistanceFading = (bool)e.NewValue;
                }));

        public static readonly DependencyProperty FadingNearDistanceProperty =
            DependencyProperty.Register("FadingNearDistance", typeof(double), typeof(PointMaterial), new PropertyMetadata(0.0,
                (d, e) =>
                {
                    ((d as PointMaterial).Core as PointMaterialCore).FadingNearDistance = (float)(double)e.NewValue;
                }));

        public static readonly DependencyProperty FadingFarDistanceProperty =
            DependencyProperty.Register("FadingFarDistance", typeof(double), typeof(PointMaterial), new PropertyMetadata(100.0,
                (d, e) =>
                {
                    ((d as PointMaterial).Core as PointMaterialCore).FadingFarDistance = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// Gets or sets the point color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Media.Color Color
        {
            get { return (Media.Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Size Size
        {
            get { return (Size)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the figure.
        /// </summary>
        /// <value>
        /// The figure.
        /// </value>
        public PointFigure Figure
        {
            get { return (PointFigure)this.GetValue(FigureProperty); }
            set { this.SetValue(FigureProperty, value); }
        }
        /// <summary>
        /// Gets or sets the figure ratio.
        /// </summary>
        /// <value>
        /// The figure ratio.
        /// </value>
        public double FigureRatio
        {
            get { return (double)this.GetValue(FigureRatioProperty); }
            set { this.SetValue(FigureRatioProperty, value); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable distance fading].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable distance fading]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDistanceFading
        {
            set { SetValue(EnableDistanceFadingProperty, value); }
            get { return (bool)GetValue(EnableDistanceFadingProperty); }
        }
        /// <summary>
        /// Gets or sets the fading near distance.
        /// </summary>
        /// <value>
        /// The fading near distance.
        /// </value>
        public double FadingNearDistance
        {
            get { return (double)this.GetValue(FadingNearDistanceProperty); }
            set { this.SetValue(FadingNearDistanceProperty, value); }
        }
        /// <summary>
        /// Gets or sets the fading far distance.
        /// </summary>
        /// <value>
        /// The fading far distance.
        /// </value>
        public double FadingFarDistance
        {
            get { return (double)this.GetValue(FadingFarDistanceProperty); }
            set { this.SetValue(FadingFarDistanceProperty, value); }
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
                Name = Name,
                EnableDistanceFading = EnableDistanceFading,
                FadingNearDistance = (float)FadingNearDistance,
                FadingFarDistance = (float)FadingFarDistance
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
