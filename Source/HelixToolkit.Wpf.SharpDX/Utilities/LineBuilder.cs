namespace HelixToolkit.Wpf.SharpDX
{
    using System;

    using HelixToolkit.Wpf.SharpDX.Core;

    using Vector3D = global::SharpDX.Vector3;
    using Vector3 = global::SharpDX.Vector3;
    using Point3D = global::SharpDX.Vector3;    
    

    public class LineBuilder
    {
        private Vector3Collection positions;
        private IntCollection lineListIndices;

        public LineBuilder()
        {
            positions = new Vector3Collection();
            // textureCoordinates = new List<Point>();
            lineListIndices = new IntCollection();
        }

        public void Add(bool isClosed, params Point3D[] points)
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

        public void AddBox(Point3D center, double xlength, double ylength, double zlength)
        {
            int i0 = positions.Count;
            var dx = new Vector3D((float)xlength/2f, 0, 0);
            var dy = new Vector3D(0, (float)ylength/2f, 0);
            var dz = new Vector3D(0, 0, (float)zlength/2f);
            this.Add(true, center - dx - dy - dz, center + dx - dy - dz, center + dx + dy - dz, center - dx + dy - dz);
            this.Add(true, center - dx - dy + dz, center + dx - dy + dz, center + dx + dy + dz, center - dx + dy + dz);
            lineListIndices.AddRange(new[] { i0 + 0, i0 + 4, i0 + 1, i0 + 5, i0 + 2, i0 + 6, i0 + 3, i0 + 7 });
        }

        public void AddLine(Point3D p1, Point3D p2)
        {
            int i0 = positions.Count;
            this.positions.Add(p1);
            this.positions.Add(p2);
            this.lineListIndices.Add(i0);
            this.lineListIndices.Add(i0 + 1);
        }


        public void AddGrid(BoxFaces plane, int columns, int rows, float width, float height)
        {
            // checks
            if (columns < 2 || rows < 2)
            {
                throw new ArgumentNullException("columns or rows too small");
            }
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentNullException("width or height too small");
            }

            // step
            var stepy = height / (rows - 1);
            var stepx = width / (columns - 1);

            float minx = 0, miny = 0;
            float maxx = width;
            float maxy = height;

            for (int y = 0; y < rows; y++)
            {
                this.AddLine(new Vector3(minx, stepy * y, 0), new Vector3(maxx, stepy * y, 0));
            }
            for (int x = 0; x < columns; x++)
            {
                this.AddLine(new Vector3(stepx * x, miny, 0), new Vector3(stepx * x, maxy, 0));           
            }
        }
             

        public LineGeometry3D ToLineGeometry3D()
        {
            return new LineGeometry3D { Positions = this.positions, Indices = this.lineListIndices };
        }
               
        /// <summary>
        /// Generates a square grid with a step of 1.0 
        /// </summary>
        /// <returns></returns>
        public static LineGeometry3D GenerateGrid(int width = 10)
        {
            return GenerateGrid(Vector3.UnitY, 0, width);
        }

        /// <summary>
        /// Generates a square grid with a step of 1.0 
        /// </summary>
        /// <returns></returns>
        public static LineGeometry3D GenerateGrid(Vector3 plane, int min0 = 0, int max0 = 10, int min1 = 0, int max1 = 10)
        {
            var grid = new LineBuilder();
            //int width = max - min;
            if (plane == Vector3.UnitX)
            {
                for (int i = min0; i <= max0; i++)
                {
                    grid.AddLine(new Vector3(0, i, min1), new Vector3(0, i, max1));                    
                }
                for (int i = min1; i <= max1; i++)
                {                    
                    grid.AddLine(new Vector3(0, min0, i), new Vector3(0, max0, i));
                }
            }
            else if (plane == Vector3.UnitY)
            {
                for (int i = min0; i <= max0; i++)
                {
                    grid.AddLine(new Vector3(i, 0, min1), new Vector3(i, 0, max1));
                    
                }
                for (int i = min1; i <= max1; i++)
                {
                    grid.AddLine(new Vector3(min0, 0, i), new Vector3(max0, 0, i));
                }
            }
            else
            {
                for (int i = min0; i <= max0; i++)
                {
                    grid.AddLine(new Vector3(i, min1, 0), new Vector3(i, max1, 0));                    
                }
                for (int i = min1; i <= max1; i++)
                {
                    grid.AddLine(new Vector3(min0, i, 0), new Vector3(max0, i, 0));
                }
            }

            return grid.ToLineGeometry3D();
        }

        
        /// <summary>
        /// Generates a square grid with a step of 1.0 
        /// </summary>
        /// <returns></returns>
        public static LineGeometry3D GenerateGrid(Vector3 plane, int min = 0, int max = 10)
        {
            var grid = new LineBuilder();
            //int width = max - min;
            if (plane == Vector3.UnitX)
            {
                for (int i = min; i <= max; i++)
                {
                    grid.AddLine(new Vector3(0, i, min), new Vector3(0, i, max));
                    grid.AddLine(new Vector3(0, min, i), new Vector3(0, max, i));
                }
            }
            else if (plane == Vector3.UnitY)
            {
                for (int i = min; i <= max; i++)
                {
                    grid.AddLine(new Vector3(i, 0, min), new Vector3(i, 0, max));
                    grid.AddLine(new Vector3(min, 0, i), new Vector3(max, 0, i));
                }
            }
            else
            {
                for (int i = min; i <= max; i++)
                {
                    grid.AddLine(new Vector3(i, min, 0), new Vector3(i, max, 0));
                    grid.AddLine(new Vector3(min, i, 0), new Vector3(max, i, 0));
                }
            }

            return grid.ToLineGeometry3D();
        }

        /// <summary>
        /// Returns a line geometry of the axis-aligned bounding-box of the given mesh.
        /// </summary>
        /// <param name="mesh">Input mesh for the computation of the b-box</param>
        /// <returns></returns>
        public static LineGeometry3D GenerateBoundingBox(Geometry3D mesh)
        {
            var bb = global::SharpDX.BoundingBox.FromPoints(mesh.Positions.Array);
            return GenerateBoundingBox(bb);
        }

        /// <summary>
        /// Returns a line geometry of the axis-aligned bounding-box of the given mesh.
        /// </summary>
        /// <param name="mesh">Input mesh for the computation of the b-box</param>
        /// <returns></returns>
        public static LineGeometry3D GenerateBoundingBox(Vector3[] points)
        {
            var bb = global::SharpDX.BoundingBox.FromPoints(points);
            return GenerateBoundingBox(bb);
        }

        /// <summary>
        /// Returns a line geometry of the axis-aligned bounding-box of the given mesh.
        /// </summary>
        /// <param name="mesh">Input mesh for the computation of the b-box</param>
        /// <returns></returns>
        public static LineGeometry3D GenerateBoundingBox(global::SharpDX.BoundingBox bb)
        {            
            var cc = bb.GetCorners();
            var ll = new LineBuilder();
            ll.AddLine(cc[0], cc[1]);
            ll.AddLine(cc[1], cc[2]);
            ll.AddLine(cc[2], cc[3]);
            ll.AddLine(cc[3], cc[0]);

            ll.AddLine(cc[4], cc[5]);
            ll.AddLine(cc[5], cc[6]);
            ll.AddLine(cc[6], cc[7]);
            ll.AddLine(cc[7], cc[4]);

            ll.AddLine(cc[0], cc[4]);
            ll.AddLine(cc[1], cc[5]);
            ll.AddLine(cc[2], cc[6]);
            ll.AddLine(cc[3], cc[7]);
            return ll.ToLineGeometry3D();
        }
    }
}
