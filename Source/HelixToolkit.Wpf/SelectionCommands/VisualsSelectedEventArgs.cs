// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualsSelectedEventArgs.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides event data for the VisualsSelected event of the SelectionCommand.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides event data for the VisualsSelected event of the <see cref="SelectionCommand" />.
    /// </summary>
    public class VisualsSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualsSelectedEventArgs" /> class.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="areSortedByDistanceAscending">if set to <c>true</c> the selected visuals are sorted by distance in ascending order.</param>
        public VisualsSelectedEventArgs(IList<Visual3D> selected, bool areSortedByDistanceAscending)
        {
            this.SelectedVisuals = selected;
            this.AreSortedByDistanceAscending = areSortedByDistanceAscending;
        }

        /// <summary>
        /// Gets the selected visuals.
        /// </summary>
        public IList<Visual3D> SelectedVisuals { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the selected visuals are sorted by distance in ascending order.
        /// </summary>
        /// <value>
        /// <c>true</c> if the selected visuals are sorted by distance in ascending order; otherwise, <c>false</c>.
        /// </value>
        public bool AreSortedByDistanceAscending { get; private set; }
    }
}