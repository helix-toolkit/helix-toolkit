// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshGeometryModel3DTests.cs" company="Helix Toolkit">
//   Copyright (c) 2020 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX.Tests.Controls;
using NUnit.Framework;
using System.IO;
using System.Threading;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Tests.Elements3D
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    class MeshGeometryModel3DTests
    {
        /// <summary>
        /// Test for pull request #54.
        /// Fix IndexOutOfRange exception in CreateDefaultVertexArray.
        /// </summary>
        [Test]
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

        private MeshGeometryModel3D GetGeometryModel3D()
        {
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(new Vector3(0f), 1, 1, 1);
            return new MeshGeometryModel3D()
            {
                Geometry = meshBuilder.ToMesh(),
            };
        }

        [Test]
        public void HitTestShouldReturnOnePointOnFrontOfCubeWithNoCuttingPlanes()
        {
            var viewport = new Viewport3DX();
            var ray = new Ray(new Vector3(2f, 0f, 0f), new Vector3(-1, 0, 0));
            var hits = new List<HitTestResult>();
            var geometryModel3D = GetGeometryModel3D();
            geometryModel3D.HitTest(new HitTestContext(viewport.RenderContext, ref ray), ref hits);
            Assert.AreEqual(1, hits.Count);
            Assert.AreEqual(new Vector3(0.5f, 0, 0), hits[0].PointHit);
        }
    }
}