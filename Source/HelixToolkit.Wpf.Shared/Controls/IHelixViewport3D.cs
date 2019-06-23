// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHelixViewport3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interface for 3D viewports.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Controls;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Interface for 3D viewports.
    /// </summary>
    public interface IHelixViewport3D
    {
        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>The camera.</value>
        ProjectionCamera Camera { get; }

        /// <summary>
        /// Gets the camera controller.
        /// </summary>
        /// <value>The camera controller.</value>
        CameraController CameraController { get; }

        /// <summary>
        /// Gets the lights.
        /// </summary>
        /// <value>The lights.</value>
        Model3DGroup Lights { get; }

        /// <summary>
        /// Gets the viewport.
        /// </summary>
        /// <value>The viewport.</value>
        Viewport3D Viewport { get; }

        /// <summary>
        /// Copies the view to the clipboard.
        /// </summary>
        void Copy();

        /// <summary>
        /// Copies the view to the clipboard as xaml.
        /// </summary>
        void CopyXaml();

        /// <summary>
        /// Exports the view to the specified file name.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        void Export(string fileName);

        /// <summary>
        /// Zooms to extents.
        /// </summary>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        void ZoomExtents(double animationTime);

    }
}