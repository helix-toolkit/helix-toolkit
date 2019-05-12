// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreePointLights.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that contains a three point light setup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that contains a three point light setup.
    /// </summary>
    /// <remarks>
    /// See http://www.3drender.com/light/3point.html
    /// </remarks>
    public class ThreePointLights : LightSetup
    {
        /// <summary>
        /// Identifies the <see cref="Distance"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register(
            "Distance", typeof(double), typeof(ThreePointLights), new UIPropertyMetadata(10.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="FillLightAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillLightAngleProperty = DependencyProperty.Register(
            "FillLightAngle", typeof(double), typeof(ThreePointLights), new UIPropertyMetadata(45.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="FillLightSideAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillLightSideAngleProperty =
            DependencyProperty.Register(
                "FillLightSideAngle",
                typeof(double),
                typeof(ThreePointLights),
                new UIPropertyMetadata(-20.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="FrontDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontDirectionProperty = DependencyProperty.Register(
            "FrontDirection",
            typeof(Vector3D),
            typeof(ThreePointLights),
            new UIPropertyMetadata(new Vector3D(0, 1, 0), SetupChanged));

        /// <summary>
        /// Identifies the <see cref="KeyLightAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyLightAngleProperty = DependencyProperty.Register(
            "KeyLightAngle", typeof(double), typeof(ThreePointLights), new UIPropertyMetadata(30.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="KeyLightBrightness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyLightBrightnessProperty =
            DependencyProperty.Register(
                "KeyLightBrightness",
                typeof(double),
                typeof(ThreePointLights),
                new UIPropertyMetadata(1.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="KeyLightSideAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyLightSideAngleProperty =
            DependencyProperty.Register(
                "KeyLightSideAngle",
                typeof(double),
                typeof(ThreePointLights),
                new UIPropertyMetadata(45.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="KeyToFillLightRatio"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyToFillLightRatioProperty =
            DependencyProperty.Register(
                "KeyToFillLightRatio",
                typeof(double),
                typeof(ThreePointLights),
                new UIPropertyMetadata(2.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="KeyToRimLightRatio"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyToRimLightRatioProperty =
            DependencyProperty.Register(
                "KeyToRimLightRatio",
                typeof(double),
                typeof(ThreePointLights),
                new UIPropertyMetadata(1.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="RimLightAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RimLightAngleProperty = DependencyProperty.Register(
            "RimLightAngle", typeof(double), typeof(ThreePointLights), new UIPropertyMetadata(20.0, SetupChanged));

        /// <summary>
        /// Identifies the <see cref="Target"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target",
            typeof(Point3D),
            typeof(ThreePointLights),
            new UIPropertyMetadata(new Point3D(0, 0, 0), SetupChanged));

        /// <summary>
        /// Identifies the <see cref="UpDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register(
            "UpDirection",
            typeof(Vector3D),
            typeof(ThreePointLights),
            new UIPropertyMetadata(new Vector3D(0, 0, 1), SetupChanged));

        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        /// <value>The distance.</value>
        public double Distance
        {
            get
            {
                return (double)this.GetValue(DistanceProperty);
            }

            set
            {
                this.SetValue(DistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the fill light angle.
        /// </summary>
        /// <value>The fill light angle.</value>
        public double FillLightAngle
        {
            get
            {
                return (double)this.GetValue(FillLightAngleProperty);
            }

            set
            {
                this.SetValue(FillLightAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the fill light side angle.
        /// </summary>
        /// <value>The fill light side angle.</value>
        public double FillLightSideAngle
        {
            get
            {
                return (double)this.GetValue(FillLightSideAngleProperty);
            }

            set
            {
                this.SetValue(FillLightSideAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the front direction.
        /// </summary>
        /// <value>The front direction.</value>
        public Vector3D FrontDirection
        {
            get
            {
                return (Vector3D)this.GetValue(FrontDirectionProperty);
            }

            set
            {
                this.SetValue(FrontDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key light angle.
        /// </summary>
        /// <value>The key light angle.</value>
        public double KeyLightAngle
        {
            get
            {
                return (double)this.GetValue(KeyLightAngleProperty);
            }

            set
            {
                this.SetValue(KeyLightAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key light brightness.
        /// </summary>
        /// <value>The key light brightness.</value>
        public double KeyLightBrightness
        {
            get
            {
                return (double)this.GetValue(KeyLightBrightnessProperty);
            }

            set
            {
                this.SetValue(KeyLightBrightnessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key light side angle.
        /// </summary>
        /// <value>The key light side angle.</value>
        public double KeyLightSideAngle
        {
            get
            {
                return (double)this.GetValue(KeyLightSideAngleProperty);
            }

            set
            {
                this.SetValue(KeyLightSideAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key to fill light ratio.
        /// </summary>
        /// <value>The key to fill light ratio.</value>
        public double KeyToFillLightRatio
        {
            get
            {
                return (double)this.GetValue(KeyToFillLightRatioProperty);
            }

            set
            {
                this.SetValue(KeyToFillLightRatioProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key to rim light ratio.
        /// </summary>
        /// <value>The key to rim light ratio.</value>
        public double KeyToRimLightRatio
        {
            get
            {
                return (double)this.GetValue(KeyToRimLightRatioProperty);
            }

            set
            {
                this.SetValue(KeyToRimLightRatioProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rim light angle.
        /// </summary>
        /// <value>The rim light angle.</value>
        public double RimLightAngle
        {
            get
            {
                return (double)this.GetValue(RimLightAngleProperty);
            }

            set
            {
                this.SetValue(RimLightAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public Point3D Target
        {
            get
            {
                return (Point3D)this.GetValue(TargetProperty);
            }

            set
            {
                this.SetValue(TargetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>Up direction.</value>
        public Vector3D UpDirection
        {
            get
            {
                return (Vector3D)this.GetValue(UpDirectionProperty);
            }

            set
            {
                this.SetValue(UpDirectionProperty, value);
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
            var up = this.UpDirection;
            var front = this.FrontDirection;
            up.Normalize();
            front.Normalize();
            var right = Vector3D.CrossProduct(this.UpDirection, this.FrontDirection);

            // http://www.3drender.com/light/3point.html

            // 1. Start in Darkness. Make sure there are no default lights, and there's no global ambience.
            // When you add your first light, there should be no other light in the scene.

            // 2. Add your Key Light. The Key Light creates the subject's main illumination, and defines the most visible
            // lighting and shadows. Your Key Light represents the dominant light source, such as the sun, a window, or
            // ceiling light - although the Key does not have to be positioned exactly at this source.
            // Create a spot light to serve as the Key. From the top view, offset the Key Light 15 to 45 degrees to the side
            // (to the left or right) of the camera. From a side view, raise the Key Light above the camera, so that it hits
            // your subject from about 15 to 45 degrees higher than the camera angle.
            // The key light is brighter than any other light illuminating the front of the subject, is the main shadow-caster
            // in your scene, and casts the darkest shadows. Specular highlights are triggered by the Key Light.
            // NOTE: Be sure to stop and do test-renders here. Your "one light" scene (with just the key light) should have a
            // nice balance and contrast between light and dark, and shading that uses all of the grays in between. Your
            // "one light" should look almost like the final rendering, except that the shadows are pitch black and it has
            // very harsh contrast - see the GIF animation at the top of this page, while it only has the Key light visible.
            var tKey1 = new RotateTransform3D(new AxisAngleRotation3D(up, this.KeyLightSideAngle));
            var tKey2 = new RotateTransform3D(new AxisAngleRotation3D(right, this.KeyLightAngle));
            var keyLightDirection = front * this.Distance;
            keyLightDirection = tKey1.Transform(keyLightDirection);
            keyLightDirection = tKey2.Transform(keyLightDirection);
            var i = (byte)(255 * this.KeyLightBrightness);
            lightGroup.Children.Add(new PointLight(Color.FromRgb(i, i, i), this.Target - keyLightDirection));

            // 3. Add your Fill Light(s). The Fill Light softens and extends the illumination provided by the key light,
            // and makes more of the subject visible. Fill Light can simulate light from the sky (other than the sun),
            // secondary light sources such as table lamps, or reflected and bounced light in your scene. With several
            // functions for Fill Lights, you may add several of them to a scene. Spot lights are the most useful, but
            // point lights may be used.
            // From the top view, a Fill Light should come from a generally opposite angle than the Key - if the Key is
            // on the left, the Fill should be on the right - but don't make all of your lighting 100% symmetrical! The
            // Fill can be raised to the subject's height, but should be lower than the Key.
            // At most, Fill Lights can be about half as bright as your Key (a Key-to-Fill ratio of 2:1). For more shadowy
            // environments, use only 1/8th the Key's brightness (a Key-to-Fill ratio of 8:1). If multiple Fills overlap,
            // their sum still shouldn't compete with the Key.
            // Shadows from a Fill Light are optional, and often skipped. To simulate reflected light, tint the Fill color
            // to match colors from the environment. Fill Lights are sometimes set to be Diffuse-only (set not to cast
            // specular highlights.)
            var tFill1 = new RotateTransform3D(new AxisAngleRotation3D(up, this.FillLightSideAngle));
            var tFill2 = new RotateTransform3D(new AxisAngleRotation3D(right, this.FillLightAngle));
            var fillLightDirection = front * this.Distance;
            fillLightDirection = tFill1.Transform(fillLightDirection);
            fillLightDirection = tFill2.Transform(fillLightDirection);
            var fi = (byte)Math.Round(i / this.KeyToFillLightRatio);
            lightGroup.Children.Add(new PointLight(Color.FromRgb(fi, fi, fi), this.Target - fillLightDirection));

            // 4. Add Rim Light. The Rim Light (also called Back Light) creates a bright line around the edge of the
            // object, to help visually separate the object from the background.
            // From the top view, add a spot light, and position it behind your subject, opposite from the camera.
            // From the right view, position the Back Light above your subject.
            // Adjust the Rim Light until it gives you a clear, bright outline that highlights the top or side edge
            // for your subject.  Rim Lights can be as bright as necessary to achieve the glints you want around the
            // hair or sides of your subject. A Rim Light usually needs to cast shadows. Often you will need to use
            // light linking to link rim lights only with the main subject being lit, so that it creates a rim of light
            // around the top or side of your subject, without affecting the background:
            var tRim2 = new RotateTransform3D(new AxisAngleRotation3D(right, -this.RimLightAngle));
            var rimLightDirection = -front * this.Distance;
            rimLightDirection = tRim2.Transform(rimLightDirection);
            var ri = (byte)Math.Round(i / this.KeyToRimLightRatio);
            lightGroup.Children.Add(new PointLight(Color.FromRgb(ri, ri, ri), this.Target - rimLightDirection));
        }

    }
}