// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericHeadLight.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a base class for lights that operates in camera space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides a base class for lights that operates in camera space.
    /// </summary>
    /// <typeparam name="T">The light type.</typeparam>
    public abstract class GenericHeadLight<T> : LightSetup where T : Light, new()
    {
        /// <summary>
        /// Identifies the <see cref="Brightness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(GenericHeadLight<T>), new PropertyMetadata(1d, Update));

        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(GenericHeadLight<T>), new PropertyMetadata(Colors.White, Update));

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point3D), typeof(GenericHeadLight<T>), new PropertyMetadata(new Point3D(0, 0, 3), Update));

        /// <summary>
        /// The light
        /// </summary>
        private T light;

        /// <summary>
        /// The camera
        /// </summary>
        private Camera camera;

        /// <summary>
        /// Gets or sets the brightness of the headlight. If set, this property overrides the <see cref="Color" /> property.
        /// </summary>
        /// <value>The brightness.</value>
        public double Brightness
        {
            get { return (double)this.GetValue(BrightnessProperty); }
            set { this.SetValue(BrightnessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the headlight. This property is used if <see cref="Brightness" /> is set to <c>NaN</c>.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the position of the headlight (in camera space).
        /// </summary>
        /// <value>The position.</value>
        public Point3D Position
        {
            get { return (Point3D)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Called when the parent of the 3-D visual object is changed.
        /// </summary>
        /// <param name="oldParent">A value of type <see cref="T:System.Windows.DependencyObject" /> that represents the previous parent of the <see cref="T:System.Windows.Media.Media3D.Visual3D" /> object. If the <see cref="T:System.Windows.Media.Media3D.Visual3D" /> object did not have a previous parent, the value of the parameter is null.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            var viewport = this.GetViewport3D();
            if (this.camera != null)
            {
                this.camera.Changed -= this.CameraChanged;
            }

            this.camera = viewport != null ? viewport.Camera : null;
            if (this.camera != null)
            {
                this.camera.Changed += this.CameraChanged;
            }

            this.Update();
        }

        /// <summary>
        /// Adds the lights to the element.
        /// </summary>
        /// <param name="lightGroup">The light group.</param>
        protected override void AddLights(Model3DGroup lightGroup)
        {
            this.light = new T();
            lightGroup.Children.Add(this.light);
        }

        /// <summary>
        /// Updates the light.
        /// </summary>
        /// <param name="d">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GenericHeadLight<T>)d).Update();
        }

        /// <summary>
        /// Handles changes to the camera.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CameraChanged(object sender, EventArgs e)
        {
            this.Update();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            if (!double.IsNaN(this.Brightness))
            {
                var a = (byte)(this.Brightness * 255);
                this.light.Color = Color.FromArgb(255, a, a, a);
            }
            else
            {
                this.light.Color = this.Color;
            }

            var projectionCamera = this.camera as ProjectionCamera;
            if (projectionCamera != null)
            {
                var y = projectionCamera.LookDirection;
                var x = Vector3D.CrossProduct(projectionCamera.LookDirection, projectionCamera.UpDirection);
                x.Normalize();
                y.Normalize();
                var z = Vector3D.CrossProduct(x, y);
                var lightPosition = projectionCamera.Position + (this.Position.X * x) + (this.Position.Y * y) + (this.Position.Z * z);
                var target = projectionCamera.Position + projectionCamera.LookDirection;
                var lightDirection = target - lightPosition;
                lightDirection.Normalize();

                var spotLight = this.light as SpotLight;
                if (spotLight != null)
                {
                    spotLight.Position = lightPosition;
                    spotLight.Direction = lightDirection;
                }

                var directionalLight = this.light as DirectionalLight;
                if (directionalLight != null)
                {
                    directionalLight.Direction = lightDirection;
                }
            }
        }
    }
}