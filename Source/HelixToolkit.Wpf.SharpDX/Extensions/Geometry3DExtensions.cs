// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Geometry3DExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
// </copyright>
// <summary>
// Contains extension methods for geometry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using Core;

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
            var result = new T();
            result.Indices = new IntCollection(source.Indices.Count);
            result.Positions = new Vector3Collection(source.Indices.Count);
            if (source.Colors != null && source.Colors.Count == source.Positions.Count)
            {
                result.Colors = new Color4Collection(source.Indices.Count);
                for (var i = 0; i < source.Indices.Count; i++)
                {
                    result.Colors.Add(source.Colors[source.Indices[i]]);
                    result.Positions.Add(source.Positions[source.Indices[i]]);
                    result.Indices.Add(i);
                }
            }
            else
            {
                for (var i = 0; i < source.Indices.Count; i++)
                {
                    result.Positions.Add(source.Positions[source.Indices[i]]);
                    result.Indices.Add(i);
                }
            }

            return result;
        }
    }
}