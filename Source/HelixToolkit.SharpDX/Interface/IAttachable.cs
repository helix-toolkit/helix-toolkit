namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IAttachable
{
    /// <summary>
    /// 
    /// </summary>
    bool IsAttached
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="host"></param>
    void Attach(IRenderHost host);
    /// <summary>
    /// 
    /// </summary>
    void Detach();
}
