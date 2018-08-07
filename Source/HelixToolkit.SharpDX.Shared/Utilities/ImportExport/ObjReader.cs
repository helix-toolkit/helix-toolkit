// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjReader.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A Wavefront .obj file reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Matrix = System.Numerics.Matrix4x4;
using HelixToolkit.Mathematics;
#if !NETFX_CORE
//using System.Windows.Media.Imaging;
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
#if NETFX_CORE
    using FileFormatException = Exception;
#endif
#if CORE
    using PhongMaterial = Model.PhongMaterialCore;
#endif
    using Model;
    using Core;
    using Color = Mathematics.Color4;
    using Object3DGroup = System.Collections.Generic.List<Object3D>;
    using Point = System.Numerics.Vector2;
    using Point3D = System.Numerics.Vector3;
    using Vector3D = System.Numerics.Vector3;

    public class Object3D
    {
        public Geometry3D Geometry { get; set; }
        public MaterialCore Material { get; set; }
        public List<Matrix> Transform { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// A Wavefront .obj file reader.
    /// </summary>
    /// <remarks>
    /// See the file format specifications at
    /// http://en.wikipedia.org/wiki/Obj
    /// http://en.wikipedia.org/wiki/Material_Template_Library
    /// http://www.martinreddy.net/gfx/3d/OBJ.spec
    /// http://www.eg-models.de/formats/Format_Obj.html
    /// </remarks>
    public class ObjReader : IModelReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref = "ObjReader" /> class.
        /// </summary>
        public ObjReader()
        {
            this.IgnoreErrors = false;
            this.SwitchYZ = false;

            this.IsSmoothingDefault = true;
            this.SkipTransparencyValues = true;

            this.DefaultColor = Mathematics.Color.Gold;

            this.Points = new List<Point3D>();
            this.Colors = new List<Color4>();
            this.TextureCoordinates = new List<Point>();
            this.Normals = new List<Vector3D>();

            this.Groups = new List<Group>();
            this.Materials = new Dictionary<string, MaterialDefinition>();

            this.smoothingGroupMaps = new Dictionary<long, Dictionary<Tuple<int, int, int>, int>>();
        }

        /// <summary>
        /// Gets or sets the default color.
        /// </summary>
        /// <value>The default color.</value>
        /// <remarks>
        /// The default value is Colors.Gold.
        /// </remarks>
        public Color DefaultColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore errors.
        /// </summary>
        /// <value><c>true</c> if errors should be ignored; <c>false</c> if errors should throw an exception.</value>
        /// <remarks>
        /// The default value is on (true).
        /// </remarks>
        public bool IgnoreErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to switch Y and Z coordinates.
        /// </summary>
        public bool SwitchYZ { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to skip transparency values ("Tr") in the material files.
        /// </summary>
        /// <value>
        /// <c>true</c> if transparency values should be skipped; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This option is added to allow disabling the "Tr" values in files where it has been defined incorrectly.
        /// The transparency values ("Tr") are interpreted as 0 = transparent, 1 = opaque.
        /// The dissolve values ("d") are interpreted as 0 = transparent, 1=opaque.
        /// </remarks>
        public bool SkipTransparencyValues { get; set; }

        /// <summary>
        /// Sets a value indicating whether smoothing is default.
        /// </summary>
        /// <remarks>
        /// The default value is smoothing=on (true).
        /// </remarks>
        public bool IsSmoothingDefault
        {
            set
            {
                this.currentSmoothingGroup = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Gets the groups of the file.
        /// </summary>
        /// <value>The groups.</value>
        public IList<Group> Groups { get; private set; }

        /// <summary>
        /// Gets the materials in the imported material files.
        /// </summary>
        /// <value>The materials.</value>
        public Dictionary<string, MaterialDefinition> Materials { get; private set; }

        /// <summary>
        /// Gets or sets the path to the textures.
        /// </summary>
        /// <value>The texture path.</value>
        public string TexturePath { get; set; }

        /// <summary>
        /// Additional info how to treat the model
        /// </summary>
        public ModelInfo ModelInfo { get; private set; }

        /// <summary>
        /// Reads the model from the specified path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="info">
        /// The model info.
        /// </param>
        /// <returns>
        /// The model.
        /// </returns>
        public Object3DGroup Read(string path, ModelInfo info = default(ModelInfo))
        {
            this.TexturePath = Path.GetDirectoryName(path);
            this.ModelInfo = info;

            using (var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return this.Read(s);
            }
        }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">
        /// The stream.
        /// </param>
        /// <param name="info">
        /// The model info.
        /// </param>
        /// <returns>
        /// The model.
        /// </returns>
        public Object3DGroup Read(Stream s, ModelInfo info = default(ModelInfo))
        {
            using (this.Reader = new StreamReader(s))
            {
                this.currentLineNo = 0;
                while (!this.Reader.EndOfStream)
                {
                    this.currentLineNo++;
                    var line = this.Reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    line = line.Trim();
                    while (line.EndsWith("\\"))
                    {
                        var nextLine = this.Reader.ReadLine();
                        while (nextLine.Length == 0)
                        {
                            nextLine = this.Reader.ReadLine();
                        }

                        line = line.TrimEnd('\\') + nextLine;
                    }

                    if (line.StartsWith("#") || line.Length == 0)
                    {
                        continue;
                    }

                    string keyword, values;
                    SplitLine(line, out keyword, out values);
                    switch (keyword.ToLower())
                        {
                            // Vertex data
                            case "v": // geometric vertices
                                this.AddVertex(values);
                                break;
                            case "vt": // texture vertices
                                this.AddTexCoord(values);
                                break;
                            case "vn": // vertex normals
                                this.AddNormal(values);
                                break;
                            case "vp": // parameter space vertices
                            case "cstype": // rational or non-rational forms of curve or surface type: basis matrix, Bezier, B-spline, Cardinal, Taylor
                            case "degree": // degree
                            case "bmat": // basis matrix
                            case "step": // step size
                                // not supported
                                break;

                            // Elements
                            case "f": // face
                                this.AddFace(values);
                                break;
                            case "p": // point
                            case "l": // line
                            case "curv": // curve
                            case "curv2": // 2D curve
                            case "surf": // surface
                                // not supported
                                break;

                            // Free-form curve/surface body statements
                            case "parm": // parameter name
                            case "trim": // outer trimming loop (trim)
                            case "hole": // inner trimming loop (hole)
                            case "scrv": // special curve (scrv)
                            case "sp":  // special point (sp)
                            case "end": // end statement (end)
                                // not supported
                                break;

                            // Connectivity between free-form surfaces
                            case "con": // connect
                                // not supported
                                break;

                            // Grouping
                            case "g": // group name
                                this.AddGroup(values);
                                break;
                            case "s": // smoothing group
                                this.SetSmoothingGroup(values);
                                break;
                            case "mg": // merging group
                                break;
                            case "o": // object name
                                // not supported
                                break;

                            // Display/render attributes
                            case "mtllib": // material library
                                this.LoadMaterialLib(values);
                                break;
                            case "usemtl": // material name
                                this.EnsureNewMesh();

                                this.SetMaterial(values);
                                break;
                            case "usemap": // texture map name
                                this.EnsureNewMesh();

                                break;
                            case "bevel": // bevel interpolation
                            case "c_interp": // color interpolation
                            case "d_interp": // dissolve interpolation
                            case "lod": // level of detail
                            case "shadow_obj": // shadow casting
                            case "trace_obj": // ray tracing
                            case "ctech": // curve approximation technique
                            case "stech": // surface approximation technique
                                // not supported
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
        /// <remarks>
        /// This is a file format used by Helix Toolkit only.
        /// Use the GZipHelper class to compress an .obj file.
        /// </remarks>
        public Object3DGroup ReadZ(string path)
        {
            this.TexturePath = Path.GetDirectoryName(path);
            using (var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var deflateStream = new GZipStream(s, CompressionMode.Decompress, true);
                return this.Read(deflateStream);
            }
        }



        /// <summary>
        /// The smoothing group maps.
        /// </summary>
        /// <remarks>
        /// The outer dictionary maps from a smoothing group number to a Dictionary&lt;long,int&gt;.
        /// The inner dictionary maps from an obj file (vertex, texture coordinates, normal) index to a vertex index in the current group.
        /// </remarks>
        private readonly Dictionary<long, Dictionary<Tuple<int, int, int>, int>> smoothingGroupMaps;

        /// <summary>
        /// The current smoothing group.
        /// </summary>
        private long currentSmoothingGroup;

        /// <summary>
        /// The line number of the line being parsed.
        /// </summary>
        private int currentLineNo;

        /// <summary>
        /// Gets the current group.
        /// </summary>
        private Group CurrentGroup
        {
            get
            {
                if (this.Groups.Count == 0)
                {
                    this.AddGroup("default");
                }

                return this.Groups[this.Groups.Count - 1];
            }
        }

        /// <summary>
        /// Gets or sets the normals.
        /// </summary>
        private IList<Vector3D> Normals { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        private IList<Point3D> Points { get; set; }
        /// <summary>
        /// Gets or sets the vertex colors.
        /// </summary>
        /// <value>
        /// The colors.
        /// </value>
        private IList<Color4> Colors { set; get; }
        /// <summary>
        /// Gets or sets the stream reader.
        /// </summary>
        private StreamReader Reader { get; set; }

        /// <summary>
        /// Gets or sets the texture coordinates.
        /// </summary>
        private IList<Point> TextureCoordinates { get; set; }

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
#if NETFX_CORE
            return new Color((float)fields[0], (float)fields[1], (float)fields[2], 1);
#else
            return System.Windows.Media.Color.FromRgb((byte)(fields[0] * 255), (byte)(fields[1] * 255), (byte)(fields[2] * 255)).ToColor4();
#endif
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
        /// Splits a line in keyword and arguments.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="keyword">
        /// The keyword.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        private static void SplitLine(string line, out string keyword, out string arguments)
        {
            int idx = line.IndexOf(' ');
            if (idx < 0)
            {
                keyword = line;
                arguments = null;
                return;
            }

            keyword = line.Substring(0, idx);
            arguments = line.Substring(idx + 1);
        }

        /// <summary>
        /// Adds a group with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        private void AddGroup(string name)
        {
            this.Groups.Add(new Group(name));
            this.smoothingGroupMaps.Clear();
        }

        /// <summary>
        /// Ensures that a new mesh is created.
        /// </summary>
        private void EnsureNewMesh()
        {
            if (this.CurrentGroup.MeshBuilder.TriangleIndices.Count != 0)
            {
                this.CurrentGroup.AddMesh();
                this.smoothingGroupMaps.Clear();
            }
        }

        /// <summary>
        /// Sets the smoothing group number.
        /// </summary>
        /// <param name="values">The group number.</param>
        private void SetSmoothingGroup(string values)
        {
            if (values == "off")
            {
                this.currentSmoothingGroup = 0;
            }
            else
            {
                long smoothingGroup;
                if (long.TryParse(values, out smoothingGroup))
                {
                    this.currentSmoothingGroup = smoothingGroup;
                }
                else
                {
                    // invalid parameter
                    if (this.IgnoreErrors)
                    {
                        return;
                    }
                    throw new FileFormatException(string.Format("Invalid smoothing group ({0}) at line {1}.", values, this.currentLineNo));
                }
            }
        }

        /// <summary>
        /// Adds a face.
        /// </summary>
        /// <param name="values">
        /// The input values.
        /// </param>
        /// <remarks>
        /// Adds a polygonal face. The numbers are indexes into the arrays of vertex positions,
        /// texture coordinates, and normals respectively. A number may be omitted if,
        /// for example, texture coordinates are not being defined in the model.
        /// There is no maximum number of vertices that a single polygon may contain.
        /// The .obj file specification says that each face must be flat and convex.
        /// </remarks>
        private void AddFace(string values)
        {
            var currentGroup = this.CurrentGroup;
            var builder = currentGroup.MeshBuilder;
            var positions = builder.Positions;
            var colors = currentGroup.VertexColors;
            var textureCoordinates = builder.TextureCoordinates;
            var normals = builder.Normals;

            Dictionary<Tuple<int, int, int>, int> smoothingGroupMap = null;

            // If a smoothing group is defined, get the map from obj-file-index to current-group-vertex-index.
            if (this.currentSmoothingGroup != 0)
            {
                if (!this.smoothingGroupMaps.TryGetValue(this.currentSmoothingGroup, out smoothingGroupMap))
                {
                    smoothingGroupMap = new Dictionary<Tuple<int, int, int>, int>();
                    this.smoothingGroupMaps.Add(this.currentSmoothingGroup, smoothingGroupMap);
                }
            }

            var fields = values.SplitOnWhitespace();
            var faceIndices = new List<int>();
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

                // Handle relative indices (negative numbers)
                if (vi < 0)
                {
                    vi = this.Points.Count + vi + 1;
                }

                if (vti < 0)
                {
                    vti = this.TextureCoordinates.Count + vti + 1;
                }

                if (vni < 0)
                {
                    vni = this.Normals.Count + vni + 1;
                }

                // Check if the indices are valid
                if (vi - 1 >= this.Points.Count)
                {
                    if (this.IgnoreErrors)
                    {
                        return;
                    }

                    throw new FileFormatException(string.Format("Invalid vertex index ({0}) on line {1}.", vi, this.currentLineNo));
                }

                if (vti == int.MaxValue)
                {
                    // turn off texture coordinates in the builder
                    //builder.CreateTextureCoordinates = false;
                    builder.TextureCoordinates = null;
                }

                if (vni == int.MaxValue)
                {
                    // turn off normals in the builder
                    //builder.CreateNormals = false;
                    builder.Normals = null;
                }

                // check if the texture coordinate index is valid
                if (builder.HasTexCoords && vti - 1 >= this.TextureCoordinates.Count)
                {
                    if (this.IgnoreErrors)
                    {
                        return;
                    }

                    throw new FileFormatException(string.Format("Invalid texture coordinate index ({0}) on line {1}.", vti, this.currentLineNo));
                }

                // check if the normal index is valid
                if (builder.HasNormals && vni - 1 >= this.Normals.Count)
                {
                    if (this.IgnoreErrors)
                    {
                        return;
                    }

                    throw new FileFormatException(string.Format("Invalid normal index ({0}) on line {1}.", vni, this.currentLineNo));
                }

                bool addVertex = true;

                if (smoothingGroupMap != null)
                {
                    var key = Tuple.Create(vi, vti, vni);

                    int vix;
                    if (smoothingGroupMap.TryGetValue(key, out vix))
                    {
                        // use the index of a previously defined vertex
                        addVertex = false;
                    }
                    else
                    {
                        // add a new vertex
                        vix = positions.Count;
                        smoothingGroupMap.Add(key, vix);
                    }

                    faceIndices.Add(vix);
                }
                else
                {
                    // if smoothing is off, always add a new vertex
                    faceIndices.Add(positions.Count);
                }

                if (addVertex)
                {
                    // add vertex
                    positions.Add(this.Points[vi - 1]);
                    if (Colors.Count == Points.Count)
                    { colors.Add(this.Colors[vi - 1]); }
                    // add texture coordinate (if enabled)
                    if (builder.HasTexCoords)
                    {
                        textureCoordinates.Add(this.TextureCoordinates[vti - 1]);
                    }

                    // add normal (if enabled)
                    if (builder.HasNormals)
                    {
                        normals.Add(this.Normals[vni - 1]);
                    }
                }
            }

            try
            {


                if (faceIndices.Count < 3)
                {
                    throw new HelixToolkitException("Polygon must have at least 3 indices!");
                }


                if (this.ModelInfo.Faces == MeshFaces.QuadPatches)
                {
                    if (faceIndices.Count == 3)
                    {
                        faceIndices.Add(faceIndices.Last());
                        builder.AddQuad(faceIndices);
                    }
                    if (faceIndices.Count == 4)
                    {
                        builder.AddQuad(faceIndices);
                    }
                    else
                    {
                        // add triangles by sweep line algorithm
                        builder.AddPolygonByTriangulation(faceIndices);
                    }
                }
                else
                {
                    if (faceIndices.Count == 3)
                    {
                        builder.AddTriangle(faceIndices);
                    }
                    else if (faceIndices.Count == 4)
                    {
                        //builder.AddQuad(faceIndices);
                        builder.AddTriangleFan(faceIndices);
                    }
                    else
                    {
                        // add triangles by sweep line algorithm
                        builder.AddPolygonByTriangulation(faceIndices);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error composing polygonal object: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Adds a normal.
        /// </summary>
        /// <param name="values">
        /// The input values.
        /// </param>
        private void AddNormal(string values)
        {
            var fields = Split(values);
            if (SwitchYZ)
            {
                this.Normals.Add(new Vector3D((float)fields[0], (float)-fields[2], (float)fields[1]));
            }
            else
            {
                this.Normals.Add(new Vector3D((float)fields[0], (float)fields[1], (float)fields[2]));
            }
        }

        /// <summary>
        /// Adds a texture coordinate.
        /// </summary>
        /// <param name="values">
        /// The input values.
        /// </param>
        private void AddTexCoord(string values)
        {
            var fields = Split(values);
            this.TextureCoordinates.Add(new Point((float)fields[0], 1 - (float)fields[1]));
        }

        /// <summary>
        /// Adds a vertex.
        /// </summary>
        /// <param name="values">
        /// The input values.
        /// </param>
        private void AddVertex(string values)
        {
            var fields = Split(values);
            if (SwitchYZ)
            {
                this.Points.Add(new Point3D((float)fields[0], (float)-fields[2], (float)fields[1]));
            }
            else
            {
                this.Points.Add(new Point3D((float)fields[0], (float)fields[1], (float)fields[2]));
            }
            if(fields.Count >= 6)
            {
                if (fields.Count == 6)
                {
                    this.Colors.Add(new Color4((float)fields[3], (float)fields[4], (float)fields[5], 1f));
                }
                else
                {
                    this.Colors.Add(new Color4((float)fields[3], (float)fields[4], (float)fields[5], (float)fields[6]));
                }
            }
        }

        /// <summary>
        /// Builds the model.
        /// </summary>
        /// <returns>
        /// A Model3D object.
        /// </returns>
        private Object3DGroup BuildModel()
        {
            var modelGroup = new Object3DGroup();
            foreach (var g in this.Groups)
            {
                foreach (var gm in g.CreateModels(this.ModelInfo))
                {
                    modelGroup.Add(gm);
                }
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
        private PhongMaterialCore GetMaterial(string materialName)
        {
            MaterialDefinition mat;
            if (!string.IsNullOrEmpty(materialName) && this.Materials.TryGetValue(materialName, out mat))
            {
                return mat.GetMaterial(this.TexturePath);
            }
            return new PhongMaterialCore()
            {
                Name = "DefaultVRML",
                AmbientColor = new Color(0.2f, 0.2f, 0.2f, 1.0f),
                DiffuseColor = new Color(0.8f, 0.8f, 0.8f, 1.0f),
                SpecularColor = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                EmissiveColor = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                SpecularShininess = 25.6f,
            };
        }

        /// <summary>
        /// Loads a material library.
        /// </summary>
        /// <param name="mtlFile">
        /// The mtl file.
        /// </param>
        private void LoadMaterialLib(string mtlFile)
        {
            var path = Path.GetFullPath(Path.Combine(this.TexturePath, "./" + mtlFile));
            if (!File.Exists(path))
            {
                return;
            }
            using (var fileStream = File.OpenRead(path))
            {            
                using (var mreader = new StreamReader(fileStream))
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

                        string keyword, value;
                        SplitLine(line, out keyword, out value);

                        switch (keyword.ToLower())
                        {
                            case "newmtl":
                                if (value != null)
                                {
                                    if (this.Materials.ContainsKey(value))
                                    {
                                        currentMaterial = null;
                                    }
                                    else
                                    {
                                        currentMaterial = new MaterialDefinition();
                                        this.Materials.Add(value, currentMaterial);
                                    }
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
                                if (currentMaterial != null && value != null)
                                {
                                    currentMaterial.Dissolved = DoubleParse(value);
                                }

                                break;
                            case "tr":
                                if (!this.SkipTransparencyValues && currentMaterial != null && value != null)
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
        }

        /// <summary>
        /// Sets the material for the current group.
        /// </summary>
        /// <param name="materialName">
        /// The material name.
        /// </param>
        private void SetMaterial(string materialName)
        {
            this.CurrentGroup.Material = this.GetMaterial(materialName);
        }



        /// <summary>
        /// Represents a group in the obj file.
        /// </summary>
        public class Group
        {
            /// <summary>
            /// List of mesh builders.
            /// </summary>
            private readonly IList<MeshBuilder> meshBuilders;
            private readonly IList<Color4Collection> vertexColors;
            /// <summary>
            /// List of materials.
            /// </summary>
            private readonly IList<PhongMaterialCore> materials;

            /// <summary>
            /// Initializes a new instance of the <see cref="Group"/> class.
            /// </summary>
            /// <param name="name">
            /// The name of the group.
            /// </param>
            public Group(string name)
            {
                this.Name = name;
                this.meshBuilders = new List<MeshBuilder>();
                this.materials = new List<PhongMaterialCore>();
                this.vertexColors = new List<Color4Collection>();
                this.AddMesh();
            }

            /// <summary>
            /// Sets the material.
            /// </summary>
            /// <value>The material.</value>
            public PhongMaterialCore Material
            {
                set
                {
                    this.materials[this.materials.Count - 1] = value;
                }
            }

            /// <summary>
            /// Gets the mesh builder for the current mesh.
            /// </summary>
            /// <value>The mesh builder.</value>
            public MeshBuilder MeshBuilder
            {
                get
                {
                    return this.meshBuilders[this.meshBuilders.Count - 1];
                }
            }

            public Color4Collection VertexColors
            {
                get { return this.vertexColors[this.vertexColors.Count - 1]; }
            }
            /// <summary>
            /// Gets or sets the group name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Adds a mesh.
            /// </summary>
            public void AddMesh()
            {
                var meshBuilder = new MeshBuilder(true, true);
                this.meshBuilders.Add(meshBuilder);
                this.vertexColors.Add(new Color4Collection());
                this.materials.Add(new PhongMaterialCore() { DiffuseColor = new Color(0, 1, 0, 1) });
            }

            /// <summary>
            /// Creates the models of the group.
            /// </summary>
            /// <returns>The models.</returns>
            public IEnumerable<Object3D> CreateModels(ModelInfo info)
            {
                for (int i = 0; i < this.meshBuilders.Count; i++)
                {
                    this.meshBuilders[i].ComputeNormalsAndTangents(info.Faces, true);
                    var mesh = this.meshBuilders[i].ToMeshGeometry3D();
                    mesh.Colors = this.vertexColors[i];
                    yield return new Object3D
                    {
                        Geometry = mesh,
                        Material = this.materials[i],
                        Transform = new List<Matrix>()
                    };
                }
            }
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

            /// <summary>
            /// Gets or sets the alpha map.
            /// </summary>
            /// <value>The alpha map.</value>
            public string AlphaMap { get; set; }

            /// <summary>
            /// Gets or sets the ambient color.
            /// </summary>
            /// <value>The ambient.</value>
            public Color Ambient { get; set; }

            /// <summary>
            /// Gets or sets the ambient map.
            /// </summary>
            /// <value>The ambient map.</value>
            public string AmbientMap { get; set; }

            /// <summary>
            /// Gets or sets the bump map.
            /// </summary>
            /// <value>The bump map.</value>
            public string BumpMap { get; set; }

            /// <summary>
            /// Gets or sets the diffuse color.
            /// </summary>
            /// <value>The diffuse.</value>
            public Color Diffuse { get; set; }

            /// <summary>
            /// Gets or sets the diffuse map.
            /// </summary>
            /// <value>The diffuse map.</value>
            public string DiffuseMap { get; set; }

            /// <summary>
            /// Gets or sets the opacity value.
            /// </summary>
            /// <value>The opacity.</value>
            /// <remarks>
            /// 0.0 is transparent, 1.0 is opaque.
            /// </remarks>
            public double Dissolved { get; set; }

            /// <summary>
            /// Gets or sets the illumination.
            /// </summary>
            /// <value>The illumination.</value>
            public int Illumination { get; set; }

            /// <summary>
            /// Gets or sets the specular color.
            /// </summary>
            /// <value>The specular color.</value>
            public Color Specular { get; set; }

            /// <summary>
            /// Gets or sets the specular coefficient.
            /// </summary>
            /// <value>The specular coefficient.</value>
            public double SpecularCoefficient { get; set; }

            /// <summary>
            /// Gets or sets the specular map.
            /// </summary>
            /// <value>The specular map.</value>
            public string SpecularMap { get; set; }

            /// <summary>
            /// Gets or sets the material.
            /// </summary>
            /// <value>The material.</value>
            public PhongMaterialCore Material { get; set; }

            /// <summary>
            /// Gets the material from the specified path.
            /// </summary>
            /// <param name="texturePath">
            /// The texture path.
            /// </param>
            /// <returns>
            /// The material.
            /// </returns>
            public PhongMaterialCore GetMaterial(string texturePath)
            {
                if (this.Material == null)
                {
                    this.Material = this.CreateMaterial(texturePath);
                    //this.Material.Freeze();
                }

                return this.Material;
            }

            /// <summary>
            /// Creates the material.
            /// </summary>
            /// <param name="texturePath">The texture path.</param>
            /// <returns>A WPF material.</returns>
            private PhongMaterialCore CreateMaterial(string texturePath)
            {
                MemoryStream diffuseMapMS = null;
                if (DiffuseMap != null)
                {
                    using (var fs = new FileStream(Path.GetFullPath(Path.Combine(texturePath, "./" + this.DiffuseMap)), FileMode.Open))
                    {
                        diffuseMapMS = new MemoryStream();
                        fs.CopyTo(diffuseMapMS);
                    }
                }
                MemoryStream bumpMapMS = null;
                if (BumpMap != null)
                {
                    using (var fs = new FileStream(Path.GetFullPath(Path.Combine(texturePath, "./" + this.BumpMap)), FileMode.Open))
                    {
                        bumpMapMS = new MemoryStream();
                        fs.CopyTo(bumpMapMS);
                    }
                }
                MemoryStream alphaMapMS = null;
                if (AlphaMap != null)
                {
                    using (var fs = new FileStream(Path.GetFullPath(Path.Combine(texturePath, "./" + this.AlphaMap)), FileMode.Open))
                    {
                        alphaMapMS = new MemoryStream();
                        fs.CopyTo(alphaMapMS);
                    }
                }
                var mat = new PhongMaterialCore()
                {
                    AmbientColor = this.Ambient,
                    //AmbientMap = this.AmbientMap,

                    DiffuseColor = this.Diffuse,
                    DiffuseMap = diffuseMapMS,

                    SpecularColor = this.Specular,
                    SpecularShininess = (float)this.SpecularCoefficient,
                    //SpecularMap = this.SpecularMap,

                    NormalMap = bumpMapMS,
                    DiffuseAlphaMap = alphaMapMS,
                    //Dissolved = this.Dissolved,
                    //Illumination = this.Illumination,

                };

                //return mg.Children.Count != 1 ? mg : mg.Children[0];
                return mat;
            }


            //private static BitmapImage LoadImage(string path)
            //{
            //    var bmp = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            //    return bmp;
            //}

        }
    }
}