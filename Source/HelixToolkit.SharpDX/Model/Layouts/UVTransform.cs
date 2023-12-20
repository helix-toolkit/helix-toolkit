using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 2D UV Transform
/// </summary>
public struct UVTransform
{
    /// <summary>
    /// The rotation by radian
    /// </summary>
    public float Rotation;
    /// <summary>
    /// The scaling
    /// </summary>
    public Vector2 Scaling;
    /// <summary>
    /// The translation
    /// </summary>
    public Vector2 Translation;
    /// <summary>
    /// Gets a value indicating whether this instance has uv transform.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has uv transform; otherwise, <c>false</c>.
    /// </value>
    public bool HasUVTransform
    {
        get => Rotation != 0 || Scaling.X != 1 || Scaling.Y != 1 || Translation.X != 0 || Translation.Y != 0;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="UVTransform"/> struct.
    /// </summary>
    /// <param name="rotation">The rotation.</param>
    public UVTransform(float rotation)
    {
        Rotation = rotation;
        Scaling = Vector2.One;
        Translation = Vector2.Zero;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="UVTransform"/> struct.
    /// </summary>
    /// <param name="translation">The translation.</param>
    public UVTransform(Vector2 translation)
    {
        Rotation = 0;
        Scaling = Vector2.One;
        Translation = translation;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="UVTransform"/> struct.
    /// </summary>
    /// <param name="rotation">The rotation.</param>
    /// <param name="scaling">The scaling.</param>
    /// <param name="translation">The translation.</param>
    public UVTransform(float rotation, Vector2 scaling, Vector2 translation)
    {
        Rotation = rotation;
        Scaling = scaling;
        Translation = translation;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="UVTransform"/> struct.
    /// </summary>
    /// <param name="rotation">The rotation.</param>
    /// <param name="scalingX">The scaling x.</param>
    /// <param name="scalingY">The scaling y.</param>
    /// <param name="translationX">The translation x.</param>
    /// <param name="translationY">The translation y.</param>
    public UVTransform(float rotation, float scalingX = 1, float scalingY = 1, float translationX = 0, float translationY = 0)
    {
        Rotation = rotation;
        Scaling = new Vector2(scalingX, scalingY);
        Translation = new Vector2(translationX, translationY);
    }
    /// <summary>
    /// Performs an implicit conversion from <see cref="UVTransform"/> to <see cref="Matrix"/>.
    /// </summary>
    /// <param name="uvTransform">The uv transform.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator Matrix(UVTransform uvTransform)
    {
        var cos = (float)Math.Cos(uvTransform.Rotation);
        var sine = (float)Math.Sin(uvTransform.Rotation);
        return new Matrix(cos * uvTransform.Scaling.X, sine, 0, 0,
            -sine, cos * uvTransform.Scaling.Y, 0, 0,
            0, 0, 1, 0, uvTransform.Translation.X, uvTransform.Translation.Y, 0, 1);
    }
    /// <summary>
    /// Performs an implicit conversion from <see cref="Matrix"/> to <see cref="UVTransform"/>.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator UVTransform(Matrix matrix)
    {
        Matrix.Decompose(matrix, out var s, out var r, out var t);
        return new UVTransform(r.Angle(), new Vector2(s.X, s.Y), new Vector2(t.X, t.Y));
    }
    public static readonly UVTransform Identity = new(0, Vector2.One, Vector2.Zero);

    public float[] ToArray()
    {
        return new float[] { Rotation, Scaling.X, Scaling.Y, Translation.X, Translation.Y };
    }
}
