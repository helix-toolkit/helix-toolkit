// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EffectsManagerTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NUnit.Framework;

namespace HelixToolkit.SharpDX.Core.Tests
{
    [TestFixture]
    public class EffectsManagerTests
    {
        [Test]
        public void InitializationTest()
        {
            var effectsManager = new DefaultEffectsManager();
            foreach (var techName in effectsManager.RenderTechniques)
            {
                var tech = effectsManager[techName];
                Assert.IsFalse(tech.IsNull);
                foreach (var passName in tech.ShaderPassNames)
                {
                    var p = tech[passName];
                    Assert.IsFalse(p.IsNULL);
                }
            }
            effectsManager.Dispose();
            var liveObjects = global::SharpDX.Diagnostics.ObjectTracker.FindActiveObjects();
            Assert.AreEqual(0, liveObjects.Count);
        }
    }
}
