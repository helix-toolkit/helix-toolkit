using System.Runtime.CompilerServices;

namespace HelixToolkit.Wpf;

public static class ConverterExtensions
{
    #region Array
    public static float[]? ToFloatArray(this double[]? array)
    {
        if (array is null)
        {
            return null;
        }

        if (array.Length == 0)
        {
            return Array.Empty<float>();
        }

        var result = new float[array.Length];

        FloatingPointArrayConverters.ConvertDoubleToFloat(array.Length, array, result);

        return result;
    }

    public static double[]? ToDoubleArray(this float[]? array)
    {
        if (array is null)
        {
            return null;
        }

        if (array.Length == 0)
        {
            return Array.Empty<double>();
        }

        var result = new double[array.Length];

        FloatingPointArrayConverters.ConvertFloatToDouble(array.Length, array, result);

        return result;
    }
    #endregion

    #region System.Numerics.Vector2
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Point ToWndPoint(this System.Numerics.Vector2 vector)
    {
        return new System.Windows.Point(vector.X, vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector2 ToVector2(this System.Windows.Point vector)
    {
        return new System.Numerics.Vector2((float)vector.X, (float)vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Size ToWndSize(this System.Numerics.Vector2 vector)
    {
        return new System.Windows.Size(vector.X, vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector2 ToVector2(this System.Windows.Size size)
    {
        return new System.Numerics.Vector2((float)size.Width, (float)size.Height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Vector ToWndVector(this System.Numerics.Vector2 vector)
    {
        return new System.Windows.Vector(vector.X, vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector2 ToVector2(this System.Windows.Vector vector)
    {
        return new System.Numerics.Vector2((float)vector.X, (float)vector.Y);
    }
    #endregion

    #region System.Numerics.Vector3
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Point3D ToWndPoint3D(this System.Numerics.Vector3 vector)
    {
        return new System.Windows.Media.Media3D.Point3D(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 ToVector3(this System.Windows.Media.Media3D.Point3D vector)
    {
        return new System.Numerics.Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Vector3D ToWndVector3D(this System.Numerics.Vector3 vector)
    {
        return new System.Windows.Media.Media3D.Vector3D(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 ToVector3(this System.Windows.Media.Media3D.Vector3D vector)
    {
        return new System.Numerics.Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Size3D ToWndSize3D(this System.Numerics.Vector3 vector)
    {
        return new System.Windows.Media.Media3D.Size3D(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 ToVector3(this System.Windows.Media.Media3D.Size3D size)
    {
        return new System.Numerics.Vector3((float)size.X, (float)size.Y, (float)size.Z);
    }
    #endregion

    #region System.Numerics.Vector4
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Point4D ToWnPoint4D(this System.Numerics.Vector4 vector)
    {
        return new System.Windows.Media.Media3D.Point4D(vector.X, vector.Y, vector.Z, vector.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector4 ToVector4(this System.Windows.Media.Media3D.Point4D vector)
    {
        return new System.Numerics.Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, (float)vector.W);
    }
    #endregion

    #region System.Numerics.Matrix4x4
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Matrix3D ToWndMatrix3D(this System.Numerics.Matrix4x4 matrix)
    {
        return new System.Windows.Media.Media3D.Matrix3D(
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M32, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Matrix4x4 ToMatrix(this System.Windows.Media.Media3D.Matrix3D matrix)
    {
        return new System.Numerics.Matrix4x4(
            (float)matrix.M11, (float)matrix.M12, (float)matrix.M13, (float)matrix.M14,
            (float)matrix.M21, (float)matrix.M22, (float)matrix.M23, (float)matrix.M24,
            (float)matrix.M31, (float)matrix.M32, (float)matrix.M33, (float)matrix.M34,
            (float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ, (float)matrix.M44);
    }
    #endregion

    #region System.Windows.Media.Media3D
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Point3D ToWndPoint3D(this System.Windows.Media.Media3D.Vector3D vector)
    {
        return (System.Windows.Media.Media3D.Point3D)vector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Vector3D ToWndVector3D(this System.Windows.Media.Media3D.Point3D point)
    {
        return (System.Windows.Media.Media3D.Vector3D)point;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Size3D ToWndSize3D(this System.Windows.Media.Media3D.Vector3D vector)
    {
        return (System.Windows.Media.Media3D.Size3D)vector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Vector3D ToWndVector3D(this System.Windows.Media.Media3D.Size3D size)
    {
        return (System.Windows.Media.Media3D.Vector3D)size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Point3D ToWndPoint3D(this System.Windows.Media.Media3D.Size3D size)
    {
        return (System.Windows.Media.Media3D.Point3D)size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Size3D ToWndSize3D(this System.Windows.Media.Media3D.Point3D point)
    {
        return (System.Windows.Media.Media3D.Size3D)point.ToWndVector3D();
    }
    #endregion

    #region System.Windows.Media
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Point ToWndPoint(this System.Windows.Vector vector)
    {
        return (System.Windows.Point)vector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Vector ToWndVector(this System.Windows.Point point)
    {
        return (System.Windows.Vector)point;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Point ToWndPoint(this System.Windows.Size vector)
    {
        return (System.Windows.Point)vector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Size ToWndSize(this System.Windows.Point point)
    {
        return (System.Windows.Size)point;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Size ToWndSize(this System.Windows.Vector vector)
    {
        return (System.Windows.Size)vector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Vector ToWndVector(this System.Windows.Size size)
    {
        return (System.Windows.Vector)size;
    }
    #endregion

    #region HelixToolkit.Maths.BoundingBox
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Rect3D ToWnRect3D(this HelixToolkit.Maths.BoundingBox boundingBox)
    {
        return new System.Windows.Media.Media3D.Rect3D(boundingBox.Minimum.ToWndPoint3D(), boundingBox.Size.ToWndSize3D());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HelixToolkit.Maths.BoundingBox ToBoundingBox(this System.Windows.Media.Media3D.Rect3D rect)
    {
        return new HelixToolkit.Maths.BoundingBox(rect.Location.ToVector3(), rect.Location.ToVector3() + rect.Size.ToVector3());
    }
    #endregion

    #region System.Numerics.Plane
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HelixToolkit.Wpf.Plane3D ToPlane3D(this System.Numerics.Plane plane)
    {
        return new HelixToolkit.Wpf.Plane3D((plane.Normal * plane.D).ToWndPoint3D(), plane.Normal.ToWndVector3D());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Plane ToPlane(this HelixToolkit.Wpf.Plane3D plane)
    {
        return HelixToolkit.Maths.PlaneHelper.Create(plane.Position.ToVector3(), plane.Normal.ToVector3());
    }
    #endregion

    #region HelixToolkit.Maths.Ray
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HelixToolkit.Wpf.Ray3D ToRay3D(this HelixToolkit.Maths.Ray ray)
    {
        return new HelixToolkit.Wpf.Ray3D(ray.Position.ToWndPoint3D(), ray.Direction.ToWndVector3D());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HelixToolkit.Maths.Ray ToRay(this HelixToolkit.Wpf.Ray3D ray)
    {
        return new HelixToolkit.Maths.Ray(ray.Origin.ToVector3(), ray.Direction.ToVector3());
    }
    #endregion

    #region Collection
    public static System.Windows.Media.Media3D.Vector3DCollection? ToWndVector3DCollection(this IList<System.Numerics.Vector3>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new System.Windows.Media.Media3D.Vector3DCollection(0);
        }

        var newCollection = new System.Windows.Media.Media3D.Vector3DCollection(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            if (collection is HelixToolkit.FastList<System.Numerics.Vector3> fastList)
            {
                System.Numerics.Vector3[] floatArray = fastList.GetInternalArray();
                System.Windows.Media.Media3D.Vector3D[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count * 3, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is List<System.Numerics.Vector3> list)
            {
                System.Numerics.Vector3[] floatArray = list.GetInternalArray();
                System.Windows.Media.Media3D.Vector3D[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count * 3, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is System.Numerics.Vector3[] floatArray)
            {
                System.Windows.Media.Media3D.Vector3D[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count * 3, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
        }

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToWndVector3D());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector3Collection? ToVector3Collection(this System.Windows.Media.Media3D.Vector3DCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new HelixToolkit.Vector3Collection(0);
        }

        var newCollection = new HelixToolkit.Vector3Collection();
        newCollection.IncreaseCapacity(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            System.Windows.Media.Media3D.Vector3D[]? doubleArray = collection.GetInternalArray();
            System.Numerics.Vector3[] floatArray = newCollection.GetInternalArray();

            if (doubleArray is not null)
            {
                FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 3, doubleArray, floatArray);
                return newCollection;
            }
        }

        newCollection.Clear(true);
        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector3());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector3Collection? ToVector3Collection(this IList<System.Windows.Media.Media3D.Point3D>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new HelixToolkit.Vector3Collection(0);
        }

        var newCollection = new HelixToolkit.Vector3Collection();
        newCollection.IncreaseCapacity(collection.Count);

        if (collection is HelixToolkit.FastList<System.Windows.Media.Media3D.Point3D> fastList)
        {
            System.Windows.Media.Media3D.Point3D[] doubleArray = fastList.GetInternalArray();
            System.Numerics.Vector3[]? floatArray = newCollection.GetInternalArray();

            FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 3, doubleArray, floatArray);
            return newCollection;
        }
        else if (collection is List<System.Windows.Media.Media3D.Point3D> list)
        {
            System.Windows.Media.Media3D.Point3D[] doubleArray = list.GetInternalArray();
            System.Numerics.Vector3[]? floatArray = newCollection.GetInternalArray();

            FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 3, doubleArray, floatArray);
            return newCollection;
        }
        else if (collection is System.Windows.Media.Media3D.Point3D[] doubleArray)
        {
            System.Numerics.Vector3[]? floatArray = newCollection.GetInternalArray();

            FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 3, doubleArray, floatArray);
            return newCollection;
        }

        newCollection.Clear(true);
        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector3());
        }

        return newCollection;
    }

    public static System.Windows.Media.Media3D.Point3DCollection? ToWndPoint3DCollection(this IList<System.Numerics.Vector3>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new System.Windows.Media.Media3D.Point3DCollection(0);
        }

        var newCollection = new System.Windows.Media.Media3D.Point3DCollection(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            if (collection is HelixToolkit.FastList<System.Numerics.Vector3> fastList)
            {
                System.Numerics.Vector3[] floatArray = fastList.GetInternalArray();
                System.Windows.Media.Media3D.Point3D[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count * 3, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is List<System.Numerics.Vector3> list)
            {
                System.Numerics.Vector3[] floatArray = list.GetInternalArray();
                System.Windows.Media.Media3D.Point3D[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count * 3, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is System.Numerics.Vector3[] floatArray)
            {
                System.Windows.Media.Media3D.Point3D[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count * 3, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
        }

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToWndPoint3D());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector3Collection? ToVector3Collection(this System.Windows.Media.Media3D.Point3DCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new HelixToolkit.Vector3Collection(0);
        }

        var newCollection = new HelixToolkit.Vector3Collection();
        newCollection.IncreaseCapacity(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            System.Windows.Media.Media3D.Point3D[]? doubleArray = collection.GetInternalArray();
            System.Numerics.Vector3[] floatArray = newCollection.GetInternalArray();

            if (doubleArray is not null)
            {
                FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 3, doubleArray, floatArray);
                return newCollection;
            }
        }

        newCollection.Clear(true);
        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector3());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector2Collection? ToVector2Collection(this IList<System.Windows.Point>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new HelixToolkit.Vector2Collection(0);
        }

        var newCollection = new HelixToolkit.Vector2Collection();
        newCollection.IncreaseCapacity(collection.Count);

        if (collection is HelixToolkit.FastList<System.Windows.Point> fastList)
        {
            System.Windows.Point[] doubleArray = fastList.GetInternalArray();
            System.Numerics.Vector2[]? floatArray = newCollection.GetInternalArray();

            FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 2, doubleArray, floatArray);
            return newCollection;
        }
        else if (collection is List<System.Windows.Point> list)
        {
            System.Windows.Point[] doubleArray = list.GetInternalArray();
            System.Numerics.Vector2[]? floatArray = newCollection.GetInternalArray();

            FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 2, doubleArray, floatArray);
            return newCollection;
        }
        else if (collection is System.Windows.Point[] doubleArray)
        {
            System.Numerics.Vector2[]? floatArray = newCollection.GetInternalArray();

            FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 2, doubleArray, floatArray);
            return newCollection;
        }

        newCollection.Clear(true);
        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector2());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector2Collection? ToVector2Collection(this System.Windows.Media.VectorCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new HelixToolkit.Vector2Collection(0);
        }

        var newCollection = new HelixToolkit.Vector2Collection();
        newCollection.IncreaseCapacity(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            System.Windows.Vector[]? doubleArray = collection.GetInternalArray();
            System.Numerics.Vector2[] floatArray = newCollection.GetInternalArray();

            if (doubleArray is not null)
            {
                FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 2, doubleArray, floatArray);
                return newCollection;
            }
        }

        newCollection.Clear(true);
        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector2());
        }

        return newCollection;
    }

    public static System.Windows.Media.PointCollection? ToWndPointCollection(this IList<System.Numerics.Vector2>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new System.Windows.Media.PointCollection(0);
        }

        var newCollection = new System.Windows.Media.PointCollection(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            if (collection is HelixToolkit.FastList<System.Numerics.Vector2> fastList)
            {
                System.Numerics.Vector2[] floatArray = fastList.GetInternalArray();
                System.Windows.Point[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count * 2, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is List<System.Numerics.Vector2> list)
            {
                System.Numerics.Vector2[] floatArray = list.GetInternalArray();
                System.Windows.Point[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count * 2, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is System.Numerics.Vector2[] floatArray)
            {
                System.Windows.Point[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count * 2, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
        }

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToWndPoint());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector2Collection? ToVector2Collection(this System.Windows.Media.PointCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new HelixToolkit.Vector2Collection(0);
        }

        var newCollection = new HelixToolkit.Vector2Collection();
        newCollection.IncreaseCapacity(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            System.Windows.Point[]? doubleArray = collection.GetInternalArray();
            System.Numerics.Vector2[] floatArray = newCollection.GetInternalArray();

            if (doubleArray is not null)
            {
                FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count * 2, doubleArray, floatArray);
                return newCollection;
            }
        }

        newCollection.Clear(true);
        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector2());
        }

        return newCollection;
    }

    public static System.Windows.Media.Int32Collection? ToWndInt32Collection(this IList<int>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new System.Windows.Media.Int32Collection(0);
        }

        var newCollection = new System.Windows.Media.Int32Collection(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            if (collection is HelixToolkit.FastList<int> fastList)
            {
                int[] collectionArray = fastList.GetInternalArray();
                int[]? intArray = newCollection.GetInternalArray();

                if (intArray is not null)
                {
                    Span<int> collectionSpan = new(collectionArray, 0, collection.Count);
                    Span<int> arraySpan = new(intArray, 0, collection.Count);
                    collectionSpan.CopyTo(arraySpan);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is List<int> list)
            {
                int[] collectionArray = list.GetInternalArray();
                int[]? intArray = newCollection.GetInternalArray();

                if (intArray is not null)
                {
                    Span<int> collectionSpan = new(collectionArray, 0, collection.Count);
                    Span<int> arraySpan = new(intArray, 0, collection.Count);
                    collectionSpan.CopyTo(arraySpan);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is int[] collectionArray)
            {
                int[]? intArray = newCollection.GetInternalArray();

                if (intArray is not null)
                {
                    Span<int> collectionSpan = new(collectionArray, 0, collection.Count);
                    Span<int> arraySpan = new(intArray, 0, collection.Count);
                    collectionSpan.CopyTo(arraySpan);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
        }

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i]);
        }

        return newCollection;
    }

    public static HelixToolkit.IntCollection? ToIntCollection(this System.Windows.Media.Int32Collection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new HelixToolkit.IntCollection(0);
        }

        var newCollection = new HelixToolkit.IntCollection();
        newCollection.IncreaseCapacity(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            int[]? intArray = collection.GetInternalArray();
            int[] collectionArray = newCollection.GetInternalArray();

            if (intArray is not null)
            {
                Span<int> arraySpan = new(intArray, 0, collection.Count);
                Span<int> collectionSpan = new(collectionArray, 0, collection.Count);
                arraySpan.CopyTo(collectionSpan);
                return newCollection;
            }
        }

        newCollection.Clear(true);
        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i]);
        }

        return newCollection;
    }

    public static IList<float>? ToFloatCollection(this System.Windows.Media.DoubleCollection collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new List<float>(0);
        }

        var newCollection = new List<float>(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            double[]? doubleArray = collection.GetInternalArray();
            float[] floatArray = newCollection.GetInternalArray();

            if (doubleArray is not null)
            {
                FloatingPointArrayConverters.ConvertDoubleToFloat(collection.Count, doubleArray, floatArray);
                newCollection.SetInternalSize(collection.Count);
                return newCollection;
            }
        }

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add((float)collection[i]);
        }

        return newCollection;
    }

    public static System.Windows.Media.DoubleCollection? ToWndDoubleCollection(this IList<float>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        if (collection.Count == 0)
        {
            return new System.Windows.Media.DoubleCollection(0);
        }

        var newCollection = new System.Windows.Media.DoubleCollection(collection.Count);

        if (collection.Count > WpfCollectionExtensions.InternalArrayMinSize)
        {
            if (collection is HelixToolkit.FastList<float> fastList)
            {
                float[] floatArray = fastList.GetInternalArray();
                double[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is List<float> list)
            {
                float[] floatArray = list.GetInternalArray();
                double[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
            else if (collection is float[] floatArray)
            {
                double[]? doubleArray = newCollection.GetInternalArray();

                if (doubleArray is not null)
                {
                    FloatingPointArrayConverters.ConvertFloatToDouble(collection.Count, floatArray, doubleArray);
                    newCollection.SetInternalCount(collection.Count);
                    return newCollection;
                }
            }
        }

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add((double)collection[i]);
        }

        return newCollection;
    }

    public static System.Windows.Media.Media3D.MeshGeometry3D ToWndMeshGeometry3D(this HelixToolkit.Geometry.MeshGeometry3D mesh, bool freeze = false)
    {
        var mg = new System.Windows.Media.Media3D.MeshGeometry3D()
        {
            Normals = ToWndVector3DCollection(mesh.Normals),
            Positions = ToWndPoint3DCollection(mesh.Positions),
            TextureCoordinates = ToWndPointCollection(mesh.TextureCoordinates),
            TriangleIndices = ToWndInt32Collection(mesh.TriangleIndices)
        };

        if (freeze)
        {
            mg.Freeze();
        }

        return mg;
    }

    public static HelixToolkit.Geometry.MeshGeometry3D ToMeshGeometry3D(this System.Windows.Media.Media3D.MeshGeometry3D mesh)
    {
        return new HelixToolkit.Geometry.MeshGeometry3D()
        {
            Normals = mesh.Normals?.ToVector3Collection(),
            Positions = mesh.Positions?.ToVector3Collection() ?? new HelixToolkit.Vector3Collection(),
            TextureCoordinates = mesh.TextureCoordinates?.ToVector2Collection(),
            TriangleIndices = mesh.TriangleIndices.ToIntCollection() ?? new HelixToolkit.IntCollection()
        };
    }
    #endregion 
}
