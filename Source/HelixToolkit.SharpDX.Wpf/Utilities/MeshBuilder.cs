namespace HelixToolkit.SharpDX.Wpf
{
    using System.Collections.Generic;
    using Vector3D = global::SharpDX.Vector3;
    using Point3D = global::SharpDX.Vector3;
    using Point = global::SharpDX.Vector2;

    public class MeshBuilder
    {
        private List<Point3D> positions;
        // private List<Point> textureCoordinates;
        private List<int> triangleIndices;

        public MeshBuilder()
        {
            this.positions = new List<Point3D>();
            // textureCoordinates = new List<Point>();
            this.triangleIndices = new List<int>();
        }

        public void Append(params Point3D[] points)
        {
            int i0 = this.positions.Count;
            foreach (var p in points) positions.Add(p);
            for (int i = 0; i + 2 < points.Length; i++)
            {
                this.triangleIndices.Add(i0 + i);
                this.triangleIndices.Add(i0 + i + 1);
                this.triangleIndices.Add(i0 + i + 2);

                this.triangleIndices.Add(i0 + i + 2);
                this.triangleIndices.Add(i0 + (i + 3) % points.Length);
                this.triangleIndices.Add(i0 + i);
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

            int i0 = this.positions.Count;
            this.positions.Add(p1);
            this.positions.Add(p2);
            this.positions.Add(p3);
            this.positions.Add(p4);
            //if (this.normals != null)
            //{
            //    this.normals.Add(normal);
            //    this.normals.Add(normal);
            //    this.normals.Add(normal);
            //    this.normals.Add(normal);
            //}

            //if (this.textureCoordinates != null)
            //{
            //    this.textureCoordinates.Add(new Point(1, 1));
            //    this.textureCoordinates.Add(new Point(0, 1));
            //    this.textureCoordinates.Add(new Point(0, 0));
            //    this.textureCoordinates.Add(new Point(1, 0));
            //}

            this.triangleIndices.Add(i0 + 2);
            this.triangleIndices.Add(i0 + 1);
            this.triangleIndices.Add(i0 + 0);
            this.triangleIndices.Add(i0 + 0);
            this.triangleIndices.Add(i0 + 3);
            this.triangleIndices.Add(i0 + 2);
        }

        public MeshGeometry3D ToMesh()
        {
            return new MeshGeometry3D { Positions = this.positions.ToArray(), TriangleIndices = this.triangleIndices.ToArray() };
        }
    }
}
