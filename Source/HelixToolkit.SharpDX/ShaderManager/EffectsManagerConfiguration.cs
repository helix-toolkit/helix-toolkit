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
}
