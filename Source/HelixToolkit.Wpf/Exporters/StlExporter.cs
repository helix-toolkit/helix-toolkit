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
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Exports the 3D visual tree to a STereoLithography binary file.
    /// </summary>
    public class StlExporter : Exporter<BinaryWriter>
    {
        /// <summary>
        /// Creates a <see cref="BinaryWriter"/> used to write the StlFile
        /// </summary>
        /// <param name="stream">The output stream where the STL is written.</param>
        /// <returns>The created <see cref="BinaryWriter"/></returns>
        protected override BinaryWriter Create(Stream stream)
        {
            return new BinaryWriter(stream);
        }

        /// <summary>
        /// Closes a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The writer to close</param>
        protected override void Close(BinaryWriter writer)
        {
        }

        /// <summary>
        /// Exports the specified viewport.
        /// </summary>
        /// <param name="viewport">The viewport to export</param>
        /// <param name="stream">The output stream to export to</param>
        public override void Export(Viewport3D viewport, Stream stream)
        {
            var writer = Create(stream);

            int triangleIndicesCount = 0;
            viewport.Children.Traverse<GeometryModel3D>((m, t) => triangleIndicesCount += ((MeshGeometry3D)m.Geometry).TriangleIndices.Count);
            
            ExportHeader(writer, triangleIndicesCount / 3);
            viewport.Children.Traverse<GeometryModel3D>((m, t) => ExportModel(writer, m, t));

            Close(writer);
        }

        /// <summary>
        /// Exports specified <see cref="Visual3D"/>
        /// </summary>
        /// <param name="visual">The <see cref="Visual3D"/> to export.</param>
        /// <param name="stream">The output stream to export to</param>
        public override void Export(Visual3D visual, Stream stream)
        {
            var writer = Create(stream);

            int triangleIndicesCount = 0;
            visual.Traverse<GeometryModel3D>((m, t) => triangleIndicesCount += ((MeshGeometry3D)m.Geometry).TriangleIndices.Count);

            ExportHeader(writer, triangleIndicesCount / 3);
            visual.Traverse<GeometryModel3D>((m, t) => ExportModel(writer, m, t));

            Close(writer);
        }
        
        /// <summary>
        /// Exports specified <see cref="Model3D"/>.
        /// </summary>
        /// <param name="model">The <see cref="Model3D"/> to export</param>
        /// <param name="stream">The output stream to export to</param>
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

        /// <summary>
        /// Writes an empty STL header.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write to.</param>
        protected override void ExportHeader(BinaryWriter writer)
        {
            writer.Write(new byte[80]);
        }

        /// <summary>
        /// Writes a <see cref="GeometryModel3D"/> to a <see cref="BinaryWriter"/> in STL binary format.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write to.</param>
        /// <param name="model">The model to write.</param>
        /// <param name="t">All vertices are transformed with this transform before written</param>
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

            var material = model.Material;
            var dm = material as DiffuseMaterial;

            var mg = material as MaterialGroup;
            if (mg != null)
            {
                foreach (var m in mg.Children)
                {
                    if (m is DiffuseMaterial)
                    {
                        dm = m as DiffuseMaterial;
                    }
                }
            }

            ushort attribute = 0;

            if (dm != null)
            {
                var scb = dm.Brush as SolidColorBrush;
                if (scb != null)
                {
                    byte r = scb.Color.R;
                    byte g = scb.Color.G;
                    byte b = scb.Color.B;
                    attribute = (ushort)((1 << 15) | ((r >> 3) << 10) | ((g >> 3) << 5) | (b >> 3));
                }
            }

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
