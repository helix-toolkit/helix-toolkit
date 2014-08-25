// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RangeSelectionEventArgs.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   The selection routed event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Selections
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// The selection routed event args.
    /// </summary>
    public class RangeSelectionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSelectionEventArgs"/> class.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        /// <param name="startPoint">
        /// The start Point.
        /// </param>
        /// <param name="lastPoint">
        /// The last Point.
        /// </param>
        public RangeSelectionEventArgs(IList<Model3D> selected, Point startPoint, Point lastPoint)
        {
            this.SelectedModels = selected;
            this.StartPoint = startPoint;
            this.EndPoint = lastPoint;
        }

        /// <summary>
        /// Gets or sets the selected models.
        /// </summary>
        public IList<Model3D> SelectedModels { get; set; }

        /// <summary>
        /// Gets the start point.
        /// </summary>
        public Point StartPoint { get; internal set; }

        /// <summary>
        /// Gets the end point.
        /// </summary>
        public Point EndPoint { get; internal set; }
    }
}
