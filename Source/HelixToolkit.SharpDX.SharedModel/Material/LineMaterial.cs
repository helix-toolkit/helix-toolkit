/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using Windows.UI.Xaml;
using Media = Windows.UI;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using global::SharpDX;
    using Model;
    public class LineMaterial : Material
    {
        #region Dependency Properties        
        /// <summary>
        /// The color property
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(LineMaterial), new PropertyMetadata(Media.Colors.Black, (d, e) =>
            {
                ((d as LineMaterial).Core as LineMaterialCore).LineColor = ((Media.Color)e.NewValue).ToColor4();
            }));
        /// <summary>
        /// The thickness property
        /// </summary>
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LineMaterial), new PropertyMetadata(1.0, (d, e) =>
            {
                ((d as LineMaterial).Core as LineMaterialCore).Thickness = (float)(double)e.NewValue;
            }));
        /// <summary>
        /// The smoothness property
        /// </summary>
        public static readonly DependencyProperty SmoothnessProperty =
            DependencyProperty.Register("Smoothness", typeof(double), typeof(LineMaterial), new PropertyMetadata(0.0,
            (d, e) =>
            {
                ((d as LineMaterial).Core as LineMaterialCore).Smoothness = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty EnableDistanceFadingProperty =
            DependencyProperty.Register("EnableDistanceFading", typeof(bool), typeof(LineMaterial), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as LineMaterial).Core as LineMaterialCore).EnableDistanceFading = (bool)e.NewValue;
                }));

        public static readonly DependencyProperty FadingNearDistanceProperty =
            DependencyProperty.Register("FadingNearDistance", typeof(double), typeof(LineMaterial), new PropertyMetadata(0.0,
                (d, e) =>
                {
                    ((d as LineMaterial).Core as LineMaterialCore).FadingNearDistance = (float)(double)e.NewValue;
                }));

        public static readonly DependencyProperty FadingFarDistanceProperty =
            DependencyProperty.Register("FadingFarDistance", typeof(double), typeof(LineMaterial), new PropertyMetadata(100.0,
                (d, e) =>
                {
                    ((d as LineMaterial).Core as LineMaterialCore).FadingFarDistance = (float)(double)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the color.
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
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public double Thickness
        {
            get { return (double)this.GetValue(ThicknessProperty); }
            set { this.SetValue(ThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the smoothness.
        /// </summary>
        /// <value>
        /// The smoothness.
        /// </value>
        public double Smoothness
        {
            get { return (double)this.GetValue(SmoothnessProperty); }
            set { this.SetValue(SmoothnessProperty, value); }
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
            return new LineMaterialCore()
            {
                Name = Name,
                LineColor = Color.ToColor4(),
                Smoothness = (float)Smoothness,
                Thickness = (float)Thickness,
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
