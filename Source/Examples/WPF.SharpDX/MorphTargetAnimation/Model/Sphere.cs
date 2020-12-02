// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sphere.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MorphTargetAnimationDemo
{
    using HelixToolkit.Wpf;
    using HelixToolkit.Wpf.SharpDX;

    using SharpDX;

    public class Sphere : Shape
    {
        private static Geometry3D geometry;

        static Sphere()
        {
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            geometry = b1.ToMeshGeometry3D();
        }

        protected override Geometry3D GetGeometry()
        {
            return geometry;
        }
    }
}