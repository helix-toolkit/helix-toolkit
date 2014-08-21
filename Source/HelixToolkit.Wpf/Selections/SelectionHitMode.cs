// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionHitMode.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   The selection hit mode.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    /// <summary>
    /// The selection hit mode.
    /// </summary>
    public enum SelectionHitMode
    {
        /// <summary>
        /// The model is touching to the selection range.
        /// </summary>
        Touch,

        /// <summary>
        /// The entire model is in the selection range.
        /// </summary>
        Inside,
    }
}
