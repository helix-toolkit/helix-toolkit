// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraRotationMode.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Camera rotation modes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    /// <summary>
    /// Camera rotation modes.
    /// </summary>
    public enum CameraRotationMode
    {
        /// <summary>
        /// Turntable is constrained to two axes of rotation (model up and right direction)
        /// </summary>
        Turntable,

        /// <summary>
        /// Turnball using three axes (look direction, right direction and up direction (on the left/right edges)).
        /// </summary>
        Turnball,

        /// <summary>
        /// Using a virtual trackball.
        /// </summary>
        Trackball
    }
}