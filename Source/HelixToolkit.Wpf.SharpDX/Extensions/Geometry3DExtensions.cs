using HelixToolkit.SharpDX;

namespace HelixToolkit.Wpf.SharpDX;

/// <summary>
/// Contains extension methods for geometry.
/// </summary>
public static class Geometry3DExtensions
{
    /// <summary>
    /// Returns a copy of this <see cref="Geometry3D"/> with unshared vertices. 
    /// Note that <see cref="Geometry3D.Indices"/> could be set to null if unindexed drawing was supported.
    /// </summary>
    /// <typeparam name="T">The concrete type of this <see cref="Geometry3D"/>.</typeparam>
    /// <param name="source">This respective <see cref="Geometry3D"/>.</param>
    /// <returns>A copy of this <see cref="Geometry3D"/> with unshared vertices.</returns>
    public static T ToUnshared<T>(this T source) where T : Geometry3D, new()
    {
        var result = new T
        {
            Indices = new IntCollection(source.Indices?.Count ?? 0),
            Positions = new Vector3Collection(source.Indices?.Count ?? 0)
        };

        if (source.Colors != null && source.Positions is not null && source.Colors.Count == source.Positions.Count)
        {
            result.Colors = new Color4Collection(source.Indices?.Count ?? 0);

            if (source.Indices is not null)
            {
                for (var i = 0; i < source.Indices.Count; i++)
                {
                    result.Colors.Add(source.Colors[source.Indices[i]]);
                    result.Positions.Add(source.Positions[source.Indices[i]]);
                    result.Indices.Add(i);
                }
            }
        }
        else
        {
            if (source.Indices is not null && source.Positions is not null)
            {
                for (var i = 0; i < source.Indices.Count; i++)
                {
                    result.Positions.Add(source.Positions[source.Indices[i]]);
                    result.Indices.Add(i);
                }
            }
        }

        return result;
    }
}
