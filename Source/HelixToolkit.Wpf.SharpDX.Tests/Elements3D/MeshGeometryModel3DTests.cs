// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshGeometryModel3DTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Test for pull request #54.
//   Fix IndexOutOfRange exception in CreateDefaultVertexArray.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using HelixToolkit.Wpf.SharpDX.Tests.Controls;
using NUnit.Framework;

namespace HelixToolkit.Wpf.SharpDX.Tests.Elements3D
{
    [TestFixture]
    class MeshGeometryModel3DTests
    {
        /// <summary>
        /// Test for pull request #54.
        /// Fix IndexOutOfRange exception in CreateDefaultVertexArray.
        /// </summary>
        [Test, STAThread]
        public void CreateDefaultVertexArrayForTriangle()
        {
            var reader = new ObjReader();
            var objects = reader.Read(@"Models\obj\Triangle.obj");

            Assert.AreEqual(1, objects.Count);

            var geometry = objects[0].Geometry;
            var model = new MeshGeometryModel3D { Geometry = geometry };

            var canvas = new CanvasMock();
            model.Attach(canvas);

            Assert.AreEqual(true, model.IsAttached);
        }
    }
}