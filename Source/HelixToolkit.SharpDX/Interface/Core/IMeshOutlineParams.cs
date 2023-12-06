using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IMeshOutlineParams
{
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    Color4 Color
    {
        set; get;
    }
    /// <summary>
    /// Enable outline
    /// </summary>
    bool OutlineEnabled
    {
        set; get;
    }

    /// <summary>
    /// Draw original mesh
    /// </summary>
    bool DrawMesh
    {
        set; get;
    }

    /// <summary>
    /// Draw outline order
    /// </summary>
    bool DrawOutlineBeforeMesh
    {
        set; get;
    }

    /// <summary>
    /// Outline fading
    /// </summary>
    float OutlineFadingFactor
    {
        set; get;
    }
}
