// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Geometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX
{
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

        public struct Point
        {
            public Vector3 P0;
        }
    }
}