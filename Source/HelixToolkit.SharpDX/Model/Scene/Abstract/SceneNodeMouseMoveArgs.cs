using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

public class SceneNodeMouseMoveArgs : EventArgs
{
    public HitTestResult HitResult
    {
        get;
    }
    public SceneNode Source
    {
        get;
    }
    public IViewport3DX Viewport
    {
        get;
    }
    public Vector2 Position
    {
        get;
    }
    public object? OriginalInputEventArgs
    {
        get;
    }
    public SceneNodeMouseMoveArgs(IViewport3DX viewport, Vector2 pos, SceneNode node, HitTestResult hit, object? originalInputEventArgs = null)
    {
        Viewport = viewport;
        Position = pos;
        Source = node;
        HitResult = hit;
        OriginalInputEventArgs = originalInputEventArgs;
    }
}
