using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface ITransform
{
    /// <summary>
    /// Local transform
    /// </summary>
    Matrix ModelMatrix
    {
        set; get;
    }
    /// <summary>
    /// Transform from its parent
    /// </summary>
    Matrix ParentMatrix
    {
        set; get;
    }
    /// <summary>
    /// Total model transform by ModelMatrix * ParentMatrix
    /// </summary>
    Matrix TotalModelMatrix
    {
        get;
    }
}
