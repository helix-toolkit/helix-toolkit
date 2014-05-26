namespace HelixToolkit.Wpf.SharpDX
{
    using System;

    using global::SharpDX;

    using HelixToolkit.Wpf.SharpDX.Core;

    [Serializable]
    public abstract class Geometry3D
    {
        public IntCollection Indices { get; set; }
        public Vector3Collection Positions { get; set; }
        public Color4Collection Colors { get; set; }

        public struct Triangle
        {            
            public Vector3 P0, P1, P2;
        }

        public struct Line
        {
            public Vector3 P0, P1;
        }
    }
}