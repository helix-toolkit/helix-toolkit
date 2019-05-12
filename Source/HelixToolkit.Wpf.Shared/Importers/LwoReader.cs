// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LwoReader.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   LWO (Lightwave object) file reader
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;

    /// <summary>
    /// LWO (Lightwave object) file reader
    /// </summary>
    /// <remarks>
    /// LWO2 is currently not supported.
    /// </remarks>
    public class LwoReader : ModelReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LwoReader" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public LwoReader(Dispatcher dispatcher = null) :
            base(dispatcher)
        {
            // http://www.martinreddy.net/gfx/3d/LWOB.txt
            // http://www.modwiki.net/wiki/LWO_(file_format)
            // http://www.wotsit.org/list.asp?fc=2
            // https://www.lightwave3d.com/lightwave_sdk/
            // http://home.comcast.net/~erniew/lwsdk/docs/filefmts/lwo2.html (TODO!)
        }

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
        /// Gets or sets Points.
        /// </summary>
        private IList<Point3D> Points { get; set; }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <returns>A Model3D.</returns>
        public override Model3DGroup Read(Stream s)
        {
            using (var reader = new BinaryReader(s))
            {
                long length = reader.BaseStream.Length;

                string headerId = this.ReadChunkId(reader);
                if (headerId != "FORM")
                {
                    throw new FileFormatException("Unknown file");
                }

                int headerSize = this.ReadChunkSize(reader);

                if (headerSize + 8 != length)
                {
                    throw new FileFormatException("Incomplete file (file length does not match header)");
                }

                string header2 = this.ReadChunkId(reader);
                switch (header2)
                {
                    case "LWOB":
                        break;
                    case "LWO2":
                        throw new FileFormatException("LWO2 is not yet supported.");
                    default:
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
        /// <returns>A Model3D.</returns>
        private Model3DGroup BuildModel()
        {
            Model3DGroup modelGroup = null;
            this.Dispatch(
                () =>
                {
                    modelGroup = new Model3DGroup();
                    int index = 0;
                    foreach (var mesh in this.Meshes)
                    {
                        var gm = new GeometryModel3D
                                     {
                                         Geometry = mesh.ToMesh(),
                                         Material = this.Materials[index],
                                         BackMaterial = this.Materials[index]
                                     };
                        if (this.Freeze)
                        {
                            gm.Freeze();
                        }

                        modelGroup.Children.Add(gm);
                        index++;
                    }

                    if (this.Freeze)
                    {
                        modelGroup.Freeze();
                    }
                });
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
        /// Reads the data block of a chunk.
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
        /// Reads a big-endian float.
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
        /// Reads a big-endian integer.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// The integer.
        /// </returns>
        private int ReadInt(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            return BitConverter.ToInt32(new[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// Reads points.
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
        /// Reads polygons.
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
        /// Reads a big-endian short.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// The short integer.
        /// </returns>
        private short ReadShortInt(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(2);
            return BitConverter.ToInt16(new[] { bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// Reads a string.
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
                this.Materials.Add(this.DefaultMaterial);

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