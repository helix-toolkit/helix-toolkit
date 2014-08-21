// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionRoutedEventArgs.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   The selection routed event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Selections
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// The selection routed event args.
    /// </summary>
    public class SelectionRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        public SelectionRoutedEventArgs(IList<Model3D> selected)
        {
            this.SelectedModels = selected;
        }

        /// <summary>
        /// Gets or sets the selected models.
        /// </summary>
        public IList<Model3D> SelectedModels { get; set; }
    }
}
