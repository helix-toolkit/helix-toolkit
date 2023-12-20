using Assimp;

namespace HelixToolkit.SharpDX.Assimp;

/// <summary>
/// 
/// </summary>
public static class Extensions
{
    /// <summary>
    /// To the sharp dx matrix. Already transposed after this function
    /// </summary>
    /// <param name="m">The m.</param>
    /// <param name="isColumnMajor"></param>
    /// <returns></returns>
    public static Matrix ToHxMatrix(this global::Assimp.Matrix4x4 m, bool isColumnMajor)
    {
        var matrix = new Matrix(m.A1, m.A2, m.A3, m.A4, m.B1, m.B2, m.B3, m.B4, m.C1, m.C2, m.C3, m.C4, m.D1, m.D2, m.D3, m.D4);
        if (isColumnMajor)
        {
            matrix = Matrix.Transpose(matrix);
        }
        return matrix;
    }
    /// <summary>
    /// To the assimp matrix. Already transposed after this function
    /// </summary>
    /// <param name="m">The m.</param>
    /// <param name="toColumnMajor"></param>
    /// <returns></returns>
    public static global::Assimp.Matrix4x4 ToAssimpMatrix(this Matrix m, bool toColumnMajor)
    {
        var matrix = new global::Assimp.Matrix4x4(m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44);
        if (toColumnMajor)
        {
            matrix.Transpose();
        }
        return matrix;
    }

    public static Vector3 ToVector3(this global::Assimp.Vector3D v)
    {
        return new System.Numerics.Vector3(v.X, v.Y, v.Z);
    }
    /// <summary>
    /// To the assimp vector3d.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    public static global::Assimp.Vector3D ToAssimpVector3D(this Vector3 v)
    {
        return new global::Assimp.Vector3D(v.X, v.Y, v.Z);
    }
    /// <summary>
    /// To the sharp dx vector2.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    public static Vector2 ToVector2(this global::Assimp.Vector2D v)
    {
        return new Vector2(v.X, v.Y);
    }

    /// <summary>
    /// To the assimp vector2d.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    public static global::Assimp.Vector2D ToAssimpVector2D(this Vector2 v)
    {
        return new global::Assimp.Vector2D(v.X, v.Y);
    }
    /// <summary>
    /// To the assimp vector3d.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    public static global::Assimp.Vector3D ToAssimpVector3D(this Vector2 v)
    {
        return new global::Assimp.Vector3D(v.X, v.Y, 0);
    }
    /// <summary>
    /// To the sharp dx vector2.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    public static Vector2 ToVector2(this global::Assimp.Vector3D v)
    {
        return new Vector2(v.X, v.Y);
    }
    /// <summary>
    /// To the sharp dx color4.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    public static Color4 ToColor4(this global::Assimp.Color4D v)
    {
        return new Color4(v.R, v.G, v.B, v.A);
    }

    /// <summary>
    /// To the assimp color4d.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static Color4D ToAssimpColor4D(this Color4 v, float alpha = 1f)
    {
        return new Color4D(v.Red, v.Green, v.Blue, 1f);
    }

    /// <summary>
    /// To the sharp dx quaternion.
    /// </summary>
    /// <param name="q">The q.</param>
    /// <returns></returns>
    public static Quaternion ToHelixQuaternion(this global::Assimp.Quaternion q)
    {
        return new Quaternion(q.X, q.Y, q.Z, q.W);
    }

    /// <summary>
    /// To the assimp quaternion.
    /// </summary>
    /// <param name="q">The q.</param>
    /// <returns></returns>
    public static global::Assimp.Quaternion ToAssimpQuaternion(this Quaternion q)
    {
        return new global::Assimp.Quaternion(q.W, q.X, q.Y, q.Z);
    }

    /// <summary>
    /// To the Helix UVTransform.
    /// </summary>
    /// <param name="transform">The transform.</param>
    /// <returns></returns>
    public static UVTransform ToHelixUVTransform(this global::Assimp.UVTransform transform)
    {
        return new UVTransform(transform.Rotation, transform.Scaling.ToVector2(), transform.Translation.ToVector2());
    }

    /// <summary>
    /// To the type of the helix metadata.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">Type {type} is not supported.</exception>
    public static Model.MetaDataType ToHelixMetadataType(this global::Assimp.MetaDataType type)
    {
        return type switch
        {
            MetaDataType.Bool => Model.MetaDataType.Bool,
            MetaDataType.Double => Model.MetaDataType.Double,
            MetaDataType.Float => Model.MetaDataType.Float,
            MetaDataType.Int32 => Model.MetaDataType.Int32,
            MetaDataType.String => Model.MetaDataType.String,
            MetaDataType.UInt64 => Model.MetaDataType.UInt64,
            MetaDataType.Vector3D => Model.MetaDataType.Vector3D,
            _ => throw new NotSupportedException($"Type {type} is not supported."),
        };
    }

    /// <summary>
    /// To the type of the assimp metadata.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">Type {type} is not supported.</exception>
    public static MetaDataType ToAssimpMetadataType(this Model.MetaDataType type)
    {
        return type switch
        {
            Model.MetaDataType.Bool => MetaDataType.Bool,
            Model.MetaDataType.Double => MetaDataType.Double,
            Model.MetaDataType.Float => MetaDataType.Float,
            Model.MetaDataType.Int32 => MetaDataType.Int32,
            Model.MetaDataType.String => MetaDataType.String,
            Model.MetaDataType.UInt64 => MetaDataType.UInt64,
            Model.MetaDataType.Vector3D => MetaDataType.Vector3D,
            _ => throw new NotSupportedException($"Type {type} is not supported."),
        };
    }

    /// <summary>
    /// To the helix metadata.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, Model.Metadata.Entry>> ToHelixMetadata(this Metadata m)
    {
        foreach (var d in m)
        {
            yield return new KeyValuePair<string, Model.Metadata.Entry>(d.Key, new Model.Metadata.Entry(d.Value.DataType.ToHelixMetadataType(), d.Value.Data));
        }
    }

    /// <summary>
    /// To the assimp metadata.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, Metadata.Entry>> ToAssimpMetadata(this Model.Metadata m)
    {
        foreach (var d in m)
        {
            yield return new KeyValuePair<string, Metadata.Entry>(d.Key, new Metadata.Entry(d.Value.DataType.ToAssimpMetadataType(), d.Value.Data));
        }
    }
}
