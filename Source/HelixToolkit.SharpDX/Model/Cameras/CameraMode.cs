namespace HelixToolkit.SharpDX;

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
