// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SceneNodeTests.cs" company="Helix Toolkit">
//   Copyright (c) 2020 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using HelixToolkit.SharpDX.Core.Controls;
using HelixToolkit.SharpDX.Core.Model.Scene;
using NUnit.Framework;
using SharpDX;

namespace HelixToolkit.SharpDX.Core.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    class SceneNodeTests
    {

        private SceneNode GetNode()
        {
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(new Vector3(0f), 1, 1, 1);
            return new MeshNode()
            {
                Geometry = meshBuilder.ToMesh(),
            };
        }

        [Test]
        public void HitTestShouldReturnOnePointOnFrontOfCubeWithNoCuttingPlanes()
        {
            var viewport = new ViewportCore(IntPtr.Zero);
            var ray = new Ray(new Vector3(2f, 0f, 0f), new Vector3(-1, 0, 0));
            var hits = new List<HitTestResult>();
            var sceneNode = GetNode();
            sceneNode.HitTest(new HitTestContext(viewport.RenderContext, ref ray), ref hits);
            Assert.AreEqual(1, hits.Count);
            Assert.AreEqual(new Vector3(0.5f, 0, 0), hits[0].PointHit);
        }
    }
}