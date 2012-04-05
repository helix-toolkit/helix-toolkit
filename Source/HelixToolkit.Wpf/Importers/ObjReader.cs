// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjReader.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A Wavefront .obj file reader.
    /// </summary>
    /// <remarks>
    /// See the file format specifications at
    /// http://en.wikipedia.org/wiki/Obj
    /// http://www.martinreddy.net/gfx/3d/OBJ.spec
    /// http://www.eg-models.de/formats/Format_Obj.html
    /// </remarks>
    public class ObjReader : IModelReader
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ObjReader" /> class.
        /// </summary>
        public ObjReader()
        {
            this.Points = new List<Point3D>();
            this.TexCoords = new List<Point>();
            this.Normals = new List<Vector3D>();

            this.Groups = new List<Group>();
            this.Materials = new Dictionary<string, MaterialDefinition>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public IList<Group> Groups { get; private set; }

        /// <summary>
        ///   Gets the materials.
        /// </summary>
        /// <value>The materials.</value>
        public Dictionary<string, MaterialDefinition> Materials { get; private set; }

        /// <summary>
        ///   Gets or sets the texture path.
        /// </summary>
        /// <value>The texture path.</value>
        public string TexturePath { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets CurrentGroup.
        /// </summary>
        private Group CurrentGroup
        {
            get
            {
                if (this.Groups.Count == 0)
                {
                    this.Groups.Add(new Group("default"));
                }

                return this.Groups[this.Groups.Count - 1];
            }
        }

        /// <summary>
        /// Gets or sets Normals.
        /// </summary>
        private IList<Vector3D> Normals { get; set; }

        /// <summary>
        /// Gets or sets Points.
        /// </summary>
        private IList<Point3D> Points { get; set; }

        /// <summary>
        /// Gets or sets Reader.
        /// </summary>
        private StreamReader Reader { get; set; }

        /// <summary>
        /// Gets or sets TexCoords.
        /// </summary>
        private IList<Point> TexCoords { get; set; }

        #endregion

        #region Public Methods

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
            this.TexturePath = Path.GetDirectoryName(path);
            var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = this.Read(s);
            s.Close();
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
            using (this.Reader = new StreamReader(s))
            {
                while (!this.Reader.EndOfStream)
                {
                    var line = this.Reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    line = line.Trim();
                    if (line.StartsWith("#") || line.Length == 0)
                    {
                        continue;
                    }

                    string id, values;
                    SplitLine(line, out id, out values);

                    switch (id.ToLower())
                    {
                        case "v":
                            this.AddVertex(values);
                            break;
                        case "vt":
                            this.AddTexCoord(values);
                            break;
                        case "vn":
                            this.AddNormal(values);
                            break;
                        case "f":
                            this.AddFace(values);
                            break;
                        case "g":
                            this.Groups.Add(new Group(values));
                            break;
                        case "mtllib":
                            this.LoadMaterialLib(values);
                            break;
                        case "usemtl":
                            this.SetMaterial(values);
                            break;
                        case "input":
                            this.SetSmoothing(values);
                            break;
                        case "o":
                            break;
                    }
                }
            }

            return this.BuildModel();
        }

        /// <summary>
        /// Reads a GZipStream compressed OBJ file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// A Model3D object containing the model.
        /// </returns>
        public Model3DGroup ReadZ(string path)
        {
            this.TexturePath = Path.GetDirectoryName(path);
            var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var deflateStream = new GZipStream(s, CompressionMode.Decompress, true);
            var result = this.Read(deflateStream);
            deflateStream.Close();
            s.Close();
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses a color string.
        /// </summary>
        /// <param name="values">
        /// The input.
        /// </param>
        /// <returns>
        /// The parsed color.
        /// </returns>
        private static Color ColorParse(string values)
        {
            var fields = Split(values);
            return Color.FromRgb((byte)(fields[0] * 255), (byte)(fields[1] * 255), (byte)(fields[2] * 255));
        }

        /// <summary>
        /// Parse a string containing a double value.
        /// </summary>
        /// <param name="input">
        /// The input string.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        private static double DoubleParse(string input)
        {
            return double.Parse(input, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Splits the specified string using whitespace(input) as separators.
        /// </summary>
        /// <param name="input">
        /// The input string.
        /// </param>
        /// <returns>
        /// List of input.
        /// </returns>
        private static IList<double> Split(string input)
        {
            input = input.Trim();
            var fields = input.SplitOnWhitespace();
            var result = new double[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                result[i] = DoubleParse(fields[i]);
            }

            return result;
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
        /// The input.
        /// </param>
        private static void SplitLine(string line, out string id, out string values)
        {
            int idx = line.IndexOf(' ');
            if (idx < 0)
            {
                id = line;
                values = null;
                return;
            }

            id = line.Substring(0, idx);
            values = line.Substring(idx + 1);
        }

        /// <summary>
        /// The add face.
        /// </summary>
        /// <param name="values">
        /// The input.
        /// </param>
        private void AddFace(string values)
        {
            // A polygonal face. The numbers are indexes into the arrays of vertex positions, 
            // texture coordinates, and normals respectively. A number may be omitted if, 
            // for example, texture coordinates are not being defined in the model.
            // There is no maximum number of vertices that a single polygon may contain. 
            // The .obj file specification says that each face must be flat and convex. 
            var fields = values.SplitOnWhitespace();
            var points = new List<Point3D>();
            var textureCoordinates = new List<Point>();
            var normals = new List<Vector3D>();
            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field))
                {
                    continue;
                }

                var ff = field.Split('/');
                int vi = int.Parse(ff[0]);
                int vti = ff.Length > 1 && ff[1].Length > 0 ? int.Parse(ff[1]) : int.MaxValue;
                int vni = ff.Length > 2 && ff[2].Length > 0 ? int.Parse(ff[2]) : int.MaxValue;
                if (vi < 0)
                {
                    vi = this.Points.Count + vi;
                }

                if (vti < 0)
                {
                    vti = this.TexCoords.Count + vti;
                }

                if (vni < 0)
                {
                    vni = this.Normals.Count + vni;
                }

                if (vi - 1 < this.Points.Count)
                {
                    points.Add(this.Points[vi - 1]);
                }

                if (vti < int.MaxValue && vti - 1 < this.TexCoords.Count)
                {
                    textureCoordinates.Add(this.TexCoords[vti - 1]);
                }

                if (vni < int.MaxValue && vni - 1 < this.Normals.Count)
                {
                    normals.Add(this.Normals[vni - 1]);
                }
            }

            if (textureCoordinates.Count == 0)
            {
                textureCoordinates = null;
            }

            if (normals.Count == 0)
            {
                normals = null;
            }

            if (normals == null)
            {
                // turn off normals in the mesh builder
                this.CurrentGroup.MeshBuilder.CreateNormals = false;
            }

            if (textureCoordinates == null)
            {
                // turn off texture coordinates in the mesh builder
                this.CurrentGroup.MeshBuilder.CreateTextureCoordinates = false;
            }

            // TRIANGLE
            if (points.Count == 3)
            {
                this.CurrentGroup.MeshBuilder.AddTriangles(points, normals, textureCoordinates);
                return;
            }

            // QUAD
            if (points.Count == 4)
            {
                this.CurrentGroup.MeshBuilder.AddQuads(points, normals, textureCoordinates);
                return;
            }

            // POLYGONS (flat and convex)
            {
                var poly3D = new Polygon3D(points);

                // Transform the polygon to 2D
                var poly2D = poly3D.Flatten();

                // Triangulate
                var triangleIndices = poly2D.Triangulate();
                if (triangleIndices != null)
                {
                    // Add the triangle indices with the 3D points
                    this.CurrentGroup.MeshBuilder.Append(points, triangleIndices, normals, textureCoordinates);
                }
            }
        }

        /// <summary>
        /// The add normal.
        /// </summary>
        /// <param name="values">
        /// The input.
        /// </param>
        private void AddNormal(string values)
        {
            var fields = Split(values);
            this.Normals.Add(new Vector3D(fields[0], fields[1], fields[2]));
        }

        /// <summary>
        /// The add tex coord.
        /// </summary>
        /// <param name="values">
        /// The input.
        /// </param>
        private void AddTexCoord(string values)
        {
            var fields = Split(values);
            this.TexCoords.Add(new Point(fields[0], 1 - fields[1]));
        }

        /// <summary>
        /// The add vertex.
        /// </summary>
        /// <param name="values">
        /// The input.
        /// </param>
        private void AddVertex(string values)
        {
            var fields = Split(values);
            this.Points.Add(new Point3D(fields[0], fields[1], fields[2]));
        }

        /// <summary>
        /// Builds the model.
        /// </summary>
        /// <returns>
        /// A Model3D object.
        /// </returns>
        private Model3DGroup BuildModel()
        {
            var modelGroup = new Model3DGroup();
            foreach (var g in this.Groups)
            {
                var gm = new GeometryModel3D { Geometry = g.MeshBuilder.ToMesh(), Material = g.Material };
                gm.BackMaterial = gm.Material;
                modelGroup.Children.Add(gm);
            }

            return modelGroup;
        }

        /// <summary>
        /// Gets the material with the specified name.
        /// </summary>
        /// <param name="materialName">
        /// The material name.
        /// </param>
        /// <returns>
        /// The material.
        /// </returns>
        private Material GetMaterial(string materialName)
        {
            MaterialDefinition mat;
            if (!string.IsNullOrEmpty(materialName) && this.Materials.TryGetValue(materialName, out mat))
            {
                return mat.GetMaterial(this.TexturePath);
            }

            return MaterialHelper.CreateMaterial(Brushes.Gold);
        }

        /// <summary>
        /// The load material lib.
        /// </summary>
        /// <param name="mtlFile">
        /// The mtl file.
        /// </param>
        private void LoadMaterialLib(string mtlFile)
        {
            var path = Path.Combine(this.TexturePath, mtlFile);
            if (!File.Exists(path))
            {
                return;
            }

            using (var mreader = new StreamReader(path))
            {
                MaterialDefinition currentMaterial = null;

                while (!mreader.EndOfStream)
                {
                    var line = mreader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    line = line.Trim();

                    if (line.StartsWith("#") || line.Length == 0)
                    {
                        continue;
                    }

                    string id, value;
                    SplitLine(line, out id, out value);

                    switch (id.ToLower())
                    {
                        case "newmtl":
                            if (value != null)
                            {
                                currentMaterial = new MaterialDefinition();
                                this.Materials.Add(value, currentMaterial);
                            }

                            break;
                        case "ka":
                            if (currentMaterial != null && value != null)
                            {
                                currentMaterial.Ambient = ColorParse(value);
                            }

                            break;
                        case "kd":
                            if (currentMaterial != null && value != null)
                            {
                                currentMaterial.Diffuse = ColorParse(value);
                            }

                            break;
                        case "ks":
                            if (currentMaterial != null && value != null)
                            {
                                currentMaterial.Specular = ColorParse(value);
                            }

                            break;
                        case "ns":
                            if (currentMaterial != null && value != null)
                            {
                                currentMaterial.SpecularCoefficient = DoubleParse(value);
                            }

                            break;
                        case "d":
                        case "tr":
                            if (currentMaterial != null && value != null)
                            {
                                currentMaterial.Dissolved = DoubleParse(value);
                            }

                            break;

                        case "illum":
                            if (currentMaterial != null && value != null)
                            {
                                currentMaterial.Illumination = int.Parse(value);
                            }

                            break;
                        case "map_ka":
                            if (currentMaterial != null)
                            {
                                currentMaterial.AmbientMap = value;
                            }

                            break;
                        case "map_kd":
                            if (currentMaterial != null)
                            {
                                currentMaterial.DiffuseMap = value;
                            }

                            break;
                        case "map_ks":
                            if (currentMaterial != null)
                            {
                                currentMaterial.SpecularMap = value;
                            }

                            break;
                        case "map_d":
                            if (currentMaterial != null)
                            {
                                currentMaterial.AlphaMap = value;
                            }

                            break;
                        case "map_bump":
                        case "bump":
                            if (currentMaterial != null)
                            {
                                currentMaterial.BumpMap = value;
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The set material.
        /// </summary>
        /// <param name="materialName">
        /// The material name.
        /// </param>
        private void SetMaterial(string materialName)
        {
            this.CurrentGroup.Material = this.GetMaterial(materialName);
        }

        /// <summary>
        /// The set smoothing.
        /// </summary>
        /// <param name="s">
        /// The input.
        /// </param>
        private void SetSmoothing(string s)
        {
            int smoothing;
            int.TryParse(s, out smoothing);
            this.CurrentGroup.Smoothing = smoothing;
        }

        #endregion

        /// <summary>
        /// A group.
        /// </summary>
        public class Group
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Group"/> class.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            public Group(string name)
            {
                this.Name = name;
                this.Material = MaterialHelper.CreateMaterial(Brushes.Green);
                this.MeshBuilder = new MeshBuilder(true, true);
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///   Gets or sets the material.
            /// </summary>
            /// <value>The material.</value>
            public Material Material { get; set; }

            /// <summary>
            ///   Gets or sets the mesh builder.
            /// </summary>
            /// <value>The mesh builder.</value>
            public MeshBuilder MeshBuilder { get; set; }

            /// <summary>
            ///   Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            ///   Gets or sets the smoothing.
            /// </summary>
            /// <value>The smoothing.</value>
            public int Smoothing { get; set; }

            #endregion
        }

        /// <summary>
        /// A material definition.
        /// </summary>
        /// <remarks>
        /// The file format is documented in http://en.wikipedia.org/wiki/Material_Template_Library.
        /// </remarks>
        public class MaterialDefinition
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MaterialDefinition"/> class.
            /// </summary>
            public MaterialDefinition()
            {
                this.Dissolved = 1.0;
            }

            #region Public Properties

            /// <summary>
            ///   Gets or sets the alpha map.
            /// </summary>
            /// <value>The alpha map.</value>
            public string AlphaMap { get; set; }

            /// <summary>
            ///   Gets or sets the ambient.
            /// </summary>
            /// <value>The ambient.</value>
            public Color Ambient { get; set; }

            /// <summary>
            ///   Gets or sets the ambient map.
            /// </summary>
            /// <value>The ambient map.</value>
            public string AmbientMap { get; set; }

            /// <summary>
            ///   Gets or sets the bump map.
            /// </summary>
            /// <value>The bump map.</value>
            public string BumpMap { get; set; }

            /// <summary>
            ///   Gets or sets the diffuse.
            /// </summary>
            /// <value>The diffuse.</value>
            public Color Diffuse { get; set; }

            /// <summary>
            ///   Gets or sets the diffuse map.
            /// </summary>
            /// <value>The diffuse map.</value>
            public string DiffuseMap { get; set; }

            /// <summary>
            ///   Gets or sets the dissolved.
            /// </summary>
            /// <value>The dissolved.</value>
            public double Dissolved { get; set; }

            /// <summary>
            ///   Gets or sets the illumination.
            /// </summary>
            /// <value>The illumination.</value>
            public int Illumination { get; set; }

            /// <summary>
            ///   Gets or sets the specular.
            /// </summary>
            /// <value>The specular.</value>
            public Color Specular { get; set; }

            /// <summary>
            ///   Gets or sets the specular coefficient.
            /// </summary>
            /// <value>The specular coefficient.</value>
            public double SpecularCoefficient { get; set; }

            /// <summary>
            ///   Gets or sets the specular map.
            /// </summary>
            /// <value>The specular map.</value>
            public string SpecularMap { get; set; }

            #endregion

            #region Public Methods

            /// <summary>
            /// Gets the material from the specified path.
            /// </summary>
            /// <param name="texturePath">
            /// The texture path.
            /// </param>
            /// <returns>
            /// The material.
            /// </returns>
            public Material GetMaterial(string texturePath)
            {
                var mg = new MaterialGroup();

                // add the diffuse component
                if (this.DiffuseMap == null)
                {
                    var diffuseBrush = new SolidColorBrush(this.Diffuse) { Opacity = this.Dissolved };
                    mg.Children.Add(new DiffuseMaterial(diffuseBrush));
                }
                else
                {
                    var path = Path.Combine(texturePath, this.DiffuseMap);
                    if (File.Exists(path))
                    {
                        var img = new BitmapImage(new Uri(path, UriKind.Relative));
                        var textureBrush = new ImageBrush(img) { Opacity = this.Dissolved, ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.Tile };
                        mg.Children.Add(new DiffuseMaterial(textureBrush));
                    }
                }

                // add the ambient components (using EmissiveMaterial)
                if (this.AmbientMap == null)
                {
                    var ambientBrush = new SolidColorBrush(this.Ambient) { Opacity = this.Dissolved };
                    mg.Children.Add(new EmissiveMaterial(ambientBrush));
                }
                else
                {
                    var path = Path.Combine(texturePath, this.AmbientMap);
                    if (File.Exists(path))
                    {
                        var img = new BitmapImage(new Uri(path, UriKind.Relative));
                        var textureBrush = new ImageBrush(img) { Opacity = this.Dissolved, ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.Tile };
                        mg.Children.Add(new EmissiveMaterial(textureBrush));
                    }
                }

                // add the specular component
                mg.Children.Add(new SpecularMaterial(new SolidColorBrush(this.Specular), this.SpecularCoefficient));

                return mg;
            }

            #endregion
        }
    }
}