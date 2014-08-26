// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelsSelectedEventArgs.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   Provides event data for the SelectionCommand.ModelsSelected event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides event data for the <see cref="SelectionCommand.ModelsSelected" /> event.
    /// </summary>
    public class ModelsSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelsSelectedEventArgs"/> class.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        public ModelsSelectedEventArgs(IList<Model3D> selected)
        {
            this.SelectedModels = selected;
        }

        /// <summary>
        /// Gets or sets the selected models.
        /// </summary>
        public IList<Model3D> SelectedModels { get; set; }
    }
}
