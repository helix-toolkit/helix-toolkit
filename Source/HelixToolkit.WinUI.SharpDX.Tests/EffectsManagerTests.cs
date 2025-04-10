using HelixToolkit.SharpDX;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SharpDX.Direct3D;
using SharpDX.DXGI;

namespace HelixToolkit.WinUI.SharpDX.Tests;

[TestFixture]
public class EffectsManagerTests
{
    [SetUp]
    public void Setup()
    {
        using var f = new Factory1();
        Adapter[] adapters = f.Adapters;
        if (adapters.Length == 0)
        {
            Assert.Ignore("No graphics adapters found.");
        }
        var hasAdapter = false;
        foreach (var adapter in adapters)
        {
            var level = global::SharpDX.Direct3D11.Device.GetSupportedFeatureLevel(adapter);
            if (level < FeatureLevel.Level_10_0)
            {
                continue;
            }
            hasAdapter = true;
            break;
        }
        if (!hasAdapter)
        {
            Assert.Ignore("No graphics adapters found with FeatureLevel >= 10.0.");
        }        
    }

    [Test]
    public void InitializationTest()
    {


        using (var effectsManager = new DefaultEffectsManager())
        {
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
        }

        var liveObjects = global::SharpDX.Diagnostics.ObjectTracker.FindActiveObjects();
        ClassicAssert.AreEqual(0, liveObjects.Count);
    }
}
