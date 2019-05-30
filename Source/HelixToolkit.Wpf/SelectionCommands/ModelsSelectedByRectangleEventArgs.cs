// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelsSelectedByRectangleEventArgs.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides event data for the ModelsSelected event of the RectangleSelectionCommand.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides event data for the ModelsSelected event of the <see cref="RectangleSelectionCommand" />.
    /// </summary>
    public class ModelsSelectedByRectangleEventArgs : ModelsSelectedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelsSelectedByRectangleEventArgs"/> class.
        /// </summary>
        /// <param name="selectedModels">The selected models.</param>
        /// <param name="rectangle">The selection rectangle.</param>
        /// <remarks>
        /// For the models selected by rectangle, they are not sorted by distance in ascending order.
        /// </remarks>
        public ModelsSelectedByRectangleEventArgs(IList<Model3D> selectedModels, Rect rectangle)
            : base(selectedModels, false)
        {
            this.Rectangle = rectangle;
        }

        /// <summary>
        /// Gets the rectangle of selection.
        /// </summary>
        public Rect Rectangle { get; private set; }
    }
}