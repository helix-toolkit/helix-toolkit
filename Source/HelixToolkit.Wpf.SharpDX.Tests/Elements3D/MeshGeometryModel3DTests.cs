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
using System.IO;

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
            var objects = reader.Read(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Models\obj\Triangle.obj"));

            Assert.AreEqual(1, objects.Count);

            var geometry = objects[0].Geometry;
            var model = new MeshGeometryModel3D { Geometry = geometry };

            var canvas = new CanvasMock();
            model.SceneNode.Attach(canvas.RenderHost);

            Assert.AreEqual(true, model.IsAttached);
        }
    }
}