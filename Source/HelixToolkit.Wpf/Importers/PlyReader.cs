using System.IO;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace HelixToolkit.Wpf;

/// <summary>
/// Polygon File Format Reader.
/// </summary>
/// <remarks>
/// https://www.cc.gatech.edu/projects/large_models/ply.html
/// http://graphics.stanford.edu/data/3Dscanrep/
/// </remarks>
public sealed class PlyReader : ModelReader
{
    /// <summary>
    /// Initializes a new <see cref="PlyReader"/>.
    /// </summary>
    /// <param name="dispatcher"></param>
    public PlyReader(Dispatcher? dispatcher = null)
        : base(dispatcher)
    {
        Header = new PlyHeader();
        Body = new List<PlyElement>();
    }

    #region Public methods
    /// <summary>
    /// Reads the model from the specified stream.
    /// </summary>
    /// <param name="s">The stream.</param>
    /// <returns>A <see cref="Model3DGroup" />.</returns>
    public override Model3DGroup Read(Stream s)
    {
        this.Load(s);
        return this.CreateModel3D();
    }

    /// <summary>
    /// Reads the model from the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The model.</returns>
    public override Model3DGroup Read(string path)
    {
        Load(path);
        return this.CreateModel3D();
    }

    /// <summary>
    /// Creates a mesh from the loaded file.
    /// </summary>
    /// <returns>
    /// A <see cref="Mesh3D" />.
    /// </returns>
    public Mesh3D CreateMesh()
    {
        var mesh = new Mesh3D();

        var vertexElement = Body.Find(item => item.Name == "vertex");
        if (vertexElement != null && vertexElement.Count > 0)
        {
            foreach (var vertProp in vertexElement.Instances)
            {
                var vertPropList = vertProp.ToList();
                var xProp = vertPropList.Find(item => !item.IsList && item.Name == "x");
                var yProp = vertPropList.Find(item => !item.IsList && item.Name == "y");
                var zProp = vertPropList.Find(item => !item.IsList && item.Name == "z");

                if (xProp != null && yProp != null && zProp != null)
                {
                    var xCoord = double.Parse(xProp.Value?.ToString() ?? "0");
                    var yCoord = double.Parse(yProp.Value?.ToString() ?? "0");
                    var zCoord = double.Parse(zProp.Value?.ToString() ?? "0");
                    var vertex = new Point3D(xCoord, yCoord, zCoord);
                    mesh.Vertices.Add(vertex);
                }

                //var sProp = vertPropList.Find(item => !item.IsList && item.Name == "s");
                //var tProp = vertPropList.Find(item => !item.IsList && item.Name == "t");

                //if (sProp != null && tProp != null)
                //{
                //    var sCoord = double.Parse(sProp.Value.ToString());
                //    var tCoord = double.Parse(tProp.Value.ToString());
                //    var texturePt = new Point(sCoord, tCoord);
                //    mesh.TextureCoordinates.Add(texturePt);
                //}
            }
        }

        var faceElement = Body.Find(item => item.Name == "face");
        if (faceElement != null && faceElement.Count > 0)
        {
            foreach (var faceProp in faceElement.Instances)
            {
                var vertexIndicesProperties = (from item in faceProp where item.IsList && item.Name == "vertex_indices" || item.Name == "vertex_index" select item).ToArray();
                if (vertexIndicesProperties.Length > 0)
                {
                    var vertexIndices = new List<int>();
                    object[]? values = vertexIndicesProperties[0].ListContentValues;
                    if (values is not null)
                    {
                        foreach (var item in values)
                        {
                            vertexIndices.Add(Convert.ToInt32(item ?? "0"));
                        }
                    }
                    mesh.Faces.Add(vertexIndices.ToArray());
                }

            }
        }

        return mesh;
    }

    /// <summary>
    /// Creates a <see cref="MeshGeometry3D" /> object from the loaded file. Polygons are triangulated using triangle fans.
    /// </summary>
    /// <returns>
    /// A <see cref="MeshGeometry3D" />.
    /// </returns>
    public MeshGeometry3D CreateMeshGeometry3D()
    {
        var mesh = CreateMesh();
        return mesh.ToMeshGeometry3D();
    }

    /// <summary>
    /// Creates a <see cref="Model3DGroup" /> from the loaded file.
    /// </summary>
    /// <returns>A <see cref="Model3DGroup" />.</returns>
    public Model3DGroup CreateModel3D()
    {
        Model3DGroup? modelGroup = null;
        this.Dispatch(() =>
        {
            modelGroup = new Model3DGroup();
            var g = this.CreateMeshGeometry3D();
            var gm = new GeometryModel3D
            {
                Geometry = g,
                Material = this.DefaultMaterial,
                BackMaterial = DefaultMaterial
            };
            if (this.Freeze)
            {
                gm.Freeze();
            }
            modelGroup.Children.Add(gm);
            if (this.Freeze)
            {
                modelGroup.Freeze();
            }
        });

        return modelGroup!;
    }

    /// <summary>
    /// Loads a ply file from the <see cref="Stream"/>.
    /// </summary>
    /// <param name="s">The stream containing the ply file.</param>
    public void Load(Stream s)
    {
        Header = new PlyHeader();
        Body = new List<PlyElement>();
        var plyFileData = LoadPlyFile(s);

        Header = plyFileData.Item1;
        Body = plyFileData.Item2;
        //var location = "";
        //DumpFileAsASCII(location, plyHeader, plyBody);
    }

    /// <summary>
    /// Loads a plyfile from the specified filepath.
    /// </summary>
    /// <param name="path">The filepath.</param>
    public void Load(string path)
    {
        this.Directory = Path.GetDirectoryName(path) ?? "";
        using var stream = GetResourceStream(path);

        if (stream is null)
        {
            return;
        }

        Load(stream);
    }

    /// <summary>
    /// Loads a ply file from the stream but doesn't consume it.
    /// </summary>
    /// <param name="plyFileStream"></param>
    /// <returns></returns>
    /// <remarks>
    /// This could be useful when we have several streams of plyfiles to reconstruct
    /// into a single mesh, without updating the Header and Body properties of this reader.
    /// </remarks>
    public Tuple<PlyHeader, List<PlyElement>> LoadPlyFile(Stream plyFileStream)
    {
        var headerLines = new List<string>();
        Int64 startPosition = 0;
        var textReader = new StreamReader(plyFileStream);
        plyFileStream.Position = 0;
        plyFileStream.Seek(0, SeekOrigin.Begin);

        var readingHeader = true;
        var headerLineBuilders = new List<StringBuilder>() { new StringBuilder() };
        var newLineCharMet = false;

        while (readingHeader && textReader.EndOfStream == false)
        {
            var currentChar = (char)textReader.Read();

            if (currentChar == '\r' || currentChar == '\n')
            {
                if (newLineCharMet)
                    startPosition++;
                else
                {
                    newLineCharMet = true;
                    headerLineBuilders.Add(new StringBuilder());
                    startPosition++;
                }
            }
            else
            {
                if (headerLineBuilders.Count > 2 && headerLineBuilders[headerLineBuilders.Count - 2].ToString() == "end_header")
                {
                    headerLineBuilders.RemoveAt(headerLineBuilders.Count - 1);
                    readingHeader = false;
                }
                else
                {
                    newLineCharMet = false;
                    headerLineBuilders.Last().Append(currentChar.ToString());
                    startPosition++;
                }
            }
        }

        foreach (var headerLineBuilder in headerLineBuilders)
        {
            headerLines.Add(headerLineBuilder.ToString());
        }

        var plyHeader = ReadHeader(headerLines.ToArray());
        var plyBody = new List<PlyElement>();
        plyFileStream.Position = startPosition;

        switch (plyHeader.FormatType)
        {
            case PlyFormatTypes.ascii:
                plyBody = ReadASCII(plyFileStream, plyHeader);
                break;
            case PlyFormatTypes.binary_big_endian:
                plyBody = ReadBinary(plyFileStream, plyHeader, true);
                break;
            case PlyFormatTypes.binary_little_endian:
                plyBody = ReadBinary(plyFileStream, plyHeader, false);
                break;
            default:
                break;
        }

        textReader.Dispose();
        textReader.Close();
        return new Tuple<PlyHeader, List<PlyElement>>(plyHeader, plyBody);
    }

    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the header of the loaded ply file.
    /// </summary>
    public PlyHeader Header { get; private set; }

    /// <summary>
    /// Gets or sets the body of the loaded ply file.
    /// </summary>
    public List<PlyElement> Body { get; private set; }
    #endregion

    #region Data
    /// <summary>
    /// The supported version of the ply format.
    /// </summary>
    public static readonly Version SUPPORTEDVERSION = new Version(1, 0, 0);

    /// <summary>
    /// Specifies the types of ply model formats.
    /// </summary>
    public enum PlyFormatTypes
    {
        /// <summary>
        /// ASCII ply format.
        /// </summary>
        ascii,
        /// <summary>
        /// Binary big endian ply format.
        /// </summary>
        binary_big_endian,
        /// <summary>
        /// Binary little endian ply format.
        /// </summary>
        binary_little_endian
    }

    /// <summary>
    /// Specifies the types of ply data types.
    /// </summary>
    public enum PlyDataTypes
    {
        /// <summary>
        /// character
        /// </summary>
        _char,
        /// <summary>
        /// unsigned character
        /// </summary>
        _uchar,
        /// <summary>
        /// short integer
        /// </summary>
        _short,
        /// <summary>
        /// unsigned short integer
        /// </summary>
        _ushort,
        /// <summary>
        /// integer
        /// </summary>
        _int,
        /// <summary>
        /// integer
        /// </summary>
        _int32,
        /// <summary>
        /// unsigned integer
        /// </summary>
        _uint,
        /// <summary>
        /// unsigned integer
        /// </summary>
        _uint8,
        /// <summary>
        /// single-precision float
        /// </summary>
        _float,
        /// <summary>
        /// single-precision float
        /// </summary>
        _float32,
        /// <summary>
        /// double-precision float
        /// </summary>
        _double,
    }

    /// <summary>
    /// Specifies the types of items in a ply header.
    /// </summary>
    public enum PlyHeaderItems
    {
        /// <summary>
        /// The beginning of a ply file.
        /// </summary>
        ply,
        /// <summary>
        /// The format of a ply file.
        /// </summary>
        format,
        /// <summary>
        /// A comment in a ply file.
        /// </summary>
        comment,
        /// <summary>
        /// An object info in a ply header
        /// </summary>
        obj_info,
        /// <summary>
        /// The declaration of an element.
        /// </summary>
        element,
        /// <summary>
        /// The property to be attached to an element.
        /// </summary>
        property,
        /// <summary>
        /// The end of header declaration.
        /// </summary>
        end_header
    }

    /// <summary>
    /// Represents a ply element.
    /// </summary>
    public sealed class PlyElement
    {
        /// <summary>
        /// Initializes a new <see cref="PlyElement"/>.
        /// </summary>
        /// <param name="name">The name of this element.</param>
        /// <param name="count">The number of instances of this element.</param>
        /// <param name="instances">The instances of this elements properties.</param>
        public PlyElement(string name, int count, List<PlyProperty[]> instances)
        {
            Name = name;
            Count = count;
            Instances = instances;
        }

        /// <summary>
        /// The name of this element.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The number of times this element is expected to appear.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// The instances of this elements properties.
        /// </summary>
        /// <remarks>
        /// An element can have any number of properties and that list
        /// of properties can appear <see cref="Count"/> number of times.
        /// This property holds those values.
        /// </remarks>
        public List<PlyProperty[]> Instances { get; }
    }

    /// <summary>
    /// Represents a property of a <see cref="PlyElement"/>.
    /// </summary>
    public sealed class PlyProperty
    {
        /// <summary>
        /// Initializes a new ply property with the specified values.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="isList">Specifies whether the property is a list or not.</param>
        /// <param name="listContentType">The type of contents in the list if it is a list.</param>
        /// <param name="listContentValues">The items in the property's list.</param>
        public PlyProperty(string name, PlyDataTypes type, object? value, bool isList, PlyDataTypes listContentType, object[]? listContentValues)
        {
            Name = name;
            Type = type;
            Value = value;
            IsList = isList;
            ListContentType = listContentType;
            ListContentValues = listContentValues;
        }

        /// <summary>
        /// The name of this property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// For a scalar property: the type of value it holds.<para/>
        /// For a vector property: the type of the items count value.
        /// </summary>
        /// <remarks>
        /// A scalar property is a property where <see cref="IsList"/> is false.
        /// </remarks>
        public PlyDataTypes Type { get; }

        /// <summary>
        /// For a scalar property: The value of this property.<para/>
        /// For a vector property: The number of items in the list.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Specifies whether this property is a scalar or vector (list).
        /// </summary>
        public bool IsList { get; }

        /// <summary>
        /// The type of items in the list.
        /// </summary>
        public PlyDataTypes ListContentType { get; }

        /// <summary>
        /// The value of the items in the list.
        /// </summary>
        public object[]? ListContentValues { get; }
    }

    /// <summary>
    /// Represents the header of a ply file.
    /// </summary>
    public sealed class PlyHeader
    {
        /// <summary>
        /// Initializes a ply header with type <see cref="PlyFormatTypes.ascii"/> and no elements, comments and object infos.
        /// </summary>
        public PlyHeader()
        {
            FormatType = PlyFormatTypes.ascii;
            Version = SUPPORTEDVERSION;
            Comments = Array.Empty<string>();
            ObjectInfos = Array.Empty<Tuple<string, string>>();
            Elements = Array.Empty<PlyElement>();
        }

        /// <summary>
        /// Initializes a new Ply header with the given values.
        /// </summary>
        /// <param name="plyFormatType"></param>
        /// <param name="version"></param>
        /// <param name="elements"></param>
        /// <param name="objInfos"></param>
        /// <param name="comments"></param>
        public PlyHeader(PlyFormatTypes plyFormatType, Version version, PlyElement[] elements, Tuple<string, string>[] objInfos, string[] comments)
        {
            FormatType = plyFormatType;
            Version = version;
            ObjectInfos = objInfos;
            Comments = comments;
            Elements = elements;
        }

        /// <summary>
        /// The format of the ply file's body.
        /// </summary>
        public PlyFormatTypes FormatType { get; }

        /// <summary>
        /// The version of the ply file.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Gets the comments made in the file.
        /// </summary>
        public string[] Comments { get; }

        /// <summary>
        /// Gets the object informations for this file (mostly producer independent).
        /// </summary>
        public Tuple<string, string>[] ObjectInfos { get; }

        /// <summary>
        /// Gets the elements declared in the header.
        /// </summary>
        public PlyElement[] Elements { get; }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Reads and validates the header lines of a ply file.
    /// </summary>
    /// <param name="headerLines">The lines to read.</param>
    /// <returns></returns>
    private PlyHeader ReadHeader(string[] headerLines)
    {
        if (headerLines.Length > 2 && (PlyHeaderItems)Enum.Parse(typeof(PlyHeaderItems), headerLines[0]) == PlyHeaderItems.ply
            && (PlyHeaderItems)Enum.Parse(typeof(PlyHeaderItems), headerLines[headerLines.Length - 1]) == PlyHeaderItems.end_header)
        {
            var formatSpecLineParts = headerLines[1].Split(' ');
            var formatStr = formatSpecLineParts[0];
            var formatTypeStr = formatSpecLineParts[1];
            var fileVersion = Version.Parse(formatSpecLineParts[2]);

            if ((PlyHeaderItems)Enum.Parse(typeof(PlyHeaderItems), formatStr) == PlyHeaderItems.format
                && Enum.TryParse(formatTypeStr, out PlyFormatTypes formatType) && fileVersion <= SUPPORTEDVERSION)
            {
                var comments = new List<string>();
                var objInfos = new List<Tuple<string, string>>();
                var elements = new List<PlyElement>();

                for (int i = 2; i < headerLines.Length - 1; i++)
                {
                    var lineParts = headerLines[i].Split(' ');
                    if (Enum.TryParse(lineParts[0], out PlyHeaderItems headerItemType))
                    {
                        switch (headerItemType)
                        {
                            case PlyHeaderItems.element:
                                {
                                    if (lineParts.Length == 3)
                                    {
                                        var elementName = lineParts[1];
                                        var elementCount = int.Parse(lineParts[2]);
                                        var element = new PlyElement(elementName, elementCount, new List<PlyProperty[]> { Array.Empty<PlyProperty>() });
                                        elements.Add(element);
                                    }
                                    break;
                                }
                            case PlyHeaderItems.property:
                                {
                                    if (lineParts.Length >= 3 && elements.Count > 0)
                                    {
                                        if (lineParts[1] != "list" && lineParts.Length == 3)
                                        {
                                            if (Enum.TryParse($"_{lineParts[1]}", out PlyDataTypes propertyType))
                                            {
                                                var propertyName = lineParts[2];

                                                var property = new PlyProperty(propertyName, propertyType, null, false, PlyDataTypes._char, null);

                                                var newPropertyList = new List<PlyProperty>();
                                                for (int j = 0; j < elements.Last().Instances[0].Length; j++)
                                                {
                                                    newPropertyList.Add(elements.Last().Instances[0][j]);
                                                }
                                                newPropertyList.Add(property);
                                                elements.Last().Instances[0] = newPropertyList.ToArray();
                                            }
                                            else
                                                throw new InvalidDataException($"Invalid data type, {lineParts[1]}.");
                                        }
                                        else if (lineParts[1] == "list" && lineParts.Length == 5)
                                        {
                                            //array property
                                            if (Enum.TryParse($"_{lineParts[2]}", out PlyDataTypes propertyType) && Enum.TryParse($"_{lineParts[3]}", out PlyDataTypes listContentType))
                                            {
                                                var propertyName = lineParts[4];

                                                var property = new PlyProperty(propertyName, propertyType, null, true, listContentType, null);

                                                var newPropertyList = new List<PlyProperty>();
                                                for (int j = 0; j < elements.Last().Instances[0].Length; j++)
                                                {
                                                    newPropertyList.Add(elements.Last().Instances[0][j]);
                                                }
                                                newPropertyList.Add(property);
                                                elements.Last().Instances[0] = newPropertyList.ToArray();

                                            }
                                            else
                                                throw new InvalidDataException($"Invalid data type, {lineParts[1]}.");
                                        }
                                        else
                                            throw new InvalidDataException("Invalid property definition.");
                                    }
                                    break;
                                }
                            case PlyHeaderItems.obj_info:
                                {
                                    if (lineParts.Length == 3)
                                    {
                                        objInfos.Add(new Tuple<string, string>(lineParts[1], lineParts[2]));
                                    }
                                    else
                                    {
                                        objInfos.Add(new Tuple<string, string>($"htk_info_{objInfos.Count}", headerLines[i].Substring(lineParts[0].Length + 1)));
                                    }
                                    break;
                                }
                            case PlyHeaderItems.comment:
                                {
                                    comments.Add(headerLines[i].Substring(lineParts[0].Length + 1));
                                    break;
                                }
                            default:
                                {
                                    throw new InvalidDataException($"Unknown header item, {lineParts[0]}.");
                                }
                        }
                    }
                    else
                        throw new InvalidDataException($"Unknown header item, {lineParts[0]}.");
                }

                var plyHeader = new PlyHeader(formatType, fileVersion, elements.ToArray(), objInfos.ToArray(), comments.ToArray());
                return plyHeader;
            }
            else
            {
                throw new InvalidDataException("Invalid format specification.");
            }
        }
        else
            throw new InvalidDataException("Invalid ply file.");
    }

    /// <summary>
    /// Converts the value of a property to the specified data type.
    /// </summary>
    /// <param name="plyDataType">The type to convert to.</param>
    /// <param name="propValue">The value to convert.</param>
    /// <returns></returns>
    private object ConvertPropValueASCII(PlyDataTypes plyDataType, string propValue)
    {
        object result = "";
        switch (plyDataType)
        {
            case PlyDataTypes._char:
                result = Convert.ToChar(propValue);
                break;
            case PlyDataTypes._uint8:
            case PlyDataTypes._uchar:
                result = Convert.ToByte(propValue);
                break;
            case PlyDataTypes._short:
                result = short.Parse(propValue);
                break;
            case PlyDataTypes._ushort:
                result = ushort.Parse(propValue);
                break;
            case PlyDataTypes._int:
            case PlyDataTypes._int32:
                result = int.Parse(propValue);
                break;
            case PlyDataTypes._uint:
                result = uint.Parse(propValue);
                break;
            case PlyDataTypes._float:
            case PlyDataTypes._float32:
                result = float.Parse(propValue);
                break;
            case PlyDataTypes._double:
                result = double.Parse(propValue);
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary>
    /// Reads the value of a property in the specified data type.
    /// </summary>
    /// <param name="plyDataType"></param>
    /// <param name="reader"></param>
    /// <param name="bigEndian"></param>
    /// <returns></returns>
    private object ConvertPropValueBinary(PlyDataTypes plyDataType, BinaryReader reader, bool bigEndian)
    {
        object result = "";
        var reverseBytes = bigEndian && BitConverter.IsLittleEndian;
        switch (plyDataType)
        {
            case PlyDataTypes._char:
                {
                    result = reverseBytes ? BitConverter.ToChar(reader.ReadBytes(1).Reverse().ToArray(), 0) : reader.ReadChar();
                    break;
                }
            case PlyDataTypes._uint8:
            case PlyDataTypes._uchar:
                {
                    result = reader.ReadByte();
                    break;
                }
            case PlyDataTypes._short:
                {
                    result = reverseBytes ? BitConverter.ToInt16(reader.ReadBytes(2).Reverse().ToArray(), 0) : reader.ReadInt16();
                    break;
                }
            case PlyDataTypes._ushort:
                {
                    result = reverseBytes ? BitConverter.ToUInt16(reader.ReadBytes(2).Reverse().ToArray(), 0) : reader.ReadUInt16();
                    break;
                }
            case PlyDataTypes._int:
            case PlyDataTypes._int32:
                {
                    result = reverseBytes ? BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(), 0) : reader.ReadInt32();
                    break;
                }
            case PlyDataTypes._uint:
                {
                    result = reverseBytes ? BitConverter.ToUInt32(reader.ReadBytes(4).Reverse().ToArray(), 0) : reader.ReadUInt32();
                    break;
                }
            case PlyDataTypes._float:
            case PlyDataTypes._float32:
                {
                    result = reverseBytes ? BitConverter.ToSingle(reader.ReadBytes(4).Reverse().ToArray(), 0) : reader.ReadSingle();
                    break;
                }
            case PlyDataTypes._double:
                {
                    result = reverseBytes ? BitConverter.ToDouble(reader.ReadBytes(8).Reverse().ToArray(), 0) : reader.ReadDouble();
                    break;
                }
            default:
                throw new InvalidOperationException("Unimplemented data conversion.");
        }

        return result;
    }

    /// <summary>
    /// Reads a ply file in an ascii format.
    /// </summary>
    /// <param name="s">The stream to read from.</param>
    /// <param name="plyHeader">The header of the ply file.</param>
    private List<PlyElement> ReadASCII(Stream s, PlyHeader plyHeader)
    {
        var plyBody = new List<PlyElement>();
        using (var reader = new StreamReader(s, Encoding.ASCII))
        {
            //The index of the element being read from the header.
            var currentElementIdx = 0;
            //The index of the instances of the current element.
            var currentIdx = 0;
            var currentPlyElementProperties = new List<PlyProperty[]>();
            var currentHeadElement = plyHeader.Elements[currentElementIdx];
            var debLineNo = 0;
            while (!reader.EndOfStream)
            {
                debLineNo++;
                if (currentElementIdx < plyHeader.Elements.Length)
                {
                    var currentLine = reader.ReadLine()?.Trim();
                    var lineDataArr = currentLine?.Split(' ');
                readElementInstance:
                    if (currentIdx < currentHeadElement.Count)
                    {
                        var plyHeadProperties = currentHeadElement.Instances[0];
                        //The number of items from the current lineDataArr start position(0)
                        //This allows an element to contain multiple lists and properties
                        var idxOffset = 0;
                        var plyBodyProperties = new List<PlyProperty>();
                        for (int i = 0; i < plyHeadProperties.Length; i++)
                        {
                            var currentPlyHeadProp = plyHeadProperties[i];
                            if (currentPlyHeadProp.IsList)
                            {
                                var itemsNumStr = ConvertPropValueASCII(currentPlyHeadProp.Type, lineDataArr?[idxOffset] ?? "");
                                if (int.TryParse(itemsNumStr.ToString(), out int itemsNum))
                                {
                                    idxOffset++;

                                    var listContentItems = new List<object>();
                                    for (int j = 0; j < itemsNum; j++)
                                    {
                                        var listContentItem = ConvertPropValueASCII(currentPlyHeadProp.ListContentType, lineDataArr?[idxOffset] ?? "");
                                        listContentItems.Add(listContentItem);
                                        idxOffset++;
                                    }
                                    var plyBodyProp = new PlyProperty(currentPlyHeadProp.Name, currentPlyHeadProp.Type,
                                        itemsNum, currentPlyHeadProp.IsList, currentPlyHeadProp.ListContentType, listContentItems.ToArray());
                                    plyBodyProperties.Add(plyBodyProp);
                                }
                                else
                                    throw new InvalidDataException("Invalid list items count.");
                            }
                            else
                            {
                                var plyBodyProp = new PlyProperty(currentPlyHeadProp.Name, currentPlyHeadProp.Type,
                                    ConvertPropValueASCII(currentPlyHeadProp.Type, lineDataArr?[idxOffset] ?? ""), currentPlyHeadProp.IsList, currentPlyHeadProp.ListContentType, null);
                                plyBodyProperties.Add(plyBodyProp);
                                idxOffset++;
                            }
                        }

                        currentPlyElementProperties.Add(plyBodyProperties.ToArray());
                        currentIdx++;
                    }
                    else if (currentIdx == currentHeadElement.Count)
                    {
                        var plyBodyElement = new PlyElement(currentHeadElement.Name, currentHeadElement.Count, currentPlyElementProperties);
                        plyBody.Add(plyBodyElement);

                        currentElementIdx++;
                        currentIdx = 0;
                        currentPlyElementProperties = new List<PlyProperty[]>();
                        currentHeadElement = plyHeader.Elements[currentElementIdx];
                        goto readElementInstance;
                    }
                    else
                    {
                        throw new InvalidOperationException("Index was pushed too far out.");
                    }
                }
                else if (currentElementIdx == plyHeader.Elements.Length)
                {

                }
            }

            var lastPlyBodyElement = new PlyElement(currentHeadElement.Name, currentHeadElement.Count, currentPlyElementProperties);
            plyBody.Add(lastPlyBodyElement);
        }
        return plyBody;
    }

    /// <summary>
    /// Reads a ply file in a binary big endian format or in a binary little endian format.
    /// </summary>
    /// <param name="s">The stream to read from.</param>
    /// <param name="plyHeader">The header of the ply file.</param>
    /// <param name="bigEndian">Specifies whether the byte order is big endian or little endian.</param>
    /// <returns>
    /// The list of Ply elements declared in the header.
    /// </returns>
    private List<PlyElement> ReadBinary(Stream s, PlyHeader plyHeader, bool bigEndian)
    {
        var plyBody = new List<PlyElement>();
        var streamEncoding = bigEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;
        using (var reader = new BinaryReader(s, streamEncoding))
        {
            for (int i = 0; i < plyHeader.Elements.Length; i++)
            {
                var currentHeadElement = plyHeader.Elements[i];
                var currentElementInstanceProperties = new List<PlyProperty[]>();

                for (int j = 0; j < currentHeadElement.Count; j++)
                {
                    var currentInstanceProperties = new List<PlyProperty>();

                    for (int k = 0; k < currentHeadElement.Instances[0].Length; k++)
                    {
                        var currentHeadProp = currentHeadElement.Instances[0][k];
                        if (currentHeadProp.IsList)
                        {
                            var itemsNumStr = ConvertPropValueBinary(currentHeadProp.Type, reader, bigEndian);
                            if (int.TryParse(itemsNumStr.ToString(), out int itemsNum))
                            {
                                var listContentItems = new List<object>();
                                for (int l = 0; l < itemsNum; l++)
                                {
                                    var listContentItem = ConvertPropValueBinary(currentHeadProp.ListContentType, reader, bigEndian);
                                    listContentItems.Add(listContentItem);
                                }
                                var plyProp = new PlyProperty(currentHeadProp.Name, currentHeadProp.Type,
                                    itemsNum, currentHeadProp.IsList, currentHeadProp.ListContentType, listContentItems.ToArray());
                                currentInstanceProperties.Add(plyProp);
                            }
                            else
                                throw new InvalidDataException("Invalid list items count.");
                        }
                        else
                        {
                            var newProperty = new PlyProperty(currentHeadProp.Name, currentHeadProp.Type,
                                ConvertPropValueBinary(currentHeadProp.Type, reader, bigEndian), currentHeadProp.IsList, currentHeadProp.ListContentType, currentHeadProp.ListContentValues);
                            currentInstanceProperties.Add(newProperty);
                        }
                    }

                    currentElementInstanceProperties.Add(currentInstanceProperties.ToArray());
                }

                var plyElement = new PlyElement(currentHeadElement.Name, currentHeadElement.Count, currentElementInstanceProperties);
                plyBody.Add(plyElement);
            }
        }


        return plyBody;
    }

    /// <summary>
    /// Writes the ply header and body to a ply file in an ASCII format.
    /// </summary>
    /// <param name="dumpPath"></param>
    /// <param name="plyHeader"></param>
    /// <param name="plyBody"></param>
    private void DumpAsASCII(string dumpPath, PlyHeader plyHeader, List<PlyElement> plyBody)
    {
        using (var fs = new FileStream(dumpPath, FileMode.Create, FileAccess.Write))
        {
            using (var sw = new StreamWriter(fs))
            {
                #region Header
                sw.WriteLine("ply");
                sw.WriteLine("format ascii 1.0");
                foreach (var comment in plyHeader.Comments)
                {
                    sw.WriteLine($"comment {comment}");
                }

                foreach (var objInfo in plyHeader.ObjectInfos)
                {
                    sw.WriteLine($"obj_info {objInfo.Item1} {objInfo.Item2}");
                }

                foreach (var element in plyHeader.Elements)
                {
                    sw.WriteLine($"element {element.Name} {element.Count}");
                    foreach (var propertyTemplate in element.Instances[0])
                    {
                        if (propertyTemplate.IsList)
                        {
                            sw.WriteLine($"property list {propertyTemplate.Type.ToString().Substring(1)} {propertyTemplate.ListContentType.ToString().Substring(1)} {propertyTemplate.Name}");
                        }
                        else
                        {
                            sw.WriteLine($"property {propertyTemplate.Type.ToString().Substring(1)} {propertyTemplate.Name}");
                        }
                    }
                }
                sw.WriteLine("end_header");
                #endregion

                #region Body
                foreach (var element in plyBody)
                {
                    foreach (var instances in element.Instances)
                    {
                        var instanceBuilder = new StringBuilder();
                        foreach (var property in instances)
                        {
                            if (property.IsList)
                            {
                                instanceBuilder.Append($" {property.ListContentValues?.Length}");
                                for (int i = 0; i < property.ListContentValues?.Length; i++)
                                {
                                    instanceBuilder.Append($" {property.ListContentValues[i]}");
                                }
                            }
                            else
                            {
                                instanceBuilder.Append($" {property.Value?.ToString()}");
                            }
                        }
                        sw.WriteLine(instanceBuilder.ToString().Trim());
                    }
                }
                #endregion
            }
        }
    }

    #endregion
}
