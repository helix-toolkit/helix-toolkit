/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
#endif
#endif
{
    /// <summary>
    /// Camera interface
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>
        /// The look direction.
        /// </value>
        Vector3 LookDirection { get; set; }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        Vector3 UpDirection { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [create left hand system].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create left hand system]; otherwise, <c>false</c>.
        /// </value>
        bool CreateLeftHandSystem { set; get; }
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
        Matrix CreateProjectionMatrix(float aspectRatio);
        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <param name="nearPlane">The near plane.</param>
        /// <param name="farPlane">The far plane.</param>
        /// <returns></returns>
        Matrix CreateProjectionMatrix(float aspectRatio, float nearPlane, float farPlane);
    }
}
