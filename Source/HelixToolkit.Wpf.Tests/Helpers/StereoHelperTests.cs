using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.Wpf.Tests.Helpers;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class StereoHelperTests
{
    [Test]
    public void CalculateStereoBase_40mm_ExpectedBehavior()
    {
        Assert.Multiple(() =>
        {
            // fov=48.45deg, 36mm film => 40mm focal length
            Assert.That(StereoHelper.FindFocalLength(48.4554, 36), Is.EqualTo(40).Within(1e-3));

            // F=40mm, P=1.2mm, 2m-10m, 36mm film
            Assert.That(StereoHelper.CalculateStereoBase(1.2, 10e3, 2e3, 40), Is.EqualTo(74.09).Within(1e-2));

            // fov=48.45deg, 1/30, 2m-10m, 36mm film
            Assert.That(StereoHelper.CalculateStereoBase(10e3, 2e3, 36, 1.0 / 30, 48.4554), Is.EqualTo(74.09).Within(1e-2));
        });
    }
}
