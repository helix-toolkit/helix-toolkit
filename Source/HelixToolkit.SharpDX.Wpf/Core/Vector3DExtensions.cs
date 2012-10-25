namespace HelixToolkit.SharpDX
{
    using System.Windows.Media.Media3D;

    public static class Vector3DExtensions
    {
        public static global::SharpDX.Vector3 ToVector3(this Point3D point)
        {
            return new global::SharpDX.Vector3((float)point.X, (float)point.Y, (float)point.Z);
        }

        public static global::SharpDX.Vector3 ToVector3(this Vector3D vector)
        {
            return new global::SharpDX.Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }

        public static global::SharpDX.Matrix ToMatrix(this Matrix3D m)
        {
            return new global::SharpDX.Matrix(
                (float)m.M11,
                (float)m.M12,
                (float)m.M13,
                (float)m.M14,
                (float)m.M21,
                (float)m.M22,
                (float)m.M23,
                (float)m.M24,
                (float)m.M31,
                (float)m.M32,
                (float)m.M33,
                (float)m.M34,
                (float)m.OffsetX,
                (float)m.OffsetY,
                (float)m.OffsetZ,
                (float)m.M44);
        }
    }
}