// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrthographicCamera.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents an orthographic projection camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows;

    using global::SharpDX;
    using System;

    /// <summary>
    /// Represents an orthographic projection camera.
    /// </summary>
    public class OrthographicCamera : ProjectionCamera
    {
        /// <summary>
        /// The width property
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width", typeof(double), typeof(OrthographicCamera), new PropertyMetadata(10.0));

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get { return (double)this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }

        public OrthographicCamera()
        {
            // default values for near-far must be different for ortho:
            NearPlaneDistance = -10.0;
            FarPlaneDistance = 100.0;
        }

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">
        /// The aspect ratio.
        /// </param>
        /// <returns>
        /// A <see cref="Matrix"/>.
        /// </returns>
        public override Matrix CreateProjectionMatrix(double aspectRatio)
        {
            Matrix projM;
            if (this.CreateLeftHandSystem)
            {
                projM = Matrix.OrthoLH(
                    (float)this.Width, 
                    (float)(this.Width / aspectRatio), 
                    (float)this.NearPlaneDistance,
                    Math.Min(1e15f, (float)this.FarPlaneDistance));
            }
            else
            {
                projM = Matrix.OrthoRH(
                    (float)this.Width, 
                    (float)(this.Width / aspectRatio), 
                    (float)this.NearPlaneDistance,
                    Math.Min(1e15f, (float)this.FarPlaneDistance));
            }
            return projM;
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new OrthographicCamera();
        }
    }
}