// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StudioReader.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A 3D Studio .3ds file reader.
    /// </summary>
    public class StudioReader : IModelReader
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local

        // ReSharper restore UnusedMember.Local
        // ReSharper restore InconsistentNaming

        // http://faydoc.tripod.com/formats/3ds.htm
        // http://www.gametutorials.com
        // http://code.google.com/p/lib3ds/
        // http://blogs.msdn.com/b/danlehen/archive/2005/10/09/478923.aspx
        #region Constants and Fields

        /// <summary>
        /// The materials.
        /// </summary>
        private readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        #endregion

        #region Enums

        /// <summary>
        /// The chunk id.
        /// </summary>
        private enum ChunkID
        {
            // Primary chunk

            /// <summary>
            /// The mai n 3 ds.
            /// </summary>
            MAIN3DS = 0x4D4D,

            // Main Chunks

            /// <summary>
            /// The edi t 3 ds.
            /// </summary>
            EDIT3DS = 0x3D3D, // this is the start of the editor config
            /// <summary>
            /// The key f 3 ds.
            /// </summary>
            KEYF3DS = 0xB000, // this is the start of the keyframer config
            /// <summary>
            /// The version.
            /// </summary>
            VERSION = 0x0002,

            /// <summary>
            /// The meshversion.
            /// </summary>
            MESHVERSION = 0x3D3E,

            // sub defines of EDIT3DS

            /// <summary>
            /// The edi t_ material.
            /// </summary>
            EDIT_MATERIAL = 0xAFFF,

            /// <summary>
            /// The edi t_ confi g 1.
            /// </summary>
            EDIT_CONFIG1 = 0x0100,

            /// <summary>
            /// The edi t_ confi g 2.
            /// </summary>
            EDIT_CONFIG2 = 0x3E3D,

            /// <summary>
            /// The edi t_ vie w_ p 1.
            /// </summary>
            EDIT_VIEW_P1 = 0x7012,

            /// <summary>
            /// The edi t_ vie w_ p 2.
            /// </summary>
            EDIT_VIEW_P2 = 0x7011,

            /// <summary>
            /// The edi t_ vie w_ p 3.
            /// </summary>
            EDIT_VIEW_P3 = 0x7020,

            /// <summary>
            /// The edi t_ vie w 1.
            /// </summary>
            EDIT_VIEW1 = 0x7001,

            /// <summary>
            /// The edi t_ backgr.
            /// </summary>
            EDIT_BACKGR = 0x1200,

            /// <summary>
            /// The edi t_ ambient.
            /// </summary>
            EDIT_AMBIENT = 0x2100,

            /// <summary>
            /// The edi t_ object.
            /// </summary>
            EDIT_OBJECT = 0x4000,

            /// <summary>
            /// The edi t_ unkn w 01.
            /// </summary>
            EDIT_UNKNW01 = 0x1100,

            /// <summary>
            /// The edi t_ unkn w 02.
            /// </summary>
            EDIT_UNKNW02 = 0x1201,

            /// <summary>
            /// The edi t_ unkn w 03.
            /// </summary>
            EDIT_UNKNW03 = 0x1300,

            /// <summary>
            /// The edi t_ unkn w 04.
            /// </summary>
            EDIT_UNKNW04 = 0x1400,

            /// <summary>
            /// The edi t_ unkn w 05.
            /// </summary>
            EDIT_UNKNW05 = 0x1420,

            /// <summary>
            /// The edi t_ unkn w 06.
            /// </summary>
            EDIT_UNKNW06 = 0x1450,

            /// <summary>
            /// The edi t_ unkn w 07.
            /// </summary>
            EDIT_UNKNW07 = 0x1500,

            /// <summary>
            /// The edi t_ unkn w 08.
            /// </summary>
            EDIT_UNKNW08 = 0x2200,

            /// <summary>
            /// The edi t_ unkn w 09.
            /// </summary>
            EDIT_UNKNW09 = 0x2201,

            /// <summary>
            /// The edi t_ unkn w 10.
            /// </summary>
            EDIT_UNKNW10 = 0x2210,

            /// <summary>
            /// The edi t_ unkn w 11.
            /// </summary>
            EDIT_UNKNW11 = 0x2300,

            /// <summary>
            /// The edi t_ unkn w 12.
            /// </summary>
            EDIT_UNKNW12 = 0x2302,

            /// <summary>
            /// The edi t_ unkn w 13.
            /// </summary>
            EDIT_UNKNW13 = 0x3000,

            /// <summary>
            /// The edi t_ unkn w 14.
            /// </summary>
            EDIT_UNKNW14 = 0xAFFF,

            // sub defines of EDIT_MATERIAL
            /// <summary>
            /// The ma t_ nam e 01.
            /// </summary>
            MAT_NAME01 = 0xA000,

            /// <summary>
            /// The ma t_ luminance.
            /// </summary>
            MAT_LUMINANCE = 0xA010,

            /// <summary>
            /// The ma t_ diffuse.
            /// </summary>
            MAT_DIFFUSE = 0xA020,

            /// <summary>
            /// The ma t_ specular.
            /// </summary>
            MAT_SPECULAR = 0xA030,

            /// <summary>
            /// The ma t_ shininess.
            /// </summary>
            MAT_SHININESS = 0xA040,

            /// <summary>
            /// The ma t_ map.
            /// </summary>
            MAT_MAP = 0xA200,

            /// <summary>
            /// The ma t_ mapfile.
            /// </summary>
            MAT_MAPFILE = 0xA300,

            // sub defines of EDIT_OBJECT
            /// <summary>
            /// The ob j_ trimesh.
            /// </summary>
            OBJ_TRIMESH = 0x4100,

            /// <summary>
            /// The ob j_ light.
            /// </summary>
            OBJ_LIGHT = 0x4600,

            /// <summary>
            /// The ob j_ camera.
            /// </summary>
            OBJ_CAMERA = 0x4700,

            /// <summary>
            /// The ob j_ unknw n 01.
            /// </summary>
            OBJ_UNKNWN01 = 0x4010,

            /// <summary>
            /// The ob j_ unknw n 02.
            /// </summary>
            OBJ_UNKNWN02 = 0x4012, // Could be shadow

            // sub defines of OBJ_CAMERA
            /// <summary>
            /// The ca m_ unknw n 01.
            /// </summary>
            CAM_UNKNWN01 = 0x4710,

            /// <summary>
            /// The ca m_ unknw n 02.
            /// </summary>
            CAM_UNKNWN02 = 0x4720,

            // sub defines of OBJ_LIGHT
            /// <summary>
            /// The li t_ off.
            /// </summary>
            LIT_OFF = 0x4620,

            /// <summary>
            /// The li t_ spot.
            /// </summary>
            LIT_SPOT = 0x4610,

            /// <summary>
            /// The li t_ unknw n 01.
            /// </summary>
            LIT_UNKNWN01 = 0x465A,

            // sub defines of OBJ_TRIMESH
            /// <summary>
            /// The tr i_ vertexl.
            /// </summary>
            TRI_VERTEXL = 0x4110,

            /// <summary>
            /// The tr i_ face l 2.
            /// </summary>
            TRI_FACEL2 = 0x4111,

            /// <summary>
            /// The tr i_ face l 1.
            /// </summary>
            TRI_FACEL1 = 0x4120,

            /// <summary>
            /// The tr i_ facemat.
            /// </summary>
            TRI_FACEMAT = 0x4130,

            /// <summary>
            /// The tr i_ texcoord.
            /// </summary>
            TRI_TEXCOORD = 0x4140,

            /// <summary>
            /// The tr i_ smooth.
            /// </summary>
            TRI_SMOOTH = 0x4150,

            /// <summary>
            /// The tr i_ local.
            /// </summary>
            TRI_LOCAL = 0x4160,

            /// <summary>
            /// The tr i_ visible.
            /// </summary>
            TRI_VISIBLE = 0x4165,

            // sub defs of KEYF3DS

            /// <summary>
            /// The key f_ unknw n 01.
            /// </summary>
            KEYF_UNKNWN01 = 0xB009,

            /// <summary>
            /// The key f_ unknw n 02.
            /// </summary>
            KEYF_UNKNWN02 = 0xB00A,

            /// <summary>
            /// The key f_ frames.
            /// </summary>
            KEYF_FRAMES = 0xB008,

            /// <summary>
            /// The key f_ objdes.
            /// </summary>
            KEYF_OBJDES = 0xB002,

            /// <summary>
            /// The key f_ hierarchy.
            /// </summary>
            KEYF_HIERARCHY = 0xB030,

            /// <summary>
            /// The kfname.
            /// </summary>
            KFNAME = 0xB010,

            // these define the different color chunk types
            /// <summary>
            /// The co l_ rgb.
            /// </summary>
            COL_RGB = 0x0010,

            /// <summary>
            /// The co l_ tru.
            /// </summary>
            COL_TRU = 0x0011, // RGB24
            /// <summary>
            /// The co l_ unk.
            /// </summary>
            COL_UNK = 0x0013,

            // defines for viewport chunks

            /// <summary>
            /// The top.
            /// </summary>
            TOP = 0x0001,

            /// <summary>
            /// The bottom.
            /// </summary>
            BOTTOM = 0x0002,

            /// <summary>
            /// The left.
            /// </summary>
            LEFT = 0x0003,

            /// <summary>
            /// The right.
            /// </summary>
            RIGHT = 0x0004,

            /// <summary>
            /// The front.
            /// </summary>
            FRONT = 0x0005,

            /// <summary>
            /// The back.
            /// </summary>
            BACK = 0x0006,

            /// <summary>
            /// The user.
            /// </summary>
            USER = 0x0007,

            /// <summary>
            /// The camera.
            /// </summary>
            CAMERA = 0x0008, // = 0xFFFF is the actual code read from file
            /// <summary>
            /// The light.
            /// </summary>
            LIGHT = 0x0009,

            /// <summary>
            /// The disabled.
            /// </summary>
            DISABLED = 0x0010,

            /// <summary>
            /// The bogus.
            /// </summary>
            BOGUS = 0x0011,
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the texture path.
        /// </summary>
        /// <value>The texture path.</value>
        public string TexturePath { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Model.
        /// </summary>
        private Model3DGroup Model { get; set; }

        /// <summary>
        /// Gets or sets reader.
        /// </summary>
        private BinaryReader reader { get; set; }

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
            Model3DGroup model = Read(s);
            s.Close();
            return model;
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
            this.reader = new BinaryReader(s);

            long length = this.reader.BaseStream.Length;

            // http://gpwiki.org/index.php/Loading_3ds_files
            // http://www.flipcode.com/archives/3DS_File_Loader.shtml
            // http://sandy.googlecode.com/svn/trunk/sandy/as3/branches/3.0.2/src/sandy/parser/Parser3DS.as
            ChunkID headerID = this.ReadChunkId();
            if (headerID != ChunkID.MAIN3DS)
            {
                throw new FileFormatException("Unknown file");
            }

            int headerSize = this.ReadChunkSize();
            if (headerSize != length)
            {
                throw new FileFormatException("Incomplete file (file length does not match header)");
            }

            this.Model = new Model3DGroup();

            while (this.reader.BaseStream.Position < this.reader.BaseStream.Length)
            {
                ChunkID id = this.ReadChunkId();
                int size = this.ReadChunkSize();
                // Debug.WriteLine(id);

                switch (id)
                {
                    case ChunkID.EDIT_MATERIAL:
                        this.ReadMaterial(size);
                        break;
                    case ChunkID.EDIT_OBJECT:
                        this.ReadObject(size);
                        break;
                    case ChunkID.EDIT3DS:
                    case ChunkID.OBJ_CAMERA:
                    case ChunkID.OBJ_LIGHT:
                    case ChunkID.OBJ_TRIMESH:

                        // don't read the whole chunk, read the sub-defines...
                        break;

                    default:

                        // download the whole chunk
                        byte[] bytes = this.ReadData(size - 6);
                        break;
                }
            }

            this.reader.Close();
            return this.Model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The read chunk id.
        /// </summary>
        /// <returns>
        /// </returns>
        private ChunkID ReadChunkId()
        {
            return (ChunkID)this.reader.ReadUInt16();
        }

        /// <summary>
        /// The read chunk size.
        /// </summary>
        /// <returns>
        /// The read chunk size.
        /// </returns>
        private int ReadChunkSize()
        {
            return (int)this.reader.ReadUInt32();
        }

        /// <summary>
        /// The read color.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// </returns>
        private Color ReadColor(int size)
        {
            // var bb = ReadData(reader, size - 6);
            // return Colors.White;
            ChunkID type = this.ReadChunkId();
            int csize = this.ReadChunkSize();
            size -= 6;
            switch (type)
            {
                case ChunkID.COL_RGB:
                    {
                        // not checked...
                        Debug.Assert(false);
                        float r = this.reader.ReadSingle();
                        float g = this.reader.ReadSingle();
                        float b = this.reader.ReadSingle();
                        return Color.FromScRgb(1, r, g, b);
                    }

                case ChunkID.COL_TRU:
                    {
                        byte r = this.reader.ReadByte();
                        byte g = this.reader.ReadByte();
                        byte b = this.reader.ReadByte();
                        return Color.FromArgb(0xFF, r, g, b);
                    }

                default:
                    this.ReadData(csize);
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
        /// The read data.
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
        /// The read face list.
        /// </summary>
        /// <returns>
        /// </returns>
        private Int32Collection ReadFaceList()
        {
            int size = this.reader.ReadUInt16();
            var faces = new Int32Collection(size * 3);
            for (int i = 0; i < size; i++)
            {
                faces.Add(this.reader.ReadUInt16());
                faces.Add(this.reader.ReadUInt16());
                faces.Add(this.reader.ReadUInt16());
                float flags = this.reader.ReadUInt16();
            }

            return faces;
        }

        /// <summary>
        /// The read face materials.
        /// </summary>
        /// <param name="msize">
        /// The msize.
        /// </param>
        /// <returns>
        /// </returns>
        private List<FaceMaterial> ReadFaceMaterials(int msize)
        {
            int total = 6;
            var list = new List<FaceMaterial>();
            while (total < msize)
            {
                ChunkID id = this.ReadChunkId();
                int size = this.ReadChunkSize();
                // Debug.WriteLine(id);
                total += size;
                switch (id)
                {
                    case ChunkID.TRI_FACEMAT:
                        {
                            string name = this.ReadString();
                            int n = this.reader.ReadUInt16();
                            var c = new Int32Collection();
                            for (int i = 0; i < n; i++)
                            {
                                c.Add(this.reader.ReadUInt16());
                            }

                            var fm = new FaceMaterial { Name = name, Faces = c };
                            list.Add(fm);
                            break;
                        }

                    case ChunkID.TRI_SMOOTH:
                        {
                            byte[] bytes = this.ReadData(size - 6);
                            break;
                        }

                    default:
                        {
                            byte[] bytes = this.ReadData(size - 6);
                            break;
                        }
                }
            }

            return list;
        }

        /// <summary>
        /// The read mat map.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The read mat map.
        /// </returns>
        private string ReadMatMap(int size)
        {
            ChunkID id = this.ReadChunkId();
            int siz = this.ReadChunkSize();
            ushort f1 = this.reader.ReadUInt16();
            ushort f2 = this.reader.ReadUInt16();
            ushort f3 = this.reader.ReadUInt16();
            ushort f4 = this.reader.ReadUInt16();
            size -= 14;
            string cname = this.ReadString();
            size -= cname.Length + 1;
            byte[] morebytes = this.ReadData(size);
            return cname;
        }

        /// <summary>
        /// The read material.
        /// </summary>
        /// <param name="msize">
        /// The msize.
        /// </param>
        private void ReadMaterial(int msize)
        {
            int total = 6;
            string name = null;

            Color luminance = Colors.Transparent;
            Color diffuse = Colors.Transparent;
            Color specular = Colors.Transparent;
            Color shininess = Colors.Transparent;
            string texture = null;

            while (total < msize)
            {
                ChunkID id = this.ReadChunkId();
                int size = this.ReadChunkSize();
                // Debug.WriteLine(id);
                total += size;

                switch (id)
                {
                    case ChunkID.MAT_NAME01:
                        name = this.ReadString();

                        // name = ReadString(size - 6);
                        break;

                    case ChunkID.MAT_LUMINANCE:
                        luminance = this.ReadColor(size);
                        break;

                    case ChunkID.MAT_DIFFUSE:
                        diffuse = this.ReadColor(size);
                        break;

                    case ChunkID.MAT_SPECULAR:
                        specular = this.ReadColor(size);
                        break;

                    case ChunkID.MAT_SHININESS:
                        byte[] bytes = this.ReadData(size - 6);

                        // shininess = ReadColor(r, size);
                        break;

                    case ChunkID.MAT_MAP:
                        texture = this.ReadMatMap(size - 6);
                        break;

                    case ChunkID.MAT_MAPFILE:
                        this.ReadData(size - 6);
                        break;

                    default:
                        this.ReadData(size - 6);
                        break;
                }
            }

            int specularPower = 100;
            var mg = new MaterialGroup();

            // mg.Children.Add(new DiffuseMaterial(new SolidColorBrush(luminance)));
            if (texture != null)
            {
                string ext = Path.GetExtension(texture).ToLower();

                // TGA not supported - convert textures to .png!
                if (ext == ".tga")
                {
                    texture = Path.ChangeExtension(texture, ".png");
                }

                if (ext == ".bmp")
                {
                    texture = Path.ChangeExtension(texture, ".jpg");
                }

                string path = Path.Combine(this.TexturePath, texture);
                if (File.Exists(path))
                {
                    var img = new BitmapImage(new Uri(path, UriKind.Relative));
                    var textureBrush = new ImageBrush(img) { ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.Tile };
                    mg.Children.Add(new DiffuseMaterial(textureBrush));
                }
                else
                {
                    Debug.WriteLine(string.Format("Texture {0} not found in {1}", texture, this.TexturePath));
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
                this.Materials.Add(name, mg);
            }
        }

        /// <summary>
        /// The read object.
        /// </summary>
        /// <param name="msize">
        /// The msize.
        /// </param>
        private void ReadObject(int msize)
        {
            int total = 6;

            string objectName = this.ReadString();
            total += objectName.Length + 1;

            while (total < msize)
            {
                ChunkID id = this.ReadChunkId();
                int size = this.ReadChunkSize();
                // Debug.WriteLine(id);
                total += size;
                switch (id)
                {
                    case ChunkID.OBJ_TRIMESH:
                        this.ReadTriangularMesh(size);
                        break;

                    // case ChunkID.OBJ_CAMERA:
                    default:
                        {
                            byte[] bytes = this.ReadData(size - 6);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// The read string.
        /// </summary>
        /// <returns>
        /// The read string.
        /// </returns>
        private string ReadString()
        {
            var sb = new StringBuilder();
            while (true)
            {
                var ch = (char)this.reader.ReadByte();
                if (ch == 0)
                {
                    break;
                }

                sb.Append(ch);
            }

            return sb.ToString();
        }

        /// <summary>
        /// The read tex coords.
        /// </summary>
        /// <returns>
        /// </returns>
        private PointCollection ReadTexCoords()
        {
            int size = this.reader.ReadUInt16();
            var pts = new PointCollection(size);
            for (int i = 0; i < size; i++)
            {
                float x = this.reader.ReadSingle();
                float y = this.reader.ReadSingle();
                pts.Add(new Point(x, 1 - y));
            }

            return pts;
        }

        /// <summary>
        /// The read transformation.
        /// </summary>
        /// <returns>
        /// </returns>
        private Matrix3D ReadTransformation()
        {
            Vector3D localx = this.ReadVector();
            Vector3D localy = this.ReadVector();
            Vector3D localz = this.ReadVector();
            Vector3D origin = this.ReadVector();

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
        /// <param name="chunkSize">
        /// The chunk size.
        /// </param>
        private void ReadTriangularMesh(int chunkSize)
        {
            int bytesRead = 6;
            Point3DCollection vertices = null;
            Int32Collection faces = null;
            PointCollection texcoords = null;
            Matrix3D matrix = Matrix3D.Identity;
            List<FaceMaterial> facemat = null;

            while (bytesRead < chunkSize)
            {
                ChunkID id = this.ReadChunkId();
                int size = this.ReadChunkSize();
                // Debug.WriteLine(" " + id);
                bytesRead += size;
                switch (id)
                {
                    case ChunkID.TRI_VERTEXL:
                        vertices = this.ReadVertexList();
                        break;
                    case ChunkID.TRI_FACEL1:
                        faces = this.ReadFaceList();
                        size -= faces.Count / 3 * 8 + 2;
                        facemat = this.ReadFaceMaterials(size - 6);
                        break;
                    case ChunkID.TRI_TEXCOORD:
                        texcoords = this.ReadTexCoords();
                        break;
                    case ChunkID.TRI_LOCAL:
                        matrix = this.ReadTransformation();
                        break;

                    default:
                        this.ReadData(size - 6);
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
                    faces2.Add(faces[f * 3 + 1]);
                    faces2.Add(faces[f * 3 + 2]);
                }

                m.TriangleIndices = faces2;
                m.TextureCoordinates = texcoords;
                var model = new GeometryModel3D { Geometry = m };
                if (this.Materials.ContainsKey(fm.Name))
                {
                    model.Material = this.Materials[fm.Name];
                    model.BackMaterial = model.Material;
                }
                else
                {
                    // use default material
                    // MaterialHelper.CreateMaterial(Brushes.Brown);
                }

                this.Model.Children.Add(model);
            }
        }

        /// <summary>
        /// The read vector.
        /// </summary>
        /// <returns>
        /// </returns>
        private Vector3D ReadVector()
        {
            return new Vector3D(this.reader.ReadSingle(), this.reader.ReadSingle(), this.reader.ReadSingle());
        }

        /// <summary>
        /// The read vertex list.
        /// </summary>
        /// <returns>
        /// </returns>
        private Point3DCollection ReadVertexList()
        {
            int size = this.reader.ReadUInt16();
            var pts = new Point3DCollection(size);
            for (int i = 0; i < size; i++)
            {
                float x = this.reader.ReadSingle();
                float y = this.reader.ReadSingle();
                float z = this.reader.ReadSingle();
                pts.Add(new Point3D(x, y, z));
            }

            return pts;
        }

        #endregion

        /// <summary>
        /// The face material.
        /// </summary>
        private class FaceMaterial
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets Faces.
            /// </summary>
            public Int32Collection Faces { get; set; }

            /// <summary>
            /// Gets or sets Name.
            /// </summary>
            public string Name { get; set; }

            #endregion
        }
    }
}