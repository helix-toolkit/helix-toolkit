using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

internal static class WpfCollectionExtensions
{
    /// <summary>
    /// Minimal size of internal ArrayItemList.
    /// </summary>
    /// <remarks>
    /// See https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/Shared/MS/Utility/FrugalList.cs
    /// When the capacity is 1 the store type is SingleItemList.
    /// When the capacity is between 2 and 3 the store type is ThreeItemList.
    /// When the capacity is between 4 and 6 the store type is SixItemList.
    /// When the capacity is greater than 6 the store type is ArrayItemList.
    /// </remarks>
    public const int InternalArrayMinSize = 6;

    public static Point3D[]? GetInternalArray(this Point3DCollection list)
    {
        return GetCollectionInternalArray<Point3DCollection, Point3D>(list);
    }

    public static void SetInternalCount(this Point3DCollection list, int count)
    {
        SetCollectionInternalCount(list, count);
    }

    public static Vector3D[]? GetInternalArray(this Vector3DCollection list)
    {
        return GetCollectionInternalArray<Vector3DCollection, Vector3D>(list);
    }

    public static void SetInternalCount(this Vector3DCollection list, int count)
    {
        SetCollectionInternalCount(list, count);
    }

    public static Vector[]? GetInternalArray(this VectorCollection list)
    {
        return GetCollectionInternalArray<VectorCollection, Vector>(list);
    }

    public static void SetInternalCount(this VectorCollection list, int count)
    {
        SetCollectionInternalCount(list, count);
    }

    public static Point[]? GetInternalArray(this PointCollection list)
    {
        return GetCollectionInternalArray<PointCollection, Point>(list);
    }

    public static void SetInternalCount(this PointCollection list, int count)
    {
        SetCollectionInternalCount(list, count);
    }

    public static int[]? GetInternalArray(this Int32Collection list)
    {
        return GetCollectionInternalArray<Int32Collection, int>(list);
    }

    public static void SetInternalCount(this Int32Collection list, int count)
    {
        SetCollectionInternalCount(list, count);
    }

    public static double[]? GetInternalArray(this DoubleCollection list)
    {
        return GetCollectionInternalArray<DoubleCollection, double>(list);
    }

    public static void SetInternalCount(this DoubleCollection list, int count)
    {
        SetCollectionInternalCount(list, count);
    }

    private static TItem[]? GetCollectionInternalArray<TCollection, TItem>(TCollection list)
    {
        object? collection = list!.GetType().GetField("_collection", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(list);

        if (collection is null)
        {
            return null;
        }

        object? listStore = collection.GetType().GetField("_listStore", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(collection);

        if (listStore is null)
        {
            return null;
        }

        object? items = listStore.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(listStore);

        if (items is null)
        {
            return null;
        }

        TItem[]? array = items as TItem[];

        if (array is null)
        {
            return null;
        }

        return array;
    }

    private static bool SetCollectionInternalCount<TCollection>(TCollection list, int count)
    {
        object? collection = list!.GetType().GetField("_collection", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(list);

        if (collection is null)
        {
            return false;
        }

        object? listStore = collection.GetType().GetField("_listStore", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(collection);

        if (listStore is null)
        {
            return false;
        }

        FieldInfo? countField = listStore.GetType().GetField("_count", BindingFlags.NonPublic | BindingFlags.Instance);

        if (countField is null)
        {
            return false;
        }

        countField.SetValue(listStore, count);
        return true;
    }
}
