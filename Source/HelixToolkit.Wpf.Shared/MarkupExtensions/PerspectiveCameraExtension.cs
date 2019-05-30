// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerspectiveCameraExtension.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A markup extension creating a perspective camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows.Markup;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A markup extension creating a perspective camera.
    /// </summary>
    /// <example>
    /// <code>
    /// Camera={ht:PerspectiveCamera 10,10,20}
    ///  </code>
    /// </example>
    public class PerspectiveCameraExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveCameraExtension"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        public PerspectiveCameraExtension(double x, double y, double z)
            : this(x, y, z, -x, -y, -z)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveCameraExtension"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        /// <param name="dx">
        /// The dx.
        /// </param>
        /// <param name="dy">
        /// The dy.
        /// </param>
        /// <param name="dz">
        /// The dz.
        /// </param>
        public PerspectiveCameraExtension(double x, double y, double z, double dx, double dy, double dz)
        {
            this.Position = new Point3D(x, y, z);
            this.LookDirection = new Vector3D(dx, dy, dz);
            this.UpDirection = new Vector3D(0, 0, 1);
            this.FieldOfView = 60;
        }

        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <value>The field of view.</value>
        public double FieldOfView { get; set; }

        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>The look direction.</value>
        public Vector3D LookDirection { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point3D Position { get; set; }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>Up direction.</value>
        public Vector3D UpDirection { get; set; }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PerspectiveCamera(this.Position, this.LookDirection, this.UpDirection, this.FieldOfView);
        }

    }
}