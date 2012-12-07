// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StLReader.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A StereoLithography .StL file reader.
    /// </summary>
    public class StLReader : IModelReader
    {
        /// <summary>
        /// The normal regex.
        /// </summary>
        private readonly Regex normalRegex = new Regex(@"normal\s*(\S*)\s*(\S*)\s*(\S*)", RegexOptions.Compiled);

        /// <summary>
        /// The vertex regex.
        /// </summary>
        private readonly Regex vertexRegex = new Regex(@"vertex\s*(\S*)\s*(\S*)\s*(\S*)", RegexOptions.Compiled);

        /// <summary>
        /// The index.
        /// </summary>
        private int index;

        /// <summary>
        /// The last.
        /// </summary>
        private Color last;

        /// <summary>
        /// Initializes a new instance of the <see cref="StLReader"/> class.
        /// </summary>
        public StLReader()
        {
            this.Meshes = new List<MeshBuilder>();
            this.Materials = new List<Material>();
        }

        /// <summary>
        /// Gets the materials.
        /// </summary>
        /// <value> The materials. </value>
        public IList<Material> Materials { get; private set; }

        /// <summary>
        /// Gets the meshes.
        /// </summary>
        /// <value> The meshes. </value>
        public IList<MeshBuilder> Meshes { get; private set; }

        /// <summary>
        /// Gets or sets the ascii reader.
        /// </summary>
        /// <value> The ascii reader. </value>
        private StreamReader AsciiReader { get; set; }

        /// <summary>
        /// Gets or sets binaryReader.
        /// </summary>
        private BinaryReader BinaryReader { get; set; }

        /// <summary>
        /// Reads the model from the specified path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The model.
        /// </returns>
        public Model3DGroup Read(string path)
        {
            Model3DGroup result;
            using (var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                result = this.Read(s);
            }

            return result;
        }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">
        /// The stream.
        /// </param>
        /// <returns>
        /// The model.
        /// </returns>
        public Model3DGroup Read(Stream s)
        {
            this.BinaryReader = new BinaryReader(s);

            long length = this.BinaryReader.BaseStream.Length;

            if (length < 84)
            {
                throw new FileFormatException("Incomplete file");
            }

            string header = this.ReadHeaderB();
            uint numberTriangles = this.ReadNumberTrianglesB();

            s.Position = 0;

            if (length - 84 != numberTriangles * 50)
            {
                this.ReadA(s);
            }
            else
            {
                this.ReadB(s);
            }

            return this.BuildModel();
        }

        /// <summary>
        /// Reads ascii.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        public void ReadA(Stream s)
        {
            this.AsciiReader = new StreamReader(s);

            this.Meshes.Add(new MeshBuilder(true, true));
            this.Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));

            while (!this.AsciiReader.EndOfStream)
            {
                var line = this.AsciiReader.ReadLine().Trim();
                if (line.Length == 0 || line.StartsWith("\0") || line.StartsWith("#") || line.StartsWith("!")
                    || line.StartsWith("$"))
                {
                    continue;
                }

                string id, values;
                SplitLine(line, out id, out values);
                switch (id)
                {
                    case "solid":
                        break;
                    case "facet":
                        this.ReadFacetA(values);
                        break;
                    case "endsolid":
                        break;
                }
            }

            this.AsciiReader.Close();
        }

        /// <summary>
        /// Reads a binary stream.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        public void ReadB(Stream s)
        {
            long length = this.BinaryReader.BaseStream.Length;

            if (length < 84)
            {
                throw new FileFormatException("Incomplete file");
            }

            string header = this.ReadHeaderB();
            uint numberTriangles = this.ReadNumberTrianglesB();

            this.index = 0;
            this.Meshes.Add(new MeshBuilder(true, true));
            this.Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));

            for (int i = 0; i < numberTriangles; i++)
            {
                this.ReadTriangleB();
            }

            this.BinaryReader.Close();
        }

        /// <summary>
        /// The split line.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        private static void SplitLine(string line, out string id, out string values)
        {
            int idx = line.IndexOf(' ');
            if (idx == -1)
            {
                id = line;
                values = string.Empty;
            }
            else
            {
                id = line.Substring(0, idx).ToLower();
                values = line.Substring(idx + 1);
            }
        }

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
        /// The parse normal a.
        /// </summary>
        /// <param name="normal">
        /// The normal.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="FileFormatException">
        /// </exception>
        private Vector3D ParseNormalA(string normal)
        {
            var match = this.normalRegex.Match(normal);
            if (!match.Success)
            {
                throw new FileFormatException("Unexpected line.");
            }

            double x = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            double y = double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            double z = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// The read triangle a.
        /// </summary>
        /// <param name="normal">
        /// The normal.
        /// </param>
        private void ReadFacetA(string normal)
        {
            Vector3D n = this.ParseNormalA(normal);
            var points = new List<Point3D>();
            this.ReadLineA("outer");
            while (true)
            {
                var line = this.ReadLineA();
                Point3D point;
                if (this.TryParseVertex(line, out point))
                {
                    points.Add(point);
                    continue;
                }

                string id, values;
                SplitLine(line, out id, out values);

                if (id == "endloop")
                {
                    break;
                }
            }

            this.ReadLineA("endfacet");

            if (this.Materials.Count < this.index + 1)
            {
                this.Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));
            }

            if (this.Meshes.Count < this.index + 1)
            {
                this.Meshes.Add(new MeshBuilder(true, true));
            }

            this.Meshes[this.index].AddPolygon(points);

            // todo: add normals
        }

        /// <summary>
        /// Read float (4 byte)
        /// </summary>
        /// <returns>
        /// The read float b.
        /// </returns>
        private float ReadFloatB()
        {
            var bytes = this.BinaryReader.ReadBytes(4);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// The read header b.
        /// </summary>
        /// <returns>
        /// The read header b.
        /// </returns>
        private string ReadHeaderB()
        {
            var chars = this.BinaryReader.ReadChars(80);
            return new string(chars);
        }

        /// <summary>
        /// The read line a.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <exception cref="FileFormatException">
        /// </exception>
        private void ReadLineA(string token)
        {
            var line = this.AsciiReader.ReadLine().Trim();
            int idx = line.IndexOf(' ');
            string id, values;
            SplitLine(line, out id, out values);

            if (!string.Equals(token, id, StringComparison.OrdinalIgnoreCase))
            {
                throw new FileFormatException("Unexpected line.");
            }
        }

        /// <summary>
        /// Reads a line from the asciiReader.
        /// </summary>
        /// <returns>
        /// The line
        /// </returns>
        private string ReadLineA()
        {
            var line = this.AsciiReader.ReadLine();
            if (line != null)
            {
                line = line.Trim();
            }

            return line;
        }

        /// <summary>
        /// The read number triangles b.
        /// </summary>
        /// <returns>
        /// The read number triangles b.
        /// </returns>
        private uint ReadNumberTrianglesB()
        {
            return this.ReadUInt32B();
        }

        /// <summary>
        /// The read triangle b.
        /// </summary>
        private void ReadTriangleB()
        {
            Color current;
            bool hasColor = false;
            bool sameColor = true;

            float ni = this.ReadFloatB();
            float nj = this.ReadFloatB();
            float nk = this.ReadFloatB();
            var n = new Vector3D(ni, nj, nk);

            float v1x = this.ReadFloatB();
            float v1y = this.ReadFloatB();
            float v1z = this.ReadFloatB();
            var v1 = new Point3D(v1x, v1y, v1z);

            float v2x = this.ReadFloatB();
            float v2y = this.ReadFloatB();
            float v2z = this.ReadFloatB();
            var v2 = new Point3D(v2x, v2y, v2z);

            float v3x = this.ReadFloatB();
            float v3y = this.ReadFloatB();
            float v3z = this.ReadFloatB();
            var v3 = new Point3D(v3x, v3y, v3z);

            // UInt16 attrib = ReadUInt16();
            var attrib = Convert.ToString(this.ReadUInt16B(), 2).PadLeft(16, '0').ToCharArray();
            hasColor = attrib[0].Equals('1');

            if (hasColor)
            {
                int blue = attrib[15].Equals('1') ? 1 : 0;
                blue = attrib[14].Equals('1') ? blue + 2 : blue;
                blue = attrib[13].Equals('1') ? blue + 4 : blue;
                blue = attrib[12].Equals('1') ? blue + 8 : blue;
                blue = attrib[11].Equals('1') ? blue + 16 : blue;
                int b = blue * 8;

                int green = attrib[10].Equals('1') ? 1 : 0;
                green = attrib[9].Equals('1') ? green + 2 : green;
                green = attrib[8].Equals('1') ? green + 4 : green;
                green = attrib[7].Equals('1') ? green + 8 : green;
                green = attrib[6].Equals('1') ? green + 16 : green;
                int g = green * 8;

                int red = attrib[5].Equals('1') ? 1 : 0;
                red = attrib[4].Equals('1') ? red + 2 : red;
                red = attrib[3].Equals('1') ? red + 4 : red;
                red = attrib[2].Equals('1') ? red + 8 : red;
                red = attrib[1].Equals('1') ? red + 16 : red;
                int r = red * 8;

                current = Color.FromRgb(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
                sameColor = Color.Equals(this.last, current);

                if (!sameColor)
                {
                    this.last = current;
                    this.index++;
                }

                if (this.Materials.Count < this.index + 1)
                {
                    this.Materials.Add(MaterialHelper.CreateMaterial(current));
                }
            }
            else
            {
                if (this.Materials.Count < this.index + 1)
                {
                    this.Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));
                }
            }

            if (this.Meshes.Count < this.index + 1)
            {
                this.Meshes.Add(new MeshBuilder(true, true));
            }

            this.Meshes[this.index].AddTriangle(v1, v2, v3);
        }

        /// <summary>
        /// Read UInt16.
        /// </summary>
        /// <returns>
        /// The read u int 16 b.
        /// </returns>
        private ushort ReadUInt16B()
        {
            var bytes = this.BinaryReader.ReadBytes(2);
            return BitConverter.ToUInt16(bytes, 0);
        }

        /// <summary>
        /// Read UInt32.
        /// </summary>
        /// <returns>
        /// The read u int 32 b.
        /// </returns>
        private uint ReadUInt32B()
        {
            var bytes = this.BinaryReader.ReadBytes(4);
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Tries to parse a vertex from a string.
        /// </summary>
        /// <param name="line">
        /// The input string.
        /// </param>
        /// <param name="point">
        /// The vertex point.
        /// </param>
        /// <returns>
        /// True if parsing was successful.
        /// </returns>
        private bool TryParseVertex(string line, out Point3D point)
        {
            var match = this.vertexRegex.Match(line);
            if (!match.Success)
            {
                point = new Point3D();
                return false;
            }

            double x = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            double y = double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            double z = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

            point = new Point3D(x, y, z);
            return true;
        }

    }
}