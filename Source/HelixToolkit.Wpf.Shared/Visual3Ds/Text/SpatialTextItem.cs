// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpatialTextItem.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a spatial text item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents a spatial text item.
    /// </summary>
    public class SpatialTextItem : TextItem
    {
        /// <summary>
        /// Gets or sets the text direction.
        /// </summary>
        /// <value>The text direction.</value>
        public Vector3D TextDirection { get; set; }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>Up direction.</value>
        public Vector3D UpDirection { get; set; }
    }
}