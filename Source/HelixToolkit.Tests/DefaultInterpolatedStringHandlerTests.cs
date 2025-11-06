using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace HelixToolkit.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class DefaultInterpolatedStringHandlerTests
{
    [Test]
    public void ToStringTest()
    {
        var handler = new DefaultInterpolatedStringHandler(1, 0);
        handler.AppendLiteral("value");
        string result = handler.ToString();
        Assert.That(result, Is.EqualTo("value"));
    }

    [Test]
    public void ToStringAndClear()
    {
        var handler = new DefaultInterpolatedStringHandler(1, 0);
        handler.AppendLiteral("value");
        string result = handler.ToStringAndClear();
        Assert.That(result, Is.EqualTo("value"));
    }

    [Test]
    public void AppendLiteral()
    {
        var handler = new DefaultInterpolatedStringHandler(1, 0);
        handler.AppendLiteral("litteral");
        string result = handler.ToStringAndClear();
        Assert.That(result, Is.EqualTo("litteral"));
    }

    [Test]
    public void AppendFormatted()
    {
        var handler = new DefaultInterpolatedStringHandler(0, 1, CultureInfo.InvariantCulture);
        handler.AppendFormatted(123);
        string result = handler.ToStringAndClear();
        Assert.That(result, Is.EqualTo("123"));
    }

    [Test]
    public void AppendLiteralAndFormatted()
    {
        var handler = new DefaultInterpolatedStringHandler(1, 1, CultureInfo.InvariantCulture);
        handler.AppendLiteral("value=");
        handler.AppendFormatted(123);
        string result = handler.ToStringAndClear();
        Assert.That(result, Is.EqualTo("value=123"));
    }
}
