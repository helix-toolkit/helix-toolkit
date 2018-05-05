// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraMode.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Camera movement modes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
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
        /// Walk around (fixed camera position when rotating, move in camera direction when zooming).
        /// </summary>
        WalkAround,

        /// <summary>
        /// Fixed camera target, change field of view when zooming.
        /// </summary>
        FixedPosition
    }
}