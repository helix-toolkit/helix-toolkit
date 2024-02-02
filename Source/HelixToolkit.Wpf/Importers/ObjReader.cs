using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Geometry;

namespace HelixToolkit.Wpf;

/// <summary>
/// A Wavefront .obj file reader.
/// </summary>
public sealed class ObjReader : ModelReader
{
    private static readonly char[] Delimiters = new char[] { ' ' };

    /// <summary>
    /// The smoothing group maps.
    /// </summary>
    /// <remarks>
    /// The outer dictionary maps from a smoothing group number to a dictionary.
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
    /// Initializes a new instance of the <see cref="ObjReader" /> class.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    public ObjReader(Dispatcher? dispatcher = null)
        : base(dispatcher)
    {
        this.IgnoreErrors = false;
        this.SwitchYZ = false;

        this.IsSmoothingDefault = true;
        this.SkipTransparencyValues = true;

        this.Points = new List<Point3D>();
        this.TextureCoordinates = new List<Point>();
        this.Normals = new List<Vector3D>();

        this.Groups = new List<Group>();
        this.Materials = new Dictionary<string, MaterialDefinition>();

        this.smoothingGroupMaps = new Dictionary<long, Dictionary<Tuple<int, int, int>, int>>();

        // File format specifications
        // http://en.wikipedia.org/wiki/Obj
        // http://en.wikipedia.org/wiki/Material_Template_Library
        // http://www.martinreddy.net/gfx/3d/OBJ.spec
        // http://www.eg-models.de/formats/Format_Obj.html
    }

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
    /// Gets or sets a value indicating whether to skip transparency values in the material files.
    /// </summary>
    /// <value>
    /// <c>true</c> if transparency values should be skipped; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// This option is added to allow disabling the <code>Tr</code> values in files where it has been defined incorrectly.
    /// The transparency values (<code>Tr</code>) are interpreted as 0 = transparent, 1 = opaque.
    /// The dissolve values (<code>d</code>) are interpreted as 0 = transparent, 1 = opaque.
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
    /// Gets or sets the current material.
    /// </summary>
    private Material? CurrentMaterial { get; set; }

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
    /// Gets or sets the normal vectors.
    /// </summary>
    private IList<Vector3D> Normals { get; set; }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    private IList<Point3D> Points { get; set; }

    /// <summary>
    /// Gets or sets the stream reader.
    /// </summary>
    private StreamReader? Reader { get; set; }

    /// <summary>
    /// Gets or sets the texture coordinates.
    /// </summary>
    private IList<Point> TextureCoordinates { get; set; }

    /// <summary>
    /// Reads the model and any associated materials from streams
    /// </summary>
    /// <param name="objStream">A model stream from the obj file</param>
    /// <param name="mtlStreams">Array of Material streams referenced in the obj file</param>
    /// <returns></returns>
    public Model3DGroup Read(Stream objStream, Stream[] mtlStreams)
    {
        foreach (var mtlStream in mtlStreams)
        {
            using var mtlStreamReader = new StreamReader(mtlStream);
            ReadMaterial(mtlStreamReader);
        }

        return Read(objStream);
    }

    /// <summary>
    /// Reads the model from the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The model.</returns>
    public override Model3DGroup? Read(string path)
    {
        this.TexturePath = Path.GetDirectoryName(path) ?? "";
        using var s = GetResourceStream(path);

        if (s is null)
        {
            return null;
        }

        return this.Read(s);
    }

    /// <summary>
    /// Reads the model from the specified stream.
    /// </summary>
    /// <param name="s">The stream.</param>
    /// <returns>The model.</returns>
    public override Model3DGroup Read(Stream s)
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
                    while (nextLine?.Length == 0)
                    {
                        nextLine = this.Reader.ReadLine();
                    }

                    line = line.TrimEnd('\\') + nextLine;
                }

                if (line.StartsWith("#") || line.Length == 0)
                {
                    continue;
                }

                SplitLine(line, out string keyword, out string values);

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
    /// <param name="path">The path.</param>
    /// <returns>A Model3D object containing the model.</returns>
    /// <remarks>This is a file format used by Helix Toolkit only.
    /// Use the GZipHelper class to compress an .obj file.</remarks>
    public Model3DGroup? ReadZ(string path)
    {
        this.TexturePath = Path.GetDirectoryName(path) ?? "";
        using var s = GetResourceStream(path);

        if (s is null)
        {
            return null;
        }

        using var deflateStream = new GZipStream(s, CompressionMode.Decompress, true);
        return this.Read(deflateStream);
    }

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
        var fields = input.Split(Delimiters, System.StringSplitOptions.RemoveEmptyEntries);
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
            arguments = string.Empty;
            return;
        }

        keyword = line[..idx];
        arguments = line[(idx + 1)..];
    }

    /// <summary>
    /// Adds a group with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    private void AddGroup(string name)
    {
        this.Groups.Add(new Group(name, this.CurrentMaterial ?? this.DefaultMaterial));
        this.smoothingGroupMaps.Clear();
    }

    /// <summary>
    /// Ensures that a new mesh is created.
    /// </summary>
    private void EnsureNewMesh()
    {
        if (this.CurrentGroup.MeshBuilder.TriangleIndices.Count != 0)
        {
            this.CurrentGroup.AddMesh(this.CurrentMaterial ?? this.DefaultMaterial);
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
            if (long.TryParse(values, out long smoothingGroup))
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
    /// texture coordinates, and normal vectors respectively. A number may be omitted if,
    /// for example, texture coordinates are not being defined in the model.
    /// There is no maximum number of vertices that a single polygon may contain.
    /// The .obj file specification says that each face must be flat and convex.
    /// </remarks>
    private void AddFace(string values)
    {
        var currentGroup = this.CurrentGroup;
        var builder = currentGroup.MeshBuilder;
        var positions = builder.Positions;
        var textureCoordinates = builder.TextureCoordinates;
        var normals = builder.Normals;

        Dictionary<Tuple<int, int, int>, int>? smoothingGroupMap = null;

        // If a smoothing group is defined, get the map from obj-file-index to current-group-vertex-index.
        if (this.currentSmoothingGroup != 0)
        {
            if (!this.smoothingGroupMaps.TryGetValue(this.currentSmoothingGroup, out smoothingGroupMap))
            {
                smoothingGroupMap = new Dictionary<Tuple<int, int, int>, int>();
                this.smoothingGroupMaps.Add(this.currentSmoothingGroup, smoothingGroupMap);
            }
        }

        var fields = values.Split(Delimiters, System.StringSplitOptions.RemoveEmptyEntries);
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
                builder.CreateTextureCoordinates = false;
            }

            if (vni == int.MaxValue)
            {
                // turn off normals in the builder
                builder.CreateNormals = false;
            }

            // check if the texture coordinate index is valid
            if (builder.CreateTextureCoordinates && vti - 1 >= this.TextureCoordinates.Count)
            {
                if (this.IgnoreErrors)
                {
                    return;
                }

                throw new FileFormatException(
                        string.Format(
                            "Invalid texture coordinate index ({0}) on line {1}.", vti, this.currentLineNo));
            }

            // check if the normal index is valid
            if (builder.CreateNormals && vni - 1 >= this.Normals.Count)
            {
                if (this.IgnoreErrors)
                {
                    return;
                }

                throw new FileFormatException(
                        string.Format("Invalid normal index ({0}) on line {1}.", vni, this.currentLineNo));
            }

            bool addVertex = true;

            if (smoothingGroupMap != null)
            {
                var key = Tuple.Create(vi, vti, vni);

                if (smoothingGroupMap.TryGetValue(key, out int vix))
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
                positions.Add(this.Points[vi - 1].ToVector3());

                // add texture coordinate (if enabled)
                if (builder.CreateTextureCoordinates)
                {
                    textureCoordinates!.Add(this.TextureCoordinates[vti - 1].ToVector2());
                }

                // add normal (if enabled)
                if (builder.CreateNormals)
                {
                    normals!.Add(this.Normals[vni - 1].ToVector3());
                }
            }
        }

        if (faceIndices.Count <= 4)
        {
            // add triangles or quads
            builder.AddPolygon(faceIndices);
        }
        else
        {
            // add triangles by cutting ears algorithm
            // this algorithm is quite expensive...
            builder.AddPolygonByTriangulation(faceIndices);
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
            this.Normals.Add(new Vector3D(fields[0], -fields[2], fields[1]));
        }
        else
        {
            this.Normals.Add(new Vector3D(fields[0], fields[1], fields[2]));
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
        this.TextureCoordinates.Add(new Point(fields[0], 1 - fields[1]));
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
            this.Points.Add(new Point3D(fields[0], -fields[2], fields[1]));
        }
        else
        {
            this.Points.Add(new Point3D(fields[0], fields[1], fields[2]));
        }
    }

    /// <summary>
    /// Builds the model.
    /// </summary>
    /// <returns>A Model3D object.</returns>
    private Model3DGroup BuildModel()
    {
        Model3DGroup? modelGroup = null;
        this.Dispatch(() =>
        {
            modelGroup = new Model3DGroup();
            foreach (var g in this.Groups)
            {
                foreach (var gm in g.CreateModels())
                {
                    gm.SetName(g.Name);

                    if (this.Freeze)
                    {
                        gm.Freeze();
                    }

                    modelGroup.Children.Add(gm);
                }
            }

            if (this.Freeze)
            {
                modelGroup.Freeze();
            }
        });
        return modelGroup!;
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
        if (!string.IsNullOrEmpty(materialName) && this.Materials.TryGetValue(materialName, out MaterialDefinition? mat))
        {
            Material? m = null;
            this.Dispatch(() =>
            {
                m = mat.GetMaterial(this.TexturePath);
            });
            return m!;
        }

        return this.DefaultMaterial;
    }

    /// <summary>
    /// Loads a material library.
    /// </summary>
    /// <param name="mtlFile">
    /// The material file name.
    /// </param>
    private void LoadMaterialLib(string mtlFile)
    {
        string path = PathHelpers.GetFullPath(this.TexturePath, mtlFile);

        if (!File.Exists(path))
        {
            return;
        }

        using var materialReader = new StreamReader(path);
        ReadMaterial(materialReader);
    }

    /// <summary>
    /// Loads the material library from a streamreader
    /// </summary>
    /// <param name="materialReader"></param>
    private void ReadMaterial(StreamReader materialReader)
    {
        MaterialDefinition? currentMaterial = null;

        while (!materialReader.EndOfStream)
        {
            var line = materialReader.ReadLine();
            if (line == null)
            {
                break;
            }

            line = line.Trim();

            if (line.StartsWith("#") || line.Length == 0)
            {
                continue;
            }

            SplitLine(line, out string keyword, out string value);

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
                            currentMaterial = new MaterialDefinition(value);
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

    /// <summary>
    /// Sets the material for the current group.
    /// </summary>
    /// <param name="materialName">
    /// The material name.
    /// </param>
    private void SetMaterial(string materialName)
    {
        this.CurrentGroup.Material = this.CurrentMaterial = this.GetMaterial(materialName);
    }

    /// <summary>
    /// Represents a group in the obj file.
    /// </summary>
    public sealed class Group
    {
        /// <summary>
        /// List of mesh builders.
        /// </summary>
        private readonly List<MeshBuilder> meshBuilders;

        /// <summary>
        /// List of materials.
        /// </summary>
        private readonly List<Material> materials;

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the group.
        /// </param>
        /// <param name="material">The material of the group.</param>
        public Group(string name, Material material)
        {
            this.Name = name;
            this.meshBuilders = new List<MeshBuilder>();
            this.materials = new List<Material>();
            this.AddMesh(material);
        }

        /// <summary>
        /// Sets the material.
        /// </summary>
        /// <value>The material.</value>
        public Material Material
        {
            set
            {
                this.materials[^1] = value;
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
                return this.meshBuilders[^1]!;
            }
        }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Adds a mesh.
        /// </summary>
        /// <param name="material">The material of the group.</param>
        public void AddMesh(Material material)
        {
            var meshBuilder = new MeshBuilder(true, true);
            this.meshBuilders.Add(meshBuilder);
            this.materials.Add(material);
        }

        /// <summary>
        /// Creates the models of the group.
        /// </summary>
        /// <returns>The models.</returns>
        public IEnumerable<Model3D> CreateModels()
        {
            for (int i = 0; i < this.meshBuilders.Count; i++)
            {
                var material = this.materials[i];
                var mesh = this.meshBuilders[i].ToMesh();

                var model = new GeometryModel3D
                {
                    Geometry = mesh.ToWndMeshGeometry3D(),
                    Material = material,
                    BackMaterial = material
                };

                yield return model;
            }
        }
    }

    /// <summary>
    /// A material definition.
    /// </summary>
    /// <remarks>
    /// The file format is documented in http://en.wikipedia.org/wiki/Material_Template_Library.
    /// </remarks>
    public sealed class MaterialDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDefinition" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MaterialDefinition(string name)
        {
            this.Name = name;
            this.Dissolved = 1.0;
        }

        /// <summary>
        /// Gets or sets the alpha map.
        /// </summary>
        /// <value>The alpha map.</value>
        public string? AlphaMap { get; set; }

        /// <summary>
        /// Gets or sets the ambient color.
        /// </summary>
        /// <value>The ambient.</value>
        public Color Ambient { get; set; }

        /// <summary>
        /// Gets or sets the ambient map.
        /// </summary>
        /// <value>The ambient map.</value>
        public string? AmbientMap { get; set; }

        /// <summary>
        /// Gets or sets the bump map.
        /// </summary>
        /// <value>The bump map.</value>
        public string? BumpMap { get; set; }

        /// <summary>
        /// Gets or sets the diffuse color.
        /// </summary>
        /// <value>The diffuse.</value>
        public Color Diffuse { get; set; }

        /// <summary>
        /// Gets or sets the diffuse map.
        /// </summary>
        /// <value>The diffuse map.</value>
        public string? DiffuseMap { get; set; }

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
        /// Gets or sets the name of the material.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

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
        public string? SpecularMap { get; set; }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        /// <value>The material.</value>
        public Material? Material { get; set; }

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
            this.Material ??= this.CreateMaterial(texturePath);

            return this.Material;
        }

        /// <summary>
        /// Creates the material.
        /// </summary>
        /// <param name="texturePath">The texture path.</param>
        /// <returns>A WPF material.</returns>
        private Material CreateMaterial(string texturePath)
        {
            var mg = new MaterialGroup();
            mg.SetName(this.Name);

            // add the diffuse component
            if (this.DiffuseMap == null)
            {
                var diffuseBrush = new SolidColorBrush(this.Diffuse) { Opacity = this.Dissolved };
                mg.Children.Add(new DiffuseMaterial(diffuseBrush));
            }
            else
            {
                string path = PathHelpers.GetFullPath(texturePath, this.DiffuseMap);
                if (File.Exists(path))
                {
                    mg.Children.Add(new DiffuseMaterial(this.CreateTextureBrush(path)));
                }
            }

            // add the ambient components
            if (this.AmbientMap == null)
            {
                // ambient material is not supported by WPF?
            }
            else
            {
                string path = PathHelpers.GetFullPath(texturePath, this.AmbientMap);
                if (File.Exists(path))
                {
                    mg.Children.Add(new EmissiveMaterial(this.CreateTextureBrush(path)));
                }
            }

            // add the specular component
            if (this.Specular.R > 0 || this.Specular.G > 0 || this.Specular.B > 0)
            {
                mg.Children.Add(new SpecularMaterial(new SolidColorBrush(this.Specular), this.SpecularCoefficient));
            }

            return mg.Children.Count != 1 ? mg : mg.Children[0];
        }

        /// <summary>
        /// Creates a texture brush.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The brush.</returns>
        private ImageBrush CreateTextureBrush(string path)
        {
            var img = new BitmapImage(new Uri(path, UriKind.Relative));
            var textureBrush = new ImageBrush(img) { Opacity = this.Dissolved, ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.Tile };
            return textureBrush;
        }
    }

    /// <summary>
    /// Path helpers.
    /// </summary>
    private static class PathHelpers
    {
        /// <summary>
        /// Gets a full path.
        /// </summary>
        /// <param name="basePath">
        /// The base path.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        public static string GetFullPath(string basePath, string path)
        {
            if (path.Length > 1
                && (path[0] == Path.DirectorySeparatorChar || path[0] == Path.AltDirectorySeparatorChar)
                && (path[1] != Path.DirectorySeparatorChar && path[1] != Path.AltDirectorySeparatorChar))
            {
                path = path[1..];
            }

            if (string.IsNullOrWhiteSpace(basePath))
            {
                // If base path is non-existent, trying to set it to absolute path of the current folder.
                basePath = AppDomain.CurrentDomain.BaseDirectory;
            }

            return !string.IsNullOrWhiteSpace(basePath) ? Path.GetFullPath(Path.Combine(basePath, path)) : string.Empty;
        }
    }
}
