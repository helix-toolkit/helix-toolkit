// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenGeometryBuilder.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Base class for mesh geometry builders that work on screen coordinates.
    /// </summary>
    public abstract class ScreenGeometryBuilder
    {
        /// <summary>
        /// The parent visual.
        /// </summary>
        protected readonly Visual3D visual;

        /// <summary>
        /// The screen to visual transformation matrix.
        /// </summary>
        protected Matrix3D screenToVisual;

        /// <summary>
        /// The visual to screen transformation matrix.
        /// </summary>
        protected Matrix3D visualToScreen;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenGeometryBuilder"/> class.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        protected ScreenGeometryBuilder(Visual3D visual)
        {
            this.visual = visual;
        }

        /// <summary>
        /// Updates the transforms.
        /// </summary>
        /// <returns>
        /// True if the transform was changed.
        /// </returns>
        public bool UpdateTransforms()
        {
            var newTransform = Visual3DHelper.GetViewportTransform(this.visual);

            if (double.IsNaN(newTransform.M11))
            {
                return false;
            }

            if (!newTransform.HasInverse)
            {
                return false;
            }

            if (newTransform == this.visualToScreen)
            {
                return false;
            }

            this.visualToScreen = this.screenToVisual = newTransform;
            this.screenToVisual.Invert();

            return true;
        }

    }
}