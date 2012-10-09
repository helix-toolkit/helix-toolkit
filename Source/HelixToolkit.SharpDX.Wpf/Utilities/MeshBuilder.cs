namespace HelixToolkit.SharpDX.Wpf
{
    using System;
    using System.Collections.Generic;
    using Vector3D = global::SharpDX.Vector3;
    using Point3D = global::SharpDX.Vector3;
    using Point = global::SharpDX.Vector2;
    

    [Flags]
    public enum GeometryFlags
    {
        HasNormals = 0x1,
        HasTexCoords = 0x2,
        Left = 0x4,

        /// <summary>
        ///   The right side.
        /// </summary>
        Right = 0x8,

        /// <summary>
        ///   The front side.
        /// </summary>
        Front = 0x10,

        /// <summary>
        ///   The back side.
        /// </summary>
        Back = 0x20,

        /// <summary>
        ///   All sides.
        /// </summary>
        //All = Top | Bottom | Left | Right | Front | Back
    }

    public class MeshBuilder
    {
        private List<Point3D> m_positions;
        private List<Vector3D> m_normals;
        private List<Point> m_textureCoordinates;
        private List<int> m_triangleIndices;

        public MeshBuilder(bool normals = true, bool texCoords = true)
        {
            this.m_positions = new List<Point3D>();
            this.m_triangleIndices = new List<int>();

            if (texCoords)
                this.m_textureCoordinates = new List<Point>();
            if(normals)
                this.m_normals = new List<Vector3D>();            
        }

        public void Append(params Point3D[] points)
        {
            int i0 = this.m_positions.Count;
            foreach (var p in points) m_positions.Add(p);
            for (int i = 0; i + 2 < points.Length; i++)
            {
                this.m_triangleIndices.Add(i0 + i);
                this.m_triangleIndices.Add(i0 + i + 1);
                this.m_triangleIndices.Add(i0 + i + 2);

                this.m_triangleIndices.Add(i0 + i + 2);
                this.m_triangleIndices.Add(i0 + (i + 3) % points.Length);
                this.m_triangleIndices.Add(i0 + i);
            }
        }

        public void AppendBox(Point3D center, double xlength, double ylength, double zlength, BoxFaces faces = BoxFaces.All)
        {
            if ((faces & BoxFaces.Front) == BoxFaces.Front)
            {
                this.AddCubeFace(center, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), xlength, ylength, zlength);
            }

            if ((faces & BoxFaces.Back) == BoxFaces.Back)
            {
                this.AddCubeFace(center, new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), xlength, ylength, zlength);
            }

            if ((faces & BoxFaces.Left) == BoxFaces.Left)
            {
                this.AddCubeFace(center, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), ylength, xlength, zlength);
            }

            if ((faces & BoxFaces.Right) == BoxFaces.Right)
            {
                this.AddCubeFace(center, new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), ylength, xlength, zlength);
            }

            if ((faces & BoxFaces.Top) == BoxFaces.Top)
            {
                this.AddCubeFace(center, new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), zlength, xlength, ylength);
            }

            if ((faces & BoxFaces.Bottom) == BoxFaces.Bottom)
            {
                this.AddCubeFace(center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), zlength, xlength, ylength);
            }
        }

        private void AddCubeFace(Point3D center, Vector3D normal, Vector3D up, double dist, double width, double height)
        {
            var right = Vector3D.Cross(normal, up);
            var n = normal * (float)dist / 2;
            up *= (float)height / 2;
            right *= (float)width / 2;
            var p1 = center + n - up - right;
            var p2 = center + n - up + right;
            var p3 = center + n + up + right;
            var p4 = center + n + up - right;

            int i0 = this.m_positions.Count;
            this.m_positions.Add(p1);
            this.m_positions.Add(p2);
            this.m_positions.Add(p3);
            this.m_positions.Add(p4);

            if (this.m_normals != null)
            {
                this.m_normals.Add(normal);
                this.m_normals.Add(normal);
                this.m_normals.Add(normal);
                this.m_normals.Add(normal);
            }

            if (this.m_textureCoordinates != null)
            {
                this.m_textureCoordinates.Add(new Point(1, 1));
                this.m_textureCoordinates.Add(new Point(0, 1));
                this.m_textureCoordinates.Add(new Point(0, 0));
                this.m_textureCoordinates.Add(new Point(1, 0));
            }
            
            this.m_triangleIndices.Add(i0 + 2);
            this.m_triangleIndices.Add(i0 + 1);
            this.m_triangleIndices.Add(i0 + 0);

            this.m_triangleIndices.Add(i0 + 0);
            this.m_triangleIndices.Add(i0 + 3);
            this.m_triangleIndices.Add(i0 + 2);            
        }

        public MeshGeometry3D ToMesh()
        {
            return new MeshGeometry3D()
            { 
                Positions = this.m_positions.ToArray(), 
                Normals = this.m_normals.ToArray(), 
                TextureCoordinates = this.m_textureCoordinates.ToArray(), 
                TriangleIndices = this.m_triangleIndices.ToArray(),
            };
        }
    }
}
