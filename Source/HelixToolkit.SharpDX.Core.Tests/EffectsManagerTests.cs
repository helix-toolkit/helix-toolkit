using HelixToolkit.UWP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HelixToolkit.SharpDX.Core.Tests
{
    [TestClass]
    public class EffectsManagerTests
    {
        [TestMethod]
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
