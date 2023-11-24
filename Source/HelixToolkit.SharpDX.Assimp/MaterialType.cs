namespace HelixToolkit.SharpDX.Assimp;

/// <summary>
/// </summary>
public enum MaterialType
{
    /// <summary>
    ///     Automatic determine material type
    /// </summary>
    Auto,

    /// <summary>
    ///     The blinn phong
    /// </summary>
    BlinnPhong,

    /// <summary>
    ///     The PBR
    /// </summary>
    PBR,

    /// <summary>
    ///     The diffuse
    /// </summary>
    Diffuse,

    /// <summary>
    ///     The vertex color
    /// </summary>
    VertexColor,

    /// <summary>
    ///     The normal
    /// </summary>
    Normal,

    /// <summary>
    ///     The position
    /// </summary>
    Position
}
