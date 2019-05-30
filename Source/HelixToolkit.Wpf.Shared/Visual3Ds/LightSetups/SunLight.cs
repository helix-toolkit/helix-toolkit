// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SunLight.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that contains a "sunlight" light model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that contains a "sunlight" light model.
    /// </summary>
    public class SunLight : LightSetup
    {
        /// <summary>
        /// Identifies the <see cref="Altitude"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AltitudeProperty = DependencyProperty.Register(
            "Altitude", typeof(double), typeof(SunLight), new UIPropertyMetadata(60.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="Ambient"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AmbientProperty = DependencyProperty.Register(
            "Ambient", typeof(double), typeof(SunLight), new UIPropertyMetadata(0.4, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="Azimuth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AzimuthProperty = DependencyProperty.Register(
            "Azimuth", typeof(double), typeof(SunLight), new UIPropertyMetadata(130.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="Brightness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register(
            "Brightness", typeof(double), typeof(SunLight), new UIPropertyMetadata(0.6, SetupChanged));

        /// <summary>
        /// The altitude axis.
        /// </summary>
        private readonly Vector3D altitudeAxis = new Vector3D(0, 1, 0);

        /// <summary>
        /// The azimuth axis.
        /// </summary>
        private readonly Vector3D azimuthAxis = new Vector3D(0, 0, 1);

        /// <summary>
        /// Gets or sets the altitude angle (degrees).
        /// </summary>
        /// <value>The altitude.</value>
        public double Altitude
        {
            get
            {
                return (double)this.GetValue(AltitudeProperty);
            }

            set
            {
                this.SetValue(AltitudeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ambient lightness.
        /// </summary>
        /// <value>The ambient.</value>
        public double Ambient
        {
            get
            {
                return (double)this.GetValue(AmbientProperty);
            }

            set
            {
                this.SetValue(AmbientProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the azimuth angle (degrees).
        /// </summary>
        /// <value>The azimuth.</value>
        public double Azimuth
        {
            get
            {
                return (double)this.GetValue(AzimuthProperty);
            }

            set
            {
                this.SetValue(AzimuthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brightness.
        /// </summary>
        /// <value>The brightness.</value>
        public double Brightness
        {
            get
            {
                return (double)this.GetValue(BrightnessProperty);
            }

            set
            {
                this.SetValue(BrightnessProperty, value);
            }
        }

        /// <summary>
        /// Adds the lights to the element.
        /// </summary>
        /// <param name="lightGroup">
        /// The light group.
        /// </param>
        protected override void AddLights(Model3DGroup lightGroup)
        {
            var t1 = new RotateTransform3D(new AxisAngleRotation3D(this.azimuthAxis, this.Azimuth));
            var t2 = new RotateTransform3D(new AxisAngleRotation3D(this.altitudeAxis, this.Altitude));
            var dir = t1.Transform(t2.Transform(new Vector3D(1, 0, 0)));

            var i = (byte)(255 * this.Brightness);
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(i, i, i), dir));

            var ai = (byte)(255 * this.Ambient);
            lightGroup.Children.Add(new AmbientLight(Color.FromRgb(ai, ai, ai)));
        }
    }
}