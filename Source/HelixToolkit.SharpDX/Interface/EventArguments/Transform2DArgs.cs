using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public sealed class Transform2DArgs : EventArgs
{
    /// <summary>
    /// The transform
    /// </summary>
    public readonly Matrix3x2 Transform;
    /// <summary>
    /// Initializes a new instance of the <see cref="Transform2DArgs"/> class.
    /// </summary>
    /// <param name="transform">The transform.</param>
    public Transform2DArgs(Matrix3x2 transform)
    {
        Transform = transform;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Transform2DArgs"/> class.
    /// </summary>
    /// <param name="transform">The transform.</param>
    public Transform2DArgs(ref Matrix3x2 transform)
    {
        Transform = transform;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Transform2DArgs"/> to <see cref="Matrix3x2"/>.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator Matrix3x2(Transform2DArgs args)
    {
        return args.Transform;
    }
}
