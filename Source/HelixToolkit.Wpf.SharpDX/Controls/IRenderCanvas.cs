namespace HelixToolkit.Wpf.SharpDX.Controls
{
    /// <summary>
    /// Canvas holds the RenderHost. Provide entry point or render surface for RenderHost to render to.
    /// </summary>
    public interface IRenderCanvas
    {
        /// <summary>
        /// Gets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        IRenderHost RenderHost { get; }
    }
}
