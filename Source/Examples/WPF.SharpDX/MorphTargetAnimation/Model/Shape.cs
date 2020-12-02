// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shape.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MorphTargetAnimationDemo
{
    using HelixToolkit.Wpf.SharpDX;

    public abstract class Shape
    {
        public Geometry3D Geometry
        {
            get
            {
                return this.GetGeometry();
            }
        }

        protected abstract Geometry3D GetGeometry();

        public System.Windows.Media.Media3D.Transform3D Transform { get; set; }

        public Material Material { get; set; }
    }
}