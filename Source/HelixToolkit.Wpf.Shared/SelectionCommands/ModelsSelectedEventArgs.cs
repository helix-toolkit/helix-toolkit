// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelsSelectedEventArgs.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides event data for the ModelsSelected event of the SelectionCommand.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides event data for the ModelsSelected event of the <see cref="SelectionCommand" />.
    /// </summary>
    public class ModelsSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelsSelectedEventArgs" /> class.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="areSortedByDistanceAscending">if set to <c>true</c> the selected models are sorted by distance in ascending order.</param>
        public ModelsSelectedEventArgs(IList<Model3D> selected, bool areSortedByDistanceAscending)
        {
            this.SelectedModels = selected;
            this.AreSortedByDistanceAscending = areSortedByDistanceAscending;
        }

        /// <summary>
        /// Gets the selected models.
        /// </summary>
        public IList<Model3D> SelectedModels { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the selected models are sorted by distance in ascending order.
        /// </summary>
        /// <value>
        /// <c>true</c> if the selected models are sorted by distance in ascending order; otherwise, <c>false</c>.
        /// </value>
        public bool AreSortedByDistanceAscending { get; private set; }
    }
}