namespace HelixToolkit.SharpDX.Model.Scene;

public enum InvalidateTypes
{
    /// <summary>
    /// Notify if scene needs re-rendered.
    /// </summary>
    Render,
    /// <summary>
    /// Notify if scene graph structure has changed.
    /// </summary>
    SceneGraph,
    /// <summary>
    /// Notify to rebuild per frame renderables.
    /// </summary>
    PerFrameRenderables
}
