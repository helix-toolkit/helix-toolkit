namespace HelixToolkit.SharpDX.Wpf
{
    using System.Collections.Generic;
    using Vector3D = global::SharpDX.Vector3;
    using Point3D = global::SharpDX.Vector3;

    public class LineBuilder
    {
        private List<Point3D> positions;
        private List<int> lineListIndices;

        public LineBuilder()
        {
            positions = new List<Point3D>();
            // textureCoordinates = new List<Point>();
            lineListIndices = new List<int>();
        }

        public void Append(bool isClosed, params Point3D[] points)
        {
            int i0 = positions.Count;
            foreach (var p in points) positions.Add(p);
            for (int i = 0; i + 1 < points.Length; i++)
            {
                this.lineListIndices.Add(i0 + i);
                this.lineListIndices.Add(i0 + i + 1);
            }

            if (isClosed)
            {
                this.lineListIndices.Add(i0 + points.Length - 1);
                this.lineListIndices.Add(i0);
            }
        }

        public void AppendBox(Point3D center, double xlength, double ylength, double zlength)
        {
            int i0 = positions.Count;
            var dx = new Vector3D((float)xlength, 0, 0);
            var dy = new Vector3D(0, (float)ylength, 0);
            var dz = new Vector3D(0, 0, (float)zlength);
            this.Append(true, center - dx - dy - dz, center + dx - dy - dz, center + dx + dy - dz, center - dx + dy - dz);
            this.Append(true, center - dx - dy + dz, center + dx - dy + dz, center + dx + dy + dz, center - dx + dy + dz);
            lineListIndices.AddRange(new[] { i0 + 0, i0 + 4, i0 + 1, i0 + 5, i0 + 2, i0 + 6, i0 + 3, i0 + 7 });
        }

        public LineGeometry3D ToLine()
        {
            return new LineGeometry3D { Positions = this.positions.ToArray(), LineListIndices = this.lineListIndices.ToArray() };
        }
    }
}
