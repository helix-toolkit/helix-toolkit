// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StudioReader.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   A 3D Studio file reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A 3D Studio file reader.
    /// </summary>
    public class StudioReader : IModelReader
    {
        //// http://faydoc.tripod.com/formats/3ds.htm
        //// http://www.gametutorials.com
        //// http://code.google.com/p/lib3ds/
        //// http://blogs.msdn.com/b/danlehen/archive/2005/10/09/478923.aspx

        /// <summary>
        /// The materials.
        /// </summary>
        private readonly Dictionary<string, Material> materials = new Dictionary<string, Material>();

        /// <summary>
        /// The chunk id.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        private enum ChunkID
        {
            //// Primary chunk

            MAIN3DS = 0x4D4D,

            // Main Chunks
            EDIT3DS = 0x3D3D, // this is the start of the editor config
            KEYF3DS = 0xB000, // this is the start of the keyframer config
            VERSION = 0x0002,
            MESHVERSION = 0x3D3E,

            // sub defines of EDIT3DS
            EDIT_MATERIAL = 0xAFFF,
            EDIT_CONFIG1 = 0x0100,
            EDIT_CONFIG2 = 0x3E3D,
            EDIT_VIEW_P1 = 0x7012,
            EDIT_VIEW_P2 = 0x7011,
            EDIT_VIEW_P3 = 0x7020,
            EDIT_VIEW1 = 0x7001,
            EDIT_BACKGR = 0x1200,
            EDIT_AMBIENT = 0x2100,
            EDIT_OBJECT = 0x4000,
            EDIT_UNKNW01 = 0x1100,
            EDIT_UNKNW02 = 0x1201,
            EDIT_UNKNW03 = 0x1300,
            EDIT_UNKNW04 = 0x1400,
            EDIT_UNKNW05 = 0x1420,
            EDIT_UNKNW06 = 0x1450,
            EDIT_UNKNW07 = 0x1500,
            EDIT_UNKNW08 = 0x2200,
            EDIT_UNKNW09 = 0x2201,
            EDIT_UNKNW10 = 0x2210,
            EDIT_UNKNW11 = 0x2300,
            EDIT_UNKNW12 = 0x2302,
            EDIT_UNKNW13 = 0x3000,
            EDIT_UNKNW14 = 0xAFFF,

            // sub defines of EDIT_MATERIAL
            MAT_NAME01 = 0xA000,
            MAT_LUMINANCE = 0xA010,
            MAT_DIFFUSE = 0xA020,
            MAT_SPECULAR = 0xA030,
            MAT_SHININESS = 0xA040,
            MAT_MAP = 0xA200,
            MAT_MAPFILE = 0xA300,

            // sub defines of EDIT_OBJECT
            OBJ_TRIMESH = 0x4100,
            OBJ_LIGHT = 0x4600,
            OBJ_CAMERA = 0x4700,
            OBJ_UNKNWN01 = 0x4010,
            OBJ_UNKNWN02 = 0x4012, // Could be shadow

            // sub defines of OBJ_CAMERA
            CAM_UNKNWN01 = 0x4710,
            CAM_UNKNWN02 = 0x4720,

            // sub defines of OBJ_LIGHT
            LIT_OFF = 0x4620,
            LIT_SPOT = 0x4610,
            LIT_UNKNWN01 = 0x465A,

            // sub defines of OBJ_TRIMESH
            TRI_VERTEXL = 0x4110,
            TRI_FACEL2 = 0x4111,
            TRI_FACEL1 = 0x4120,
            TRI_FACEMAT = 0x4130,
            TRI_TEXCOORD = 0x4140,
            TRI_SMOOTH = 0x4150,
            TRI_LOCAL = 0x4160,
            TRI_VISIBLE = 0x4165,

            // sub defs of KEYF3DS
            KEYF_UNKNWN01 = 0xB009,
            KEYF_UNKNWN02 = 0xB00A,
            KEYF_FRAMES = 0xB008,
            KEYF_OBJDES = 0xB002,
            KEYF_HIERARCHY = 0xB030,
            KFNAME = 0xB010,

            // these define the different color chunk types
            COL_RGB = 0x0010,
            COL_TRU = 0x0011, // RGB24
            COL_UNK = 0x0013,

            // defines for viewport chunks
            TOP = 0x0001,
            BOTTOM = 0x0002,
            LEFT = 0x0003,
            RIGHT = 0x0004,
            FRONT = 0x0005,
            BACK = 0x0006,
            USER = 0x0007,
            CAMERA = 0x0008, // = 0xFFFF is the actual code read from file
            LIGHT = 0x0009,
            DISABLED = 0x0010,
            BOGUS = 0x0011,
            // ReSharper restore UnusedMember.Local
            // ReSharper restore InconsistentNaming
        }

        /// <summary>
        /// Gets or sets the texture path.
        /// </summary>
        /// <value> The texture path. </value>
        public string TexturePath { get; set; }

        /// <summary>
        /// Gets or sets Model.
        /// </summary>
        private Model3DGroup Model { get; set; }

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
        /// <returns>
        /// The model.
        /// </returns>
        public Model3DGroup Read(Stream s)
        {
            using (var reader = new BinaryReader(s))
            {
                long length = reader.BaseStream.Length;

                // http://gpwiki.org/index.php/Loading_3ds_files
                // http://www.flipcode.com/archives/3DS_File_Loader.shtml
                // http://sandy.googlecode.com/svn/trunk/sandy/as3/branches/3.0.2/src/sandy/parser/Parser3DS.as
                var headerId = this.ReadChunkId(reader);
                if (headerId != ChunkID.MAIN3DS)
                {
                    throw new FileFormatException("Unknown file");
                }

                int headerSize = this.ReadChunkSize(reader);
                if (headerSize != length)
                {
                    throw new FileFormatException("Incomplete file (file length does not match header)");
                }

                this.Model = new Model3DGroup();

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var id = this.ReadChunkId(reader);
                    int size = this.ReadChunkSize(reader);

                    switch (id)
                    {
                        case ChunkID.EDIT_MATERIAL:
                            this.ReadMaterial(reader, size);
                            break;
                        case ChunkID.EDIT_OBJECT:
                            this.ReadObject(reader, size);
                            break;
                        case ChunkID.EDIT3DS:
                        case ChunkID.OBJ_CAMERA:
                        case ChunkID.OBJ_LIGHT:
                        case ChunkID.OBJ_TRIMESH:

                            // don't read the whole chunk, read the sub-defines...
                            break;

                        default:

                            // download the whole chunk
                            this.ReadData(reader, size - 6);
                            break;
                    }
                }

                return this.Model;
            }
        }

        /// <summary>
        /// Read a chunk id.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The chunk ID.
        /// </returns>
        private ChunkID ReadChunkId(BinaryReader reader)
        {
            return (ChunkID)reader.ReadUInt16();
        }

        /// <summary>
        /// Read a chunk size.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The read chunk size.
        /// </returns>
        private int ReadChunkSize(BinaryReader reader)
        {
            return (int)reader.ReadUInt32();
        }

        /// <summary>
        /// Read a color.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A color.
        /// </returns>
        private Color ReadColor(BinaryReader reader)
        {
            var type = this.ReadChunkId(reader);
            int csize = this.ReadChunkSize(reader);
            switch (type)
            {
                case ChunkID.COL_RGB:
                    {
                        // this code has not been tested...
                        Debug.Assert(false);
                        float r = reader.ReadSingle();
                        float g = reader.ReadSingle();
                        float b = reader.ReadSingle();
                        return Color.FromScRgb(1, r, g, b);
                    }

                case ChunkID.COL_TRU:
                    {
                        byte r = reader.ReadByte();
                        byte g = reader.ReadByte();
                        byte b = reader.ReadByte();
                        return Color.FromArgb(0xFF, r, g, b);
                    }

                default:
                    this.ReadData(reader, csize);
                    break;
            }

            return Colors.White;
        }

        /* private string ReadString(int size)
         {
             var bytes = reader.ReadBytes(size);
             var enc = new ASCIIEncoding();
             var s = enc.GetString(bytes);
             return s.Trim('\0');
         }*/

        /// <summary>
        /// Read data.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="size">
        /// Excluding header size
        /// </param>
        /// <returns>
        /// The data.
        /// </returns>
        private byte[] ReadData(BinaryReader reader, int size)
        {
            return reader.ReadBytes(size);
        }

        /// <summary>
        /// Read a face list.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The face list.
        /// </returns>
        private Int32Collection ReadFaceList(BinaryReader reader)
        {
            int size = reader.ReadUInt16();
            var faces = new Int32Collection(size * 3);
            for (int i = 0; i < size; i++)
            {
                faces.Add(reader.ReadUInt16());
                faces.Add(reader.ReadUInt16());
                faces.Add(reader.ReadUInt16());
                reader.ReadUInt16();
            }

            return faces;
        }

        /// <summary>
        /// Read face materials.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="msize">
        /// The msize.
        /// </param>
        /// <returns>
        /// The materials.
        /// </returns>
        private List<FaceMaterial> ReadFaceMaterials(BinaryReader reader, int msize)
        {
            int total = 6;
            var list = new List<FaceMaterial>();
            while (total < msize)
            {
                var id = this.ReadChunkId(reader);
                int size = this.ReadChunkSize(reader);
                total += size;
                switch (id)
                {
                    case ChunkID.TRI_FACEMAT:
                        {
                            string name = this.ReadString(reader);
                            int n = reader.ReadUInt16();
                            var c = new Int32Collection();
                            for (int i = 0; i < n; i++)
                            {
                                c.Add(reader.ReadUInt16());
                            }

                            var fm = new FaceMaterial { Name = name, Faces = c };
                            list.Add(fm);
                            break;
                        }

                    case ChunkID.TRI_SMOOTH:
                        {
                            this.ReadData(reader, size - 6);
                            break;
                        }

                    default:
                        {
                            this.ReadData(reader, size - 6);
                            break;
                        }
                }
            }

            return list;
        }

        /// <summary>
        /// Read a mat map.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The mat map.
        /// </returns>
        private string ReadMatMap(BinaryReader reader, int size)
        {
            var id = this.ReadChunkId(reader);
            int siz = this.ReadChunkSize(reader);
            ushort f1 = reader.ReadUInt16();
            ushort f2 = reader.ReadUInt16();
            ushort f3 = reader.ReadUInt16();
            ushort f4 = reader.ReadUInt16();
            size -= 14;
            string cname = this.ReadString(reader);
            size -= cname.Length + 1;
            byte[] morebytes = this.ReadData(reader, size);
            return cname;
        }

        /// <summary>
        /// Read a material.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="msize">
        /// The size.
        /// </param>
        private void ReadMaterial(BinaryReader reader, int msize)
        {
            int total = 6;
            string name = null;

            var luminance = Colors.Transparent;
            var diffuse = Colors.Transparent;
            var specular = Colors.Transparent;
            var shininess = Colors.Transparent;
            string texture = null;

            while (total < msize)
            {
                ChunkID id = this.ReadChunkId(reader);
                int size = this.ReadChunkSize(reader);

                // Debug.WriteLine(id);
                total += size;

                switch (id)
                {
                    case ChunkID.MAT_NAME01:
                        name = this.ReadString(reader);

                        // name = ReadString(size - 6);
                        break;

                    case ChunkID.MAT_LUMINANCE:
                        luminance = this.ReadColor(reader);
                        break;

                    case ChunkID.MAT_DIFFUSE:
                        diffuse = this.ReadColor(reader);
                        break;

                    case ChunkID.MAT_SPECULAR:
                        specular = this.ReadColor(reader);
                        break;

                    case ChunkID.MAT_SHININESS:
                        byte[] bytes = this.ReadData(reader, size - 6);

                        // shininess = ReadColor(r, size);
                        break;

                    case ChunkID.MAT_MAP:
                        texture = this.ReadMatMap(reader, size - 6);
                        break;

                    case ChunkID.MAT_MAPFILE:
                        this.ReadData(reader, size - 6);
                        break;

                    default:
                        this.ReadData(reader, size - 6);
                        break;
                }
            }

            int specularPower = 100;
            var mg = new MaterialGroup();

            // mg.Children.Add(new DiffuseMaterial(new SolidColorBrush(luminance)));
            if (texture != null)
            {
                string ext = Path.GetExtension(texture);
                if (ext != null)
                {
                    ext = ext.ToLower();
                }

                // TGA not supported - convert textures to .png!
                if (ext == ".tga")
                {
                    texture = Path.ChangeExtension(texture, ".png");
                }

                var actualTexturePath = this.TexturePath ?? string.Empty;
                string path = Path.Combine(actualTexturePath, texture);
                if (File.Exists(path))
                {
                    var img = new BitmapImage(new Uri(path, UriKind.Relative));
                    var textureBrush = new ImageBrush(img) { ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.Tile };
                    mg.Children.Add(new DiffuseMaterial(textureBrush));
                }
                else
                {
                    Debug.WriteLine(string.Format("Texture not found: {0}", Path.GetFullPath(path)));
                    mg.Children.Add(new DiffuseMaterial(new SolidColorBrush(diffuse)));
                }
            }
            else
            {
                mg.Children.Add(new DiffuseMaterial(new SolidColorBrush(diffuse)));
            }

            mg.Children.Add(new SpecularMaterial(new SolidColorBrush(specular), specularPower));

            if (name != null)
            {
                this.materials[name] = mg;
            }
        }

        /// <summary>
        /// Read an object.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="msize">
        /// The size.
        /// </param>
        private void ReadObject(BinaryReader reader, int msize)
        {
            int total = 6;

            string objectName = this.ReadString(reader);
            total += objectName.Length + 1;

            while (total < msize)
            {
                var id = this.ReadChunkId(reader);
                int size = this.ReadChunkSize(reader);
                total += size;
                switch (id)
                {
                    case ChunkID.OBJ_TRIMESH:
                        this.ReadTriangularMesh(reader, size);
                        break;

                    // case ChunkID.OBJ_CAMERA:
                    default:
                        {
                            this.ReadData(reader, size - 6);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Read a string.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The string.
        /// </returns>
        private string ReadString(BinaryReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var ch = (char)reader.ReadByte();
                if (ch == 0)
                {
                    break;
                }

                sb.Append(ch);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Read tex coords.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The tex coords.
        /// </returns>
        private PointCollection ReadTexCoords(BinaryReader reader)
        {
            int size = reader.ReadUInt16();
            var pts = new PointCollection(size);
            for (int i = 0; i < size; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                pts.Add(new Point(x, 1 - y));
            }

            return pts;
        }

        /// <summary>
        /// Read a transformation.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A transformation.
        /// </returns>
        private Matrix3D ReadTransformation(BinaryReader reader)
        {
            Vector3D localx = this.ReadVector(reader);
            Vector3D localy = this.ReadVector(reader);
            Vector3D localz = this.ReadVector(reader);
            Vector3D origin = this.ReadVector(reader);

            var matrix = new Matrix3D();

            matrix.M11 = localx.X;
            matrix.M21 = localx.Y;
            matrix.M31 = localx.Z;

            matrix.M12 = localy.X;
            matrix.M22 = localy.Y;
            matrix.M32 = localy.Z;

            matrix.M13 = localz.X;
            matrix.M23 = localz.Y;
            matrix.M33 = localz.Z;

            matrix.OffsetX = origin.X;
            matrix.OffsetY = origin.Y;
            matrix.OffsetZ = origin.Z;

            matrix.M14 = 0;
            matrix.M24 = 0;
            matrix.M34 = 0;
            matrix.M44 = 1;

            return matrix;
        }

        /// <summary>
        /// The read triangular mesh.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="chunkSize">
        /// The chunk size.
        /// </param>
        private void ReadTriangularMesh(BinaryReader reader, int chunkSize)
        {
            int bytesRead = 6;
            Point3DCollection vertices = null;
            Int32Collection faces = null;
            PointCollection texcoords = null;
            List<FaceMaterial> facemat = null;

            while (bytesRead < chunkSize)
            {
                ChunkID id = this.ReadChunkId(reader);
                int size = this.ReadChunkSize(reader);
                bytesRead += size;
                switch (id)
                {
                    case ChunkID.TRI_VERTEXL:
                        vertices = this.ReadVertexList(reader);
                        break;
                    case ChunkID.TRI_FACEL1:
                        faces = this.ReadFaceList(reader);
                        size -= (faces.Count / 3 * 8) + 2;
                        facemat = this.ReadFaceMaterials(reader, size - 6);
                        break;
                    case ChunkID.TRI_TEXCOORD:
                        texcoords = this.ReadTexCoords(reader);
                        break;
                    case ChunkID.TRI_LOCAL:
                        this.ReadTransformation(reader);
                        break;

                    default:
                        this.ReadData(reader, size - 6);
                        break;
                }
            }

            if (facemat == null)
            {
                return;
            }

            // if (!matrix.IsIdentity)
            /*                for (int i = 0; i < vertices.Count; i++)
                            {
                                vertices[i] = DoTransform(matrix, vertices[i]);
                            }*/

            foreach (var fm in facemat)
            {
                var m = new MeshGeometry3D { Positions = vertices };
                var faces2 = new Int32Collection(fm.Faces.Count * 3);
                foreach (int f in fm.Faces)
                {
                    faces2.Add(faces[f * 3]);
                    faces2.Add(faces[(f * 3) + 1]);
                    faces2.Add(faces[(f * 3) + 2]);
                }

                m.TriangleIndices = faces2;
                m.TextureCoordinates = texcoords;
                var model = new GeometryModel3D { Geometry = m };
                if (this.materials.ContainsKey(fm.Name))
                {
                    model.Material = this.materials[fm.Name];
                    model.BackMaterial = model.Material;
                }

                this.Model.Children.Add(model);
            }
        }

        /// <summary>
        /// Read a vector.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A vector.
        /// </returns>
        private Vector3D ReadVector(BinaryReader reader)
        {
            return new Vector3D(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Read a vertex list.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A vertex list.
        /// </returns>
        private Point3DCollection ReadVertexList(BinaryReader reader)
        {
            int size = reader.ReadUInt16();
            var pts = new Point3DCollection(size);
            for (int i = 0; i < size; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                pts.Add(new Point3D(x, y, z));
            }

            return pts;
        }

        /// <summary>
        /// The face material.
        /// </summary>
        private class FaceMaterial
        {
            /// <summary>
            /// Gets or sets Faces.
            /// </summary>
            public Int32Collection Faces { get; set; }

            /// <summary>
            /// Gets or sets Name.
            /// </summary>
            public string Name { get; set; }

        }
    }
}