// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LwoReader.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
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
        #region Constants and Fields

        /// <summary>
        /// The reader.
        /// </summary>
        private BinaryReader reader;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the materials.
        /// </summary>
        /// <value>The materials.</value>
        public IList<Material> Materials { get; private set; }

        /// <summary>
        ///   Gets the meshes.
        /// </summary>
        /// <value>The meshes.</value>
        public IList<MeshBuilder> Meshes { get; private set; }

        /// <summary>
        ///   Gets the surfaces.
        /// </summary>
        /// <value>The surfaces.</value>
        public IList<string> Surfaces { get; private set; }

        /// <summary>
        ///   Gets or sets the texture path.
        /// </summary>
        /// <value>The texture path.</value>
        public string TexturePath { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Points.
        /// </summary>
        private IList<Point3D> Points { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads the model from the specified path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// </returns>
        public Model3DGroup Read(string path)
        {
            var texturePath = Path.GetDirectoryName(path);
            var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.TexturePath = texturePath;
            var result = Read(s);
            s.Close();
            return result;
        }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// </returns>
        public Model3DGroup Read(Stream s)
        {
            this.reader = new BinaryReader(s);

            long length = this.reader.BaseStream.Length;

            string headerID = this.ReadChunkId();
            if (headerID != "FORM")
            {
                throw new FileFormatException("Unknown file");
            }

            int headerSize = this.ReadChunkSize();

            if (headerSize + 8 != length)
            {
                throw new FileFormatException("Incomplete file (file length does not match header)");
            }

            string header2 = this.ReadChunkId();
            if (header2 != "LWOB")
            {
                throw new FileFormatException("Unknown file format (" + header2 + ").");
            }

            while (this.reader.BaseStream.Position < this.reader.BaseStream.Length)
            {
                string id = this.ReadChunkId();
                int size = this.ReadChunkSize();

                switch (id)
                {
                    case "PNTS":
                        this.ReadPoints(size);
                        break;
                    case "SRFS":
                        this.ReadSurface(size);
                        break;
                    case "POLS":
                        this.ReadPolygons(size);
                        break;
                    case "SURF":
                    default:

                        // download the whole chunk
                        var bytes = this.ReadData(size);
                        break;
                }
            }

            this.reader.Close();

            return this.BuildModel();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The build model.
        /// </summary>
        /// <returns>
        /// </returns>
        private Model3DGroup BuildModel()
        {
            var modelGroup = new Model3DGroup();
            int index = 0;
            foreach (var mesh in this.Meshes)
            {
                var gm = new GeometryModel3D();
                gm.Geometry = mesh.ToMesh();
                gm.Material = this.Materials[index];
                gm.BackMaterial = gm.Material;
                modelGroup.Children.Add(gm);
                index++;
            }

            return modelGroup;
        }

        /// <summary>
        /// The read chunk id.
        /// </summary>
        /// <returns>
        /// The read chunk id.
        /// </returns>
        private string ReadChunkId()
        {
            var chars = this.reader.ReadChars(4);
            return new string(chars);
        }

        /// <summary>
        /// The read chunk size.
        /// </summary>
        /// <returns>
        /// The read chunk size.
        /// </returns>
        private int ReadChunkSize()
        {
            return this.ReadInt();
        }

        /// <summary>
        /// Read the data block of a chunk.
        /// </summary>
        /// <param name="size">
        /// Excluding header size
        /// </param>
        /// <returns>
        /// </returns>
        private byte[] ReadData(int size)
        {
            return this.reader.ReadBytes(size);
        }

        /// <summary>
        /// Read big-endian float.
        /// </summary>
        /// <returns>
        /// The read float.
        /// </returns>
        private float ReadFloat()
        {
            var bytes = this.reader.ReadBytes(4);
            return BitConverter.ToSingle(new[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// Read big-endian int.
        /// </summary>
        /// <returns>
        /// The read int.
        /// </returns>
        private int ReadInt()
        {
            var bytes = this.reader.ReadBytes(4);
            return BitConverter.ToInt32(new[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// The read points.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        private void ReadPoints(int size)
        {
            int nPoints = size / 4 / 3;
            this.Points = new List<Point3D>(nPoints);
            for (int i = 0; i < nPoints; i++)
            {
                float x = this.ReadFloat();
                float y = this.ReadFloat();
                float z = this.ReadFloat();
                this.Points.Add(new Point3D(x, y, z));
            }
        }

        /// <summary>
        /// The read polygons.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private void ReadPolygons(int size)
        {
            while (size > 0)
            {
                short nverts = this.ReadShortInt();
                if (nverts <= 0)
                {
                    throw new NotSupportedException("details are not supported");
                }

                var pts = new List<Point3D>(nverts);
                for (int i = 0; i < nverts; i++)
                {
                    int vidx = this.ReadShortInt();
                    pts.Add(this.Points[vidx]);
                }

                short surfaceIndex = this.ReadShortInt();
                size -= (2 + nverts) * 2;

                this.Meshes[surfaceIndex - 1].AddTriangleFan(pts);
            }
        }

        /// <summary>
        /// Read big-endian short.
        /// </summary>
        /// <returns>
        /// The read short int.
        /// </returns>
        private short ReadShortInt()
        {
            var bytes = this.reader.ReadBytes(2);
            return BitConverter.ToInt16(new[] { bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// The read string.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The read string.
        /// </returns>
        private string ReadString(int size)
        {
            var bytes = this.reader.ReadBytes(size);
            var enc = new ASCIIEncoding();
            var s = enc.GetString(bytes);
            return s.Trim('\0');
        }

        /// <summary>
        /// The read surface.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        private void ReadSurface(int size)
        {
            this.Surfaces = new List<string>();
            this.Meshes = new List<MeshBuilder>();
            this.Materials = new List<Material>();

            string name = this.ReadString(size);
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

        #endregion
    }
}