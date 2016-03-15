using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object3DGroup = System.Collections.Generic.List<HelixToolkit.Wpf.SharpDX.Object3D>;
using SharpDX;
using System.Diagnostics;
using MediaColor = System.Windows.Media.Color;
using MediaMatrix3D = System.Windows.Media.Media3D.Matrix3D;
using MediaMatrixTransform = System.Windows.Media.Media3D.MatrixTransform3D;
using MediaPoint3D = System.Windows.Media.Media3D.Point3D;
using MediaVector3D = System.Windows.Media.Media3D.Vector3D;
using System.Windows.Media.Imaging;
using HelixToolkit.Wpf.SharpDX.Core;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    ///Ported from HelixToolkit.Wpf
    /// </summary>
  public class StudioReader : IModelReader
  {
    private readonly Dictionary<string,Material>materials=new Dictionary<string,Material>();
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
      //  MAT_AMBIENT=

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
    /// Helper class to create objects
    /// </summary>
    public Object3DGroup obGroup = new Object3DGroup();
    /// <summary>
    /// Gets or sets the directory
    /// </summary>
    public string Directory { get; set; }
    /// <summary>
    /// Gets or sets the texture path.
    /// </summary>
    /// <value>The texture path.</value>
    public string TexturePath
    {
      get
      {
        return this.Directory;
      }

      set
      {
        this.Directory = value;
      }
    }
    public Object3DGroup Read(string path, ModelInfo info = default(ModelInfo))
    {
      this.Directory = Path.GetDirectoryName(path);
      using (var s = File.OpenRead(path))
      {
        return this.Read(s);
      };
    }
    public Object3DGroup Read(Stream s,ModelInfo info=default(ModelInfo))
    {
      using (var reader = new BinaryReader(s))
      {
        long length = reader.BaseStream.Length;
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

      }
      return obGroup;
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
    /// reads the Material of a chunck
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="chunkSize"></param>
    private void ReadMaterial(BinaryReader reader,int chunkSize)
    {
      int total = 6;
      string name = null;
      var luminance = Color.Transparent; //SharpDX.Color not System.Windows.Media.Color
      var diffuse = Color.Transparent;
      var specular = Color.Transparent;
      var shininess = Color.Transparent;
      string texture = null;
      while (total < chunkSize)
      {
        ChunkID id = this.ReadChunkId(reader);
        int size = this.ReadChunkSize(reader);
        total += size;
        switch (id)
        {
          case ChunkID.MAT_NAME01:
            name = this.ReadString(reader);
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
      int specularPower = 100;//check if we can find this somewhere instead of just setting it to 100 
      BitmapSource image = ReadBitmapSoure(texture, diffuse);


      var material = new PhongMaterial()
      {
        DiffuseColor = diffuse,
        AmbientColor = luminance, //not really sure about this, lib3ds uses 0xA010 as AmbientColor
        SpecularColor = specular,
        SpecularShininess = specularPower,
        
        
        
        
      };
      if(image!= null)
      {
        material.NormalMap = image;
      }
      if (name != null)
      {
        materials[name] = material;
      }

    }

    /// <summary>
    /// Reads an object
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="chunkSize"></param>
    private void ReadObject(BinaryReader reader,int chunkSize)
    {
      int total = 6;
      string objectName = this.ReadString(reader);
      total += objectName.Length + 1;
      while (total < chunkSize)
      {
        var id = this.ReadChunkId(reader);
        int size = this.ReadChunkSize(reader);
        total += size;
        switch (id)
        {
          case ChunkID.OBJ_TRIMESH:
            this.ReadTriangularMesh(reader, size);
            break;
          default:
            {
              this.ReadData(reader, size - 6);
              break;
            }
        }
      }
    }
    /// <summary>
    /// Reads a triangular mesh.
    /// </summary>
    /// <param name="reader">
    /// The reader.
    /// </param>
    /// <param name="chunkSize">
    /// The chunk size.
    /// </param>
    private void ReadTriangularMesh(BinaryReader reader, int chunkSize)
    {
      MeshBuilder builder = new MeshBuilder();
      int bytesRead = 6;
      Vector3Collection positions = null;
      IntCollection faces = null;
      Vector2Collection textureCoordinates = null;
      List<FaceSet>facesets = null;
      IntCollection triangleIndices = null;
      Vector3Collection normals = null;
      MediaMatrix3D matrix = MediaMatrix3D.Identity;
      Vector3Collection tangents = null;
      Vector3Collection bitangents = null;
      while (bytesRead < chunkSize)
      {
        ChunkID id = this.ReadChunkId(reader);
        int size = this.ReadChunkSize(reader);
        bytesRead += size;
        switch (id)
        {
          case ChunkID.TRI_VERTEXL:
            positions = this.ReadVertexList(reader);
          break;
          case ChunkID.TRI_FACEL1:
            faces=ReadFaceList(reader);
            size -= (faces.Count / 3 * 8) + 2;
            facesets = this.ReadFaceSets(reader, size - 6);
            break;
          case ChunkID.TRI_TEXCOORD:
            textureCoordinates=ReadTexCoords(reader);
            break;
          case ChunkID.TRI_LOCAL:
            matrix=this.ReadTransformation(reader);
            break;
          default:
            this.ReadData(reader, size - 6);
            break;

        }
      }
      if (!matrix.IsIdentity)
      {
        for(int i = 0; i < positions.Count; i++)
        {
          positions[i] = Transform(matrix, positions[i]);
        }
      }
      if (faces == null)
      {
        //no faces defined?? return...
        return;
      }
      
      if(facesets==null||facesets.Count==0)
      {
        triangleIndices = ConvertFaceIndices(faces, faces);
        CreateMesh(positions, textureCoordinates, triangleIndices, out normals, out tangents, out bitangents,PhongMaterials.Gray);
        //Add default get and setter
      }
      else
      {
         foreach(var fm in facesets)
        {
          triangleIndices = ConvertFaceIndices(fm.Faces, faces);
          Material mat = null;
          if (this.materials.ContainsKey(fm.Name))
          {
            mat = this.materials[fm.Name];
          }
          CreateMesh(positions, textureCoordinates, triangleIndices, out normals, out tangents, out bitangents,mat);

        }
      }
    }


    /// <summary>
    /// Create a Mesh, with found props
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="textureCoordinates"></param>
    /// <param name="triangleIndices"></param>
    /// <param name="normals"></param>
    /// <param name="tangents"></param>
    /// <param name="bitangents"></param>
    /// <param name="material"></param>
    private void CreateMesh(Vector3Collection positions, Vector2Collection textureCoordinates, IntCollection triangleIndices, out Vector3Collection normals, out Vector3Collection tangents, out Vector3Collection bitangents,Material material)
    {
      ComputeNormals(positions, triangleIndices, out normals);
      if (textureCoordinates == null)
      {
        textureCoordinates = new Vector2Collection();
        foreach(var pos in positions)
        {
          textureCoordinates.Add(Vector2.One);
        }
      } 
      MeshBuilder.ComputeTangents(positions, normals, textureCoordinates, triangleIndices, out tangents, out bitangents);
      MeshGeometry3D mesh = new MeshGeometry3D()
      {
        Positions = positions,
        Normals = normals,
        TextureCoordinates = textureCoordinates,
        Indices = triangleIndices,
        Tangents = tangents,
        BiTangents = bitangents

      };
      Object3D ob3d = new Object3D();
      ob3d.Geometry = mesh;
      ob3d.Material = material;
      ob3d.Transform = Matrix.Identity;
      ob3d.Name = "Default";
      this.obGroup.Add(ob3d);
    }

    /// <summary>
    /// Stolen from MeshBuilder class, maybe make this static method there public...
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="triangleIndices"></param>
    /// <param name="normals"></param>
    private static void ComputeNormals(Vector3Collection positions, IntCollection triangleIndices, out Vector3Collection normals)
    {
      normals = new Vector3Collection(positions.Count);
      normals.AddRange(Enumerable.Repeat(Vector3.Zero, positions.Count));

      for (int t = 0; t < triangleIndices.Count; t += 3)
      {
        var i1 = triangleIndices[t];
        var i2 = triangleIndices[t + 1];
        var i3 = triangleIndices[t + 2];

        var v1 = positions[i1];
        var v2 = positions[i2];
        var v3 = positions[i3];

        var p1 = v2 - v1;
        var p2 = v3 - v1;
        var n = Vector3.Cross(p1, p2);
        // angle
        p1.Normalize();
        p2.Normalize();
        var a = (float)Math.Acos(Vector3.Dot(p1, p2));
        n.Normalize();
        normals[i1] += (a * n);
        normals[i2] += (a * n);
        normals[i3] += (a * n);
      }

      for (int i = 0; i < normals.Count; i++)
      {
        normals[i].Normalize();
      }
    }

    private static IntCollection ConvertFaceIndices(List<int> subFaces, List<int> faces)
    {
      var triangleIndices = new IntCollection(subFaces.Count * 3);// new List<int>(subFaces.Count * 3);
      foreach (int f in subFaces)
      {
        triangleIndices.Add(faces[f * 3]);
        triangleIndices.Add(faces[(f * 3) + 1]);
        triangleIndices.Add(faces[(f * 3) + 2]);
      }

      return triangleIndices;
    }

    private  Vector2Collection ReadTexCoords(BinaryReader reader)
    {
      int size = reader.ReadUInt16();
      var pts = new Vector2Collection();
      for(int i=0;i< size; i++)
      {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        pts.Add(new Vector2(x, y));
      }
      return pts;
    }

    /// <summary>
    /// Reads face sets.
    /// </summary>
    /// <param name="reader">
    /// The reader.
    /// </param>
    /// <param name="chunkSize">
    /// The chunk size.
    /// </param>
    /// <returns>
    /// A list of face sets.
    /// </returns>
    private List<FaceSet> ReadFaceSets(BinaryReader reader, int chunkSize)
    {
      int total = 6;
      var list = new List<FaceSet>();
      while (total < chunkSize)
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
              var c = new List<int>();
              for (int i = 0; i < n; i++)
              {
                c.Add(reader.ReadUInt16());
              }

              var fm = new FaceSet { Name = name, Faces = c };
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

    private IntCollection ReadFaceList(BinaryReader reader)
    {
      int size = reader.ReadUInt16();
      var faces = new IntCollection();
      for(int i=0;i< size; i++)
      {
        faces.Add(reader.ReadUInt16());
        faces.Add(reader.ReadUInt16());
        faces.Add(reader.ReadUInt16());
        reader.ReadUInt16();
      }
      return faces;
    }


    /// <summary>
    /// Reads a vector.
    /// </summary>
    /// <param name="reader">
    /// The reader.
    /// </param>
    /// <returns>
    /// A vector.
    /// </returns>
    private MediaVector3D ReadVector(BinaryReader reader)
    {
      return new MediaVector3D(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    /// <summary>
    /// Reads a transformation.
    /// </summary>
    /// <param name="reader">
    /// The reader.
    /// </param>
    /// <returns>
    /// A transformation.
    /// </returns>
    private MediaMatrix3D ReadTransformation(BinaryReader reader)
    {

      var localx = this.ReadVector(reader);
      var localy = this.ReadVector(reader);
      var localz = this.ReadVector(reader);
      var origin = this.ReadVector(reader);

      var matrix = new MediaMatrix3D
      {
        M11 = localx.X,
        M21 = localx.Y,
        M31 = localx.Z,
        M12 = localy.X,
        M22 = localy.Y,
        M32 = localy.Z,
        M13 = localz.X,
        M23 = localz.Y,
        M33 = localz.Z,
        OffsetX = origin.X,
        OffsetY = origin.Y,
        OffsetZ = origin.Z,
        M14 = 0,
        M24 = 0,
        M34 = 0,
        M44 = 1
      };

      return matrix;
    }

    /// <summary>
    /// Apply transform, doesn't work yet because datatypes(Matrix,Vector etc from .Media are use propably some float vs double offsets
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector3 Transform(MediaMatrix3D matrix,Vector3 pos)
    {
      MediaMatrixTransform transformer = new MediaMatrixTransform(matrix);
      MediaPoint3D Point = new MediaPoint3D(pos.X, pos.Y, pos.Z);
      MediaPoint3D tPoint= transformer.Transform(Point);
      //return new Vector3() { X = (float)tPoint.X, Y = (float)tPoint.Y, Z = (float)tPoint.Z };
      return pos;
    }
    private Vector3Collection ReadVertexList(BinaryReader reader)
    {
      int size = reader.ReadUInt16();
      var pts = new Vector3Collection();
      for(int i=0;i< size; i++)
      {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        pts.Add(new Vector3(x, y, z));
      }
      return pts;
    }
    /// <summary>
    /// A bit hacky we use the give texture as normalMap, if not existant we create a BitMapSource in the fallbackColor
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="fallBackColor"></param>
    /// <returns></returns>
    /// 
    private BitmapSource ReadBitmapSoure(string texture,Color fallBackColor)
    {
      if (texture == null)
      {
        return null;
      }
      try
      {
        string ext = Path.GetExtension(texture);
        if (ext != null)
        {
          ext.ToLower();
        }
        // TGA not supported - convert textures to .png
        if (ext == ".tga")
        {
          texture = Path.ChangeExtension(texture, ".png");
        }
        var actualTexturePath = this.TexturePath ?? string.Empty;
        string path = Path.Combine(actualTexturePath, texture);
        if (File.Exists(path))
        {
          return new BitmapImage(new Uri(path, UriKind.Relative));
        }
        else
        {
         // return null;
          return BitMapSoureFromFallBack(fallBackColor);
        }
      }
      catch(Exception ex) //Not really nice
      {
        return BitMapSoureFromFallBack(fallBackColor);
        //return new BitmapSource( );
      }


    }


    /// <summary>
    /// Creates FallBack Bitmapsource http://stackoverflow.com/questions/10637064/create-bitmapimage-and-apply-to-it-a-specific-color
    /// </summary>
    /// <param name="fallBackColor"></param>
    /// <returns></returns>
    private static BitmapSource BitMapSoureFromFallBack(Color fallBackColor)
    {
      //List<MediaColor> colors = new List<System.Windows.Media.Color>() { MediaColor.FromArgb(fallBackColor.A, fallBackColor.R, fallBackColor.G, fallBackColor.B) };
      MediaColor color = MediaColor.FromArgb(fallBackColor.A, fallBackColor.R, fallBackColor.G, fallBackColor.G);
      List<MediaColor> colors = new List<System.Windows.Media.Color>();
      colors.Add(color);
      BitmapPalette palette = new BitmapPalette(colors);
      int width = 128;
      int height = 128;
      int stride = width / 8;
      byte[] pixels = new byte[height*stride];
      BitmapSource bitmap = BitmapSource.Create(10, 10, 96, 96, System.Windows.Media.PixelFormats.Indexed1, palette, pixels, stride);
      return bitmap;
    }
   
    /// <summary>
    /// Reads a material map.
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

            return new Color(r, g, b,1);  // .FromScRgb(1, r, g, b);
          }

        case ChunkID.COL_TRU:
          {
            byte r = reader.ReadByte();
            byte g = reader.ReadByte();
            byte b = reader.ReadByte();
            return new Color(r, g, b); 
          }

        default:
          this.ReadData(reader, csize);
          break;
      }

      return Color.White;
    }
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
    /// Reads a string.
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
    /// Represents a set of faces that belongs to the same material.
    /// </summary>
    private class FaceSet
    {
      /// <summary>
      /// Gets or sets Faces.
      /// </summary>
      public List<int> Faces { get; set; }

      /// <summary>
      /// Gets or sets the name of the material.
      /// </summary>
      public string Name { get; set; }
    }

  }
}
