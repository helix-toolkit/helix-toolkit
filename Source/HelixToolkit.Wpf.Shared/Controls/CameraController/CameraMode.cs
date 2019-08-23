// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraMode.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Camera movement modes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    /// <summary>
    /// Camera movement modes.
    /// </summary>
    public enum CameraMode
    {
        /// <summary>
        /// Orbits around a point (fixed target position, move closer target when zooming).
        /// </summary>
        Inspect,

        /// <summary>
        /// Walk around (fixed camera position, move in cameradirection when zooming).
        /// </summary>
        WalkAround,

        /// <summary>
        /// Fixed camera target, change FOV when zooming.
        /// </summary>
        FixedPosition
    }
}