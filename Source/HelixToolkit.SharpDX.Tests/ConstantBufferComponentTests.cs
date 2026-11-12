using HelixToolkit.SharpDX.Core.Components;
using HelixToolkit.SharpDX.Shaders;
using NUnit.Framework;

namespace HelixToolkit.SharpDX.Tests;

[TestFixture]
public class ConstantBufferComponentTests
{
    [Test]
    [Platform("Win")]
    public void DetachReleasesTheSlotAllocatedByAttach()
    {
        using var manager = new EffectsManager(new EffectsManagerConfiguration
        {
            EnableSoftwareRendering = true,
            Initialize2DRendering = false
        });
        manager.AddTechnique(new TechniqueDescription { Name = "SlotReuse" });
        var technique = manager.GetTechnique("SlotReuse");
        Assert.That(technique, Is.Not.Null);
        using var storage = manager.StructArrayPool!.Register(16)!;
        var firstSlot = storage.GetId();
        storage.ReleaseId(firstSlot);

        using var component = new ConstantBufferComponent("SlotReuseBuffer", 16);
        for (var iteration = 0; iteration < 3; iteration++)
        {
            component.Attach(technique);
            component.Detach();

            var nextSlot = storage.GetId();
            try
            {
                Assert.That(nextSlot, Is.EqualTo(firstSlot));
            }
            finally
            {
                storage.ReleaseId(nextSlot);
            }
        }
    }
}
