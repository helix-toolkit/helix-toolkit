// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BillboardTextItem.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a billboard text item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    /// <summary>
    /// Represents a billboard text item.
    /// </summary>
    public class BillboardTextItem : TextItem
    {
        /// <summary>
        /// Gets or sets the depth offset.
        /// </summary>
        /// <value>The depth offset.</value>
        public double DepthOffset { get; set; }

        /// <summary>
        /// Gets or sets the depth offset in world coordinates.
        /// </summary>
        /// <value>The depth offset.</value>
        public double WorldDepthOffset { get; set; }
    }
}