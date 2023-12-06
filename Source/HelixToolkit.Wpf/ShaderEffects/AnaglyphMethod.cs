namespace HelixToolkit.Wpf;

/// <summary>
/// Anaglyph method type.
/// </summary>
public enum AnaglyphMethod
{
    /// <summary>
    /// True anaglyph.
    /// </summary>
    /// <remarks>
    /// Dark image
    /// No color reproduction
    /// Little ghosting
    /// </remarks>
    True = 0,

    /// <summary>
    /// Grayscale anaglyph.
    /// </summary>
    /// <remarks>
    /// No color reproduction
    /// More ghosting than true anaglyphs
    /// </remarks>
    Gray = 1,

    /// <summary>
    /// Color anaglyph.
    /// </summary>
    /// <remarks>
    /// Partial color reproduction
    /// Retinal rivalry
    /// </remarks>
    Color = 2,

    /// <summary>
    /// Half-color anaglyph.
    /// </summary>
    /// <remarks>
    /// Partial color reproduction (but not as good as color anaglyphs)
    /// Less retinal rivalry than color anaglyphs
    /// </remarks>
    HalfColor = 3,

    /// <summary>
    /// Optimized anaglyph.
    /// </summary>
    /// <remarks>
    /// Partial color reproduction (but not of red shades)
    /// Almost no retinal rivalry
    /// </remarks>
    Optimized = 4,

    /// <summary>
    /// Dubois anaglyph.
    /// </summary>
    Dubois = 5
}
