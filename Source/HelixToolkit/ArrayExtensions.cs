using CommunityToolkit.Diagnostics;

namespace HelixToolkit;

public static class ArrayExtensions
{
    public static TResult[,] Convert<TSource, TResult>(this TSource[,] array, Func<TSource, TResult>? conversion = null)
    {
        Guard.IsNotNull(array);

        if (conversion is null)
        {
            var resultType = typeof(TResult);
            conversion = source => (TResult)System.Convert.ChangeType(source, resultType)!;
        }

        var width = array.GetLength(1);
        var height = array.GetLength(0);
        var result = new TResult[height, width];

        for (int i = 0; i < height; ++i)
            for (int j = 0; j < width; ++j)
                result[i, j] = conversion(array[i, j]);

        return result;
    }
}
