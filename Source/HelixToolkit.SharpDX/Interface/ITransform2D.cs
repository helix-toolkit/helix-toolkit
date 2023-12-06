using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface ITransform2D
{
    /// <summary>
    /// Gets or sets the model matrix.
    /// </summary>
    /// <value>
    /// The model matrix.
    /// </value>
    Matrix3x2 ModelMatrix
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the parent matrix.
    /// </summary>
    /// <value>
    /// The parent matrix.
    /// </value>
    Matrix3x2 ParentMatrix
    {
        set; get;
    }
    /// <summary>
    /// Gets the total model matrix.
    /// </summary>
    /// <value>
    /// The total model matrix.
    /// </value>
    Matrix3x2 TotalModelMatrix
    {
        get;
    }
}
