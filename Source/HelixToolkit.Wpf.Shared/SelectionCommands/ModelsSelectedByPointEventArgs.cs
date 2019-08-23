// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelsSelectedByPointEventArgs.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides event data for the ModelsSelected event of the PointSelectionCommand.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides event data for the ModelsSelected event of the <see cref="PointSelectionCommand" />.
    /// </summary>
    public class ModelsSelectedByPointEventArgs : ModelsSelectedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelsSelectedByPointEventArgs"/> class.
        /// </summary>
        /// <param name="selectedModels">The selected models.</param>
        /// <param name="position">The position.</param>
        /// <remarks>
        /// For the models selected by point, they are sorted by distance in ascending order.
        /// </remarks>
        public ModelsSelectedByPointEventArgs(IList<Model3D> selectedModels, Point position)
            : base(selectedModels, true)
        {
            this.Position = position;
        }

        /// <summary>
        /// Gets the rectangle of selection.
        /// </summary>
        public Point Position { get; private set; }
    }
}