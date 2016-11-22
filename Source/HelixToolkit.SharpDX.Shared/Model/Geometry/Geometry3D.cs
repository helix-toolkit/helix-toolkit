// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Geometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



namespace HelixToolkit.Wpf.SharpDX
{
    using System;

    using global::SharpDX;

    using HelixToolkit.Wpf.SharpDX.Core;

    using System.Runtime.InteropServices;

#if !NETFX_CORE
    [Serializable]
#endif
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

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PointsVertex
        {
            public Vector4 Position;
            public Color4 Color;
            public const int SizeInBytes = 4 * (4 + 4);
        }

        public struct Point
        {
            public Vector3 P0;
        }
    }
}
