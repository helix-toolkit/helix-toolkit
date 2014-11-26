// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StlExporer.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Exports the 3D visual tree to a STereoLithography binary file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Controls;

namespace HelixToolkit.Wpf
{
    using System.IO;
    using System.Text;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Exports the 3D visual tree to a STereoLithography binary file.
    /// </summary>
    public class StlExporter : Exporter<BinaryWriter>
    {
        protected override BinaryWriter Create(Stream stream)
        {
            return new BinaryWriter(stream, Encoding.UTF8, true);
        }

        protected override void Close(BinaryWriter writer)
        {
            writer.Dispose();
        }

        public override void Export(Viewport3D viewport, Stream stream)
        {
            var writer = Create(stream);

            int triangleIndicesCount = 0;
            viewport.Children.Traverse<GeometryModel3D>((m, t) => triangleIndicesCount += ((MeshGeometry3D)m.Geometry).TriangleIndices.Count);
            
            ExportHeader(writer, triangleIndicesCount / 3);
            viewport.Children.Traverse<GeometryModel3D>((m, t) => ExportModel(writer, m, t));

            Close(writer);
        }

        public override void Export(Visual3D visual, Stream stream)
        {
            var writer = Create(stream);

            int triangleIndicesCount = 0;
            visual.Traverse<GeometryModel3D>((m, t) => triangleIndicesCount += ((MeshGeometry3D)m.Geometry).TriangleIndices.Count);

            ExportHeader(writer, triangleIndicesCount / 3);
            visual.Traverse<GeometryModel3D>((m, t) => ExportModel(writer, m, t));

            Close(writer);
        }
        
        public override void Export(Model3D model, Stream stream)
        {
            var writer = Create(stream);

            int triangleIndicesCount = 0;
            model.Traverse<GeometryModel3D>((m, t) => triangleIndicesCount += ((MeshGeometry3D)m.Geometry).TriangleIndices.Count);

            ExportHeader(writer, triangleIndicesCount / 3);
            model.Traverse<GeometryModel3D>((m, t) => ExportModel(writer, m, t));

            Close(writer);
        }

        private void ExportHeader(BinaryWriter writer, int triangleCount)
        {
            ExportHeader(writer);
            writer.Write(triangleCount);
        }

        protected override void ExportHeader(BinaryWriter writer)
        {
            writer.Write(new byte[80]);
        }

        protected override void ExportModel(BinaryWriter writer, GeometryModel3D model, Transform3D t)
        {
            var mesh = (MeshGeometry3D) model.Geometry;

            var normals = mesh.Normals;
            if (normals == null || normals.Count != mesh.Positions.Count)
            {
                normals = MeshGeometryHelper.CalculateNormals(mesh);
            }

            // TODO: Also handle non-uniform scale
            var matrix = t.Clone().Value;
            matrix.OffsetX = 0;
            matrix.OffsetY = 0;
            matrix.OffsetZ = 0;
            var normalTransform = new MatrixTransform3D(matrix);

            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int i0 = mesh.TriangleIndices[i + 0];
                int i1 = mesh.TriangleIndices[i + 1];
                int i2 = mesh.TriangleIndices[i + 2];

                // Normal
                var faceNormal = normalTransform.Transform(normals[i0] + normals[i1] + normals[i2]);
                faceNormal.Normalize();
                WriteVector(writer, faceNormal);

                // Vertices
                WriteVertex(writer, t.Transform(mesh.Positions[i0]));
                WriteVertex(writer, t.Transform(mesh.Positions[i1]));
                WriteVertex(writer, t.Transform(mesh.Positions[i2]));

                // Attributes
                const ushort attribute = 0;
                writer.Write(attribute);
            }
        }

        private static void WriteVector(BinaryWriter writer, Vector3D normal)
        {
            writer.Write((float)normal.X);
            writer.Write((float)normal.Y);
            writer.Write((float)normal.Z);
        }

        private static void WriteVertex(BinaryWriter writer, Point3D p)
        {
            writer.Write((float)p.X);
            writer.Write((float)p.Y);
            writer.Write((float)p.Z);
        }
    }
}
