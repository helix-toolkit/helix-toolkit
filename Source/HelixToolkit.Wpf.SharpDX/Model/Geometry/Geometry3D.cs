namespace HelixToolkit.Wpf.SharpDX
{
    using System;

    using global::SharpDX;

    using Point3D = global::SharpDX.Vector3;

    [Serializable]
    public abstract class Geometry3D
    {
        public int[] Indices { get; set; }
        public Point3D[] Positions { get; set; }
        public Color4[] Colors { get; set; }


        public struct Triangle
        {            
            public Point3D P0, P1, P2;
        }

        public struct Line
        {
            public Point3D P0, P1;
        }
    }
}