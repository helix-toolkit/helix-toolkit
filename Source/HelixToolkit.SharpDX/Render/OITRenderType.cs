namespace HelixToolkit.SharpDX;

public enum OITRenderType
{
    None,
    /// <summary>
    /// Use weighted order independent transparent rendering. This OIT is the fastest but not color accurate in many cases.
    /// </summary>
    SinglePassWeighted,
    /// <summary>
    /// Use classic depth peeling method for independent transparent rendering.
    /// </summary>
    DepthPeeling
}
