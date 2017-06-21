// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Polygon3DTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests.Geometry
{
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    using NUnit.Framework;

    [TestFixture]
    public class Polygon3DTests
    {
        [Test]
        public void Polygon_GetNormal_NotThrows()
        {
            var points = new List<Point3D>()
            {
                new Point3D(-1.39943, 0.328622, 0.97968),
                new Point3D(-1.39969, 0.328622, 0.99105),
                new Point3D(-1.39963, 0.328631, 0.99105),
                new Point3D(-1.39954, 0.328631, 0.98726),
                new Point3D(-1.39937, 0.328631, 0.97968),
            };

            var polygon = new Polygon3D(points);
            Vector3D normal = polygon.GetNormal();
        }
    }
}
