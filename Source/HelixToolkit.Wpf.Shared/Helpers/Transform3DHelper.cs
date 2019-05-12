// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Transform3DHelper.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Helper methods for Transform3D.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Helper methods for Transform3D.
    /// </summary>
    public static class Transform3DHelper
    {
        /// <summary>
        /// Combines two transforms.
        /// Null Values are treated like the Identity transform.
        /// </summary>
        /// <param name="t1">
        /// The first transform.
        /// </param>
        /// <param name="t2">
        /// The second transform.
        /// </param>
        /// <returns>
        /// The combined transform group.
        /// </returns>
        public static Transform3D CombineTransform(Transform3D t1, Transform3D t2)
        {
            if (t1 == null && t2 == null)
                return Transform3D.Identity;
            if (t1 == null && t2 != null)
                return t2;
            if (t1 != null && t2 == null)
                return t1;
            var g = new MatrixTransform3D(t1.Value * t2.Value);
            return g;
        }

    }
}