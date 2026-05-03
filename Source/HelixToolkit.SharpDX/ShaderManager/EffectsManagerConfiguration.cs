namespace HelixToolkit.SharpDX;

public sealed class EffectsManagerConfiguration
{
    public int AdapterIndex
    {
        set; get;
    } = -1;

    /// <summary>
    /// Use software rendering. 
    /// <para>
    /// Limitation: Must enable swap chain rendering to support this feature.
    /// </para>
    /// </summary>
    public bool EnableSoftwareRendering
    {
        set; get;
    } = false;

    /// <summary>
    /// Initialize 2D Rendering.
    /// </summary>
    /// <remarks>
    /// Must be disabled when debugging with RenderDoc, for example
    /// </remarks>
    public bool Initialize2DRendering
    {
        set; get;
    } = true;
}
