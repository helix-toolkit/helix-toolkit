/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if NETFX_CORE
using Point3D = SharpDX.Vector3;
using Vector3D = SharpDX.Vector3;
namespace HelixToolkit.UWP
#else
using System.Windows.Media.Media3D;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public interface ICamera
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        Point3D Position { get; set; }

        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>
        /// The look direction.
        /// </value>
        Vector3D LookDirection { get; set; }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        Vector3D UpDirection { get; set; }
        /// <summary>
        /// Creates the view matrix.
        /// </summary>
        /// <returns>A <see cref="Matrix" />.</returns>
        Matrix CreateViewMatrix();

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>A <see cref="Matrix" />.</returns>
        Matrix CreateProjectionMatrix(double aspectRatio);
    }
}
