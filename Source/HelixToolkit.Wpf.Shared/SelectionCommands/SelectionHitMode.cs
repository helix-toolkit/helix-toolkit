// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionHitMode.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Specifies the selection hit mode.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    /// <summary>
    /// Specifies the selection hit mode.
    /// </summary>
    public enum SelectionHitMode
    {
        /// <summary>
        /// Selects models touching the selection range.
        /// </summary>
        Touch,

        /// <summary>
        /// Selects models completely inside selection range.
        /// </summary>
        Inside,

    }
}