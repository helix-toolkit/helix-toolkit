using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HelixToolkit.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class NumericHelpersTests
{
    [Test]
    public void NumericHelpers_ParseInt32()
    {
        int result = NumericHelpers.ParseInt32("123".AsSpan(), CultureInfo.InvariantCulture);
        int expected = 123;
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void NumericHelpers_ParseSingle()
    {
        float result = NumericHelpers.ParseSingle("123.456".AsSpan(), CultureInfo.InvariantCulture);
        float expected = 123.456f;
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void NumericHelpers_ParseDouble()
    {
        double result = NumericHelpers.ParseDouble("123.456".AsSpan(), CultureInfo.InvariantCulture);
        double expected = 123.456;
        Assert.That(result, Is.EqualTo(expected));
    }
}
