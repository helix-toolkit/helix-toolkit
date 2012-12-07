// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LwoReader.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// LWO (Lightwave object) file reader
    /// </summary>
    /// <remarks>
    /// See http://www.martinreddy.net/gfx/3d/LWOB.txt
    /// http://www.modwiki.net/wiki/LWO_(file_format)
    /// http://www.newtek.com/lightwave/developers.php
    /// http://home.comcast.net/~erniew/lwsdk/docs/filefmts/lwo2.html
    /// </remarks>
    public class LwoReader : IModelReader
    {
        /// <summary>
        /// Gets the materials.
        /// </summary>
        /// <value>The materials.</value>
        public IList<Material> Materials { get; private set; }

        /// <summary>
        /// Gets the meshes.
        /// </summary>
        /// <value>The meshes.</value>
        public IList<MeshBuilder> Meshes { get; private set; }

        /// <summary>
        /// Gets the surfaces.
        /// </summary>
        /// <value>The surfaces.</value>
        public IList<string> Surfaces { get; private set; }

        /// <summary>
        /// Gets or sets the texture path.
        /// </summary>
        /// <value>The texture path.</value>
        public string TexturePath { get; set; }

        /// <summary>
        /// Gets or sets Points.
        /// </summary>
        private IList<Point3D> Points { get; set; }

        /// <summary>
        /// Reads the model from the specified path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// A Model3D.
        /// </returns>
        public Model3DGroup Read(string path)
        {
            var texturePath = Path.GetDirectoryName(path);
            using (var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this.TexturePath = texturePath;
                return this.Read(s);
            }
        }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">
        /// The stream.
        /// </param>
        /// <returns>
        /// A Model3D.
        /// </returns>
        public Model3DGroup Read(Stream s)
        {
            using (var reader = new BinaryReader(s))
            {
                long length = reader.BaseStream.Length;

                string headerID = this.ReadChunkId(reader);
                if (headerID != "FORM")
                {
                    throw new FileFormatException("Unknown file");
                }

                int headerSize = this.ReadChunkSize(reader);

                if (headerSize + 8 != length)
                {
                    throw new FileFormatException("Incomplete file (file length does not match header)");
                }

                string header2 = this.ReadChunkId(reader);
                if (header2 != "LWOB")
                {
                    throw new FileFormatException("Unknown file format (" + header2 + ").");
                }

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string id = this.ReadChunkId(reader);
                    int size = this.ReadChunkSize(reader);

                    switch (id)
                    {
                        case "PNTS":
                            this.ReadPoints(reader, size);
                            break;
                        case "SRFS":
                            this.ReadSurface(reader, size);
                            break;
                        case "POLS":
                            this.ReadPolygons(reader, size);
                            break;
                            // ReSharper disable RedundantCaseLabel
                        case "SURF":
                            // ReSharper restore RedundantCaseLabel
                        default:
                            // download the whole chunk
                            // ReSharper disable UnusedVariable
                            var data = this.ReadData(reader, size);
                            // ReSharper restore UnusedVariable
                            break;
                    }
                }

                return this.BuildModel();
            }
        }

        /// <summary>
        /// Builds the model.
        /// </summary>
        /// <returns>
        /// A Model3D.
        /// </returns>
        private Model3DGroup BuildModel()
        {
            var modelGroup = new Model3DGroup();
            int index = 0;
            foreach (var mesh in this.Meshes)
            {
                var gm = new GeometryModel3D { Geometry = mesh.ToMesh(), Material = this.Materials[index], BackMaterial = this.Materials[index] };
                modelGroup.Children.Add(gm);
                index++;
            }

            return modelGroup;
        }

        /// <summary>
        /// Read the chunk id.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// The chunk id.
        /// </returns>
        private string ReadChunkId(BinaryReader reader)
        {
            var chars = reader.ReadChars(4);
            return new string(chars);
        }

        /// <summary>
        /// Read the chunk size.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// The chunk size.
        /// </returns>
        private int ReadChunkSize(BinaryReader reader)
        {
            return this.ReadInt(reader);
        }

        /// <summary>
        /// Read the data block of a chunk.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="size">Excluding header size</param>
        /// <returns>
        /// The data.
        /// </returns>
        private byte[] ReadData(BinaryReader reader, int size)
        {
            return reader.ReadBytes(size);
        }

        /// <summary>
        /// Read big-endian float.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// The read float.
        /// </returns>
        private float ReadFloat(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            return BitConverter.ToSingle(new[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// Read big-endian int.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// The read int.
        /// </returns>
        private int ReadInt(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            return BitConverter.ToInt32(new[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// Read points.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="size">The size of the points array.</param>
        private void ReadPoints(BinaryReader reader, int size)
        {
            int n = size / 4 / 3;
            this.Points = new List<Point3D>(n);
            for (int i = 0; i < n; i++)
            {
                float x = this.ReadFloat(reader);
                float y = this.ReadFloat(reader);
                float z = this.ReadFloat(reader);
                this.Points.Add(new Point3D(x, y, z));
            }
        }

        /// <summary>
        /// Read polygons.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="size">The size.</param>
        private void ReadPolygons(BinaryReader reader, int size)
        {
            while (size > 0)
            {
                short nverts = this.ReadShortInt(reader);
                if (nverts <= 0)
                {
                    throw new NotSupportedException("details are not supported");
                }

                var pts = new List<Point3D>(nverts);
                for (int i = 0; i < nverts; i++)
                {
                    int vidx = this.ReadShortInt(reader);
                    pts.Add(this.Points[vidx]);
                }

                short surfaceIndex = this.ReadShortInt(reader);
                size -= (2 + nverts) * 2;

                this.Meshes[surfaceIndex - 1].AddTriangleFan(pts);
            }
        }

        /// <summary>
        /// Read big-endian short.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// The read short int.
        /// </returns>
        private short ReadShortInt(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(2);
            return BitConverter.ToInt16(new[] { bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// Read a string.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        /// The string.
        /// </returns>
        private string ReadString(BinaryReader reader, int size)
        {
            var bytes = reader.ReadBytes(size);
            var enc = new ASCIIEncoding();
            var s = enc.GetString(bytes);
            return s.Trim('\0');
        }

        /// <summary>
        /// Read a surface.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="size">The size.</param>
        private void ReadSurface(BinaryReader reader, int size)
        {
            this.Surfaces = new List<string>();
            this.Meshes = new List<MeshBuilder>();
            this.Materials = new List<Material>();

            string name = this.ReadString(reader, size);
            var names = name.Split('\0');
            for (int i = 0; i < names.Length; i++)
            {
                string n = names[i];
                this.Surfaces.Add(n);
                this.Meshes.Add(new MeshBuilder(false, false));
                this.Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));

                // If the length of the string (including the null) is odd, an extra null byte is added.
                // Then skip the next empty string.
                if ((n.Length + 1) % 2 == 1)
                {
                    i++;
                }
            }
        }

    }
}