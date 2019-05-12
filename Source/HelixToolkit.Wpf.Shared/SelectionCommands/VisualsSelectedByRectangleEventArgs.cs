// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualsSelectedByRectangleEventArgs.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides event data for the VisualsSelected event of the RectangleSelectionCommand.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides event data for the VisualsSelected event of the <see cref="RectangleSelectionCommand" />.
    /// </summary>
    public class VisualsSelectedByRectangleEventArgs : VisualsSelectedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualsSelectedByRectangleEventArgs"/> class.
        /// </summary>
        /// <param name="selectedVisuals">The selected visuals.</param>
        /// <param name="rectangle">The selection rectangle.</param>
        /// <remarks>
        /// For the visuals selected by rectangle, they are not sorted by distance in ascending order.
        /// </remarks>
        public VisualsSelectedByRectangleEventArgs(IList<Visual3D> selectedVisuals, Rect rectangle)
            : base(selectedVisuals, false)
        {
            this.Rectangle = rectangle;
        }

        /// <summary>
        /// Gets the rectangle of selection.
        /// </summary>
        public Rect Rectangle { get; private set; }
    }
}