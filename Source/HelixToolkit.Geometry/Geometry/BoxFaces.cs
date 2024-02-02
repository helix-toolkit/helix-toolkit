namespace HelixToolkit.Geometry;

/// <summary>
/// Box face enumeration.
/// </summary>
[Flags]
public enum BoxFaces
{
    /// <summary>
    /// The top.
    /// </summary>
    PositiveZ = 0x1,

    /// <summary>
    /// The top.
    /// </summary>
    Top = PositiveZ,

    /// <summary>
    /// The bottom.
    /// </summary>
    NegativeZ = 0x2,

    /// <summary>
    /// The bottom.
    /// </summary>
    Bottom = NegativeZ,

    /// <summary>
    /// The left side.
    /// </summary>
    NegativeY = 0x4,

    /// <summary>
    /// The left side.
    /// </summary>
    Left = NegativeY,

    /// <summary>
    /// The right side.
    /// </summary>
    PositiveY = 0x8,

    /// <summary>
    /// The right side.
    /// </summary>
    Right = PositiveY,

    /// <summary>
    /// The front side.
    /// </summary>
    PositiveX = 0x10,

    /// <summary>
    /// The front side.
    /// </summary>
    Front = PositiveX,

    /// <summary>
    /// The back side.
    /// </summary>
    NegativeX = 0x20,

    /// <summary>
    /// The back side.
    /// </summary>
    Back = NegativeX,

    /// <summary>
    /// All sides.
    /// </summary>
    All = PositiveZ | NegativeZ | NegativeY | PositiveY | PositiveX | NegativeX
}
