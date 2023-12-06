using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public sealed class TransformArgs : EventArgs
{
    /// <summary>
    /// The transform
    /// </summary>
    public readonly Matrix Transform;
    /// <summary>
    /// Initializes a new instance of the <see cref="TransformArgs"/> class.
    /// </summary>
    /// <param name="transform">The transform.</param>
    public TransformArgs(Matrix transform)
    {
        Transform = transform;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="TransformArgs"/> class.
    /// </summary>
    /// <param name="transform">The transform.</param>
    public TransformArgs(ref Matrix transform)
    {
        Transform = transform;
    }
    /// <summary>
    /// Performs an implicit conversion from <see cref="TransformArgs"/> to <see cref="Matrix"/>.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator Matrix(TransformArgs args)
    {
        return args.Transform;
    }
}
