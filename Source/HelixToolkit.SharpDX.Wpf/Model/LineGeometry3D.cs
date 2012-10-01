namespace HelixToolkit.SharpDX.Wpf
{
    using Point3D = global::SharpDX.Vector3;

    public class LineGeometry3D : Geometry3D
    {
        public Point3D[] Positions { get; set; }

        public int[] LineListIndices { get; set; }
    }
}