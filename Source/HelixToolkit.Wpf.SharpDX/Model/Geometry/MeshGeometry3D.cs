namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::SharpDX;

    using Point2D = global::SharpDX.Vector2;
    using Point3D = global::SharpDX.Vector3;

    [Serializable]
    public class MeshGeometry3D : Geometry3D
    {
        public Point3D[] Normals { get; set; }
        public Point2D[] TextureCoordinates { get; set; }

        public Point3D[] Tangents { get; set; }
        public Point3D[] BiTangents { get; set; }

        public IEnumerable<Geometry3D.Triangle> Triangles
        {
            get
            {
                for (int i = 0; i < Indices.Length; i += 3)
                {
                    yield return new Triangle() { P0 = Positions[Indices[i]], P1 = Positions[Indices[i + 1]], P2 = Positions[Indices[i + 2]], };
                }
            }
        }

        public static MeshGeometry3D Merge(params MeshGeometry3D[] meshes)
        {
            var positions = new List<Point3D>();
            var indices = new List<int>();

            var normals = meshes.All(x => x.Normals != null) ? new List<Point3D>() : null;
            var colors = meshes.All(x => x.Colors != null) ? new List<Color4>() : null;
            var textureCoods = meshes.All(x => x.TextureCoordinates != null) ? new List<Point2D>() : null;
            var tangents = meshes.All(x => x.Tangents != null) ? new List<Point3D>() : null;
            var bitangents = meshes.All(x => x.BiTangents != null) ? new List<Point3D>() : null;

            int index = 0;
            foreach (var part in meshes)
            {
                for (int i = 0; i < part.Positions.Length; i++)
                {
                    positions.Add(part.Positions[i]);
                }

                for (int i = 0; i < part.Indices.Length; i++)
                {
                    indices.Add(index + part.Indices[i]);
                }

                index += part.Indices.Length;
            }
            if (normals != null)
            {
                normals = meshes.SelectMany(x => x.Normals).ToList();
            }
            if (colors != null)
            {
                colors = meshes.SelectMany(x => x.Colors).ToList();
            }
            if (textureCoods != null)
            {
                textureCoods = meshes.SelectMany(x => x.TextureCoordinates).ToList();
            }
            if (tangents != null)
            {
                tangents = meshes.SelectMany(x => x.Tangents).ToList();
            }
            if (bitangents != null)
            {
                bitangents = meshes.SelectMany(x => x.BiTangents).ToList();
            }

            var mesh = new MeshGeometry3D()
            {
                Positions = positions.ToArray(),
                Indices = indices.ToArray(),
            };
            mesh.Normals = normals != null ? normals.ToArray() : null;
            mesh.Colors = colors != null ? colors.ToArray() : null;
            mesh.TextureCoordinates = textureCoods != null ? textureCoods.ToArray() : null;
            mesh.Tangents = tangents != null ? tangents.ToArray() : null;
            mesh.BiTangents = bitangents != null ? bitangents.ToArray() : null;

            return mesh;
        }
    }
}