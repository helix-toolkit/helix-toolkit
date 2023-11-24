namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IGeometryBufferModel : IAttachableBufferModel
{
    event EventHandler VertexBufferUpdated;
    event EventHandler IndexBufferUpdated;
    /// <summary>
    /// Gets or sets the effects manager.
    /// </summary>
    /// <value>
    /// The effects manager.
    /// </value>
    IEffectsManager? EffectsManager
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the geometry.
    /// </summary>
    /// <value>
    /// The geometry.
    /// </value>
    Geometry3D? Geometry
    {
        get; set;
    }
}
