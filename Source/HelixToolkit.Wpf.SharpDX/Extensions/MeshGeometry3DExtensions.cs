namespace HelixToolkit.Wpf.SharpDX
{
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Media3D = System.Windows.Media.Media3D;
    using global::SharpDX;
    /// <summary>
    /// Provider extension functions for MeshGeometry3D
    /// </summary>
    public static class MeshGeometry3DExtensions
    {
        /// <summary>
        /// Cuts the mesh with the specified plane. (Modified from HelixToolkit.Wpf version)
        /// </summary>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        /// <param name="plane">
        /// The plane origin.
        /// </param>
        /// <param name="normal">
        /// The plane normal.
        /// </param>
        /// <returns>
        /// The <see cref="MeshGeometry3D"/>.
        /// </returns>
        public static MeshGeometry3D CutMesh(this MeshGeometry3D mesh, Media3D.Point3D plane, Vector3 normal)
        {
            var hasTextureCoordinates = mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count > 0;
            var hasNormals = mesh.Normals != null && mesh.Normals.Count > 0;
            var meshBuilder = new MeshBuilder(hasNormals, hasTextureCoordinates);
            var contourHelper = new HelixToolkit.Wpf.SharpDX.Helpers.ContourHelperDX(plane, normal, mesh);
            foreach (var position in mesh.Positions)
            {
                meshBuilder.Positions.Add(position);
            }

            if (hasTextureCoordinates)
            {
                foreach (var textureCoordinate in mesh.TextureCoordinates)
                {
                    meshBuilder.TextureCoordinates.Add(textureCoordinate);
                }
            }

            if (hasNormals)
            {
                foreach (var n in mesh.Normals)
                {
                    meshBuilder.Normals.Add(n);
                }
            }

            for (var i = 0; i < mesh.Indices.Count; i += 3)
            {
                var index0 = mesh.Indices[i];
                var index1 = mesh.Indices[i + 1];
                var index2 = mesh.Indices[i + 2];

                Vector3[] positions;
                Vector3[] normals;
                Vector2[] textureCoordinates;
                int[] triangleIndices;

                contourHelper.ContourFacet(index0, index1, index2, out positions, out normals, out textureCoordinates, out triangleIndices);

                foreach (var p in positions)
                {
                    meshBuilder.Positions.Add(p);
                }

                foreach (var tc in textureCoordinates)
                {
                    meshBuilder.TextureCoordinates.Add(tc);
                }

                foreach (var n in normals)
                {
                    meshBuilder.Normals.Add(n);
                }

                foreach (var ti in triangleIndices)
                {
                    meshBuilder.TriangleIndices.Add(ti);
                }
            }

            return meshBuilder.ToMeshGeometry3D();
        }
    }
}
