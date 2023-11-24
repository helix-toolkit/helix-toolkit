using HelixToolkit.SharpDX;
using NUnit.Framework;

namespace HelixToolkit.Wpf.SharpDX.Tests.Controls;

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

            Assert.That(tech, Is.Not.Null);
            Assert.That(tech.IsNull, Is.False);

            foreach (var passName in tech.ShaderPassNames)
            {
                var p = tech[passName];
                Assert.That(p.IsNULL, Is.False);
            }
        }

        effectsManager.Dispose();
        var liveObjects = global::SharpDX.Diagnostics.ObjectTracker.FindActiveObjects();
        Assert.AreEqual(0, liveObjects.Count);
    }
}
