using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class DoubleKeyPairValueTests
{
    [Test]
    public void DoubleKeyPairValue_Ctor_Valid()
    {
        var p = new DoubleKeyPairValue<int, double, string>(2, 3.1, "4");

        Assert.That(p.Key1, Is.EqualTo(2));
        Assert.That(p.Key2, Is.EqualTo(3.1));
        Assert.That(p.Value, Is.EqualTo("4"));
    }

    [Test]
    public void DoubleKeyPairValue_Key1_Valid()
    {
        var p = new DoubleKeyPairValue<int, double, string>(2, 3.1, "4");
        p.Key1 = 20;

        Assert.That(p.Key1, Is.EqualTo(20));
    }

    [Test]
    public void DoubleKeyPairValue_Key2_Valid()
    {
        var p = new DoubleKeyPairValue<int, double, string>(2, 3.1, "4");
        p.Key2 = 31.2;

        Assert.That(p.Key2, Is.EqualTo(31.2));
    }

    [Test]
    public void DoubleKeyPairValue_Value_Valid()
    {
        var p = new DoubleKeyPairValue<int, double, string>(2, 3.1, "4");
        p.Value = "40";

        Assert.That(p.Value, Is.EqualTo("40"));
    }

    [Test, SetCulture("en")]
    public void DoubleKeyPairValue_ToString_Valid()
    {
        var p = new DoubleKeyPairValue<int, double, string?>(2, 3.1, "4");
        string s = p.ToString();

        Assert.That(s, Is.EqualTo("2 - 3.1 - 4"));
    }

    [Test, SetCulture("en")]
    public void DoubleKeyPairValue_ToString_Null()
    {
        var p = new DoubleKeyPairValue<int, double, string?>(2, 3.1, null);
        string s = p.ToString();

        Assert.That(s, Is.EqualTo("2 - 3.1 - "));
    }
}
