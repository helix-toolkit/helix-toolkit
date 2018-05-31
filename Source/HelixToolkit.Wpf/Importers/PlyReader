using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace HelixToolkit.Wpf
{
    /// <summary>
    /// Polygon File Format
    /// </summary>
    /// <remarks>
    /// This reader does not parse Normals.
    /// </remarks>
    public class PlyReader : ModelReader
    {
        public PlyReader(Dispatcher dispatcher = null) : base(dispatcher)
        {
            this.Vertices = new List<Point3D>();
            this.Faces = new List<int[]>();
        }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <returns>A <see cref="Model3DGroup" />.</returns>
        public override Model3DGroup Read(Stream s)
        {
            this.Load(s);
            return this.CreateModel3D();
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the model from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The model.</returns>
        public override Model3DGroup Read(string path)
        {
            //this.TexturePath = Path.GetDirectoryName(path);
            using (var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return this.Read(s);
            }
        }

        public IList<int[]> Faces {get;private set;}
        public IList<Point3D> Vertices { get; private set; }
        public IList<Vector3D> Normals { get; private set;}
        public int VerticesNumber { get; private set; }
        public int FacesNumber { get; private set; }

        /// <summary>
        /// Creates a mesh from the loaded file.
        /// </summary>
        /// <returns>
        /// A <see cref="Mesh3D" />.
        /// </returns>
        public Mesh3D CreateMesh()
        {
            var mesh = new Mesh3D();
            foreach (var vert in this.Vertices)
            {
                mesh.Vertices.Add(vert);
            }
            foreach (var face in this.Faces)
            {
                mesh.Faces.Add((int[])face.Clone());
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
            var mb = new MeshBuilder(false, false);
            foreach (var p in this.Vertices)
            {
                mb.Positions.Add(p);
            }

            foreach (var face in this.Faces)
            {
                mb.AddTriangleFan(face);
            }

            return mb.ToMesh();
        }

        /// <summary>
        /// Creates a <see cref="Model3DGroup" /> from the loaded file.
        /// </summary>
        /// <returns>A <see cref="Model3DGroup" />.</returns>
        public Model3DGroup CreateModel3D()
        {
            Model3DGroup modelGroup = null;
            this.Dispatch(() =>
            {
                modelGroup = new Model3DGroup();
                var g = this.CreateMeshGeometry3D();
                var gm = new GeometryModel3D { Geometry = g, Material = this.DefaultMaterial };
                gm.BackMaterial = gm.Material;
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
            return modelGroup;
        }

        public void Load(Stream s)
        {
            bool containsNormals = false;
            using (var reader=new StreamReader(s) )
            {
                while (!reader.EndOfStream)
                {
                    var curline = reader.ReadLine();
                    if (curline==null)
                    {
                        break;
                    }
                    string[] strarr = curline.Split(' ');
                    //element Line
                    if (strarr[0]=="element")
                    {
                        if (strarr[1]=="vertex")
                        {
                            VerticesNumber = int.Parse(strarr[2]);
                        }
                        else if (strarr[1] == "face")
                        {
                            FacesNumber = int.Parse(strarr[2]);
                        }
                    }

                    //property
                    if (strarr[0]=="property")
                    {
                        if (strarr[2]=="nx"|| strarr[2] == "ny"|| strarr[2] == "nz")
                        {
                            containsNormals = true;
                        }
                    }

                    if (strarr[0]=="end_header")
                    {
                        //end info, begin number collection.
                    }
                    
                    //reading the vertices
                    if (float.TryParse(strarr[0],out float value)==true&& int.TryParse(strarr[0], out int value1) == false )
                    {
                        Point3D pt = new Point3D()
                        {
                            X = double.Parse(strarr[0]),
                            Y=double.Parse(strarr[1]),
                            Z=double.Parse(strarr[2])
                        };
                        if (containsNormals)
                        {
                            Vector3D vect3 = new Vector3D
                            {
                                X = double.Parse(strarr[3]),
                                Y=double.Parse(strarr[4]),
                                Z=double.Parse(strarr[5])
                            };
                        }
                        //(strarr[0])
                        Vertices.Add(pt);
                    }
                    int intCount = 0;
                    foreach (var item in strarr)
                    {
                        if (int.TryParse(item, out int res)==true)
                        {
                            intCount += 1;
                        }
                        else { continue; }
                    }
                    //reading the faces
                    if (intCount==strarr.Length)
                    {
                        List<int> facepos = new List<int>();
                        for (int i = 1; i <= int.Parse(strarr[0]); i++)
                        {
                           facepos.Add(int.Parse(strarr[i]));
                        }
                        Faces.Add(facepos.ToArray());
                    }

                }
            }
        }


    }
}
