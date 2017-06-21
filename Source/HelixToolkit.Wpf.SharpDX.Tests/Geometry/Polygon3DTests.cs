// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Polygon3DTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX.Tests.Geometry
{
    using System.Collections.Generic;
    using global::SharpDX;

    using NUnit.Framework;

    [TestFixture]
    public class Polygon3DTests
    {
        [Test]
        public void Polygon_GetNormal_NotThrows()
        {
            var points = new List<Vector3>()
            {
                new Vector3(-1.39943f, 0.328622f, 0.97968f),
                new Vector3(-1.39969f, 0.328622f, 0.99105f),
                new Vector3(-1.39963f, 0.328631f, 0.99105f),
                new Vector3(-1.39954f, 0.328631f, 0.98726f),
                new Vector3(-1.39937f, 0.328631f, 0.97968f),
            };

            var polygon = new Polygon3D(points);
            Vector3 normal = polygon.GetNormal();
        }
    }
}
