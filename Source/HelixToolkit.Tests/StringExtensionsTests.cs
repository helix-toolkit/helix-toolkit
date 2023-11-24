using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class StringExtensionsTests
{
    [Test]
    public void SplitOnWhitespace()
    {
        var s1 = "1 2  3".SplitOnWhitespace();

        Assert.Multiple(() =>
        {
            Assert.That(s1, Has.Length.EqualTo(3));
            Assert.That(s1[0], Is.EqualTo("1"));
            Assert.That(s1[1], Is.EqualTo("2"));
            Assert.That(s1[2], Is.EqualTo("3"));
        });
    }

    [Test]
    public void SplitOnWhitespace_IncludingStartAndEndWhitespace()
    {
        var s1 = " 1 2  3 ".SplitOnWhitespace();

        Assert.Multiple(() =>
        {
            Assert.That(s1, Has.Length.EqualTo(3));
            Assert.That(s1[0], Is.EqualTo("1"));
            Assert.That(s1[1], Is.EqualTo("2"));
            Assert.That(s1[2], Is.EqualTo("3"));
        });
    }

    [Test]
    public void SplitOnWhitespace_IncludingTabsAndNewline()
    {
        var s1 = " 1 \t 2 \n  3 ".SplitOnWhitespace();

        Assert.Multiple(() =>
        {
            Assert.That(s1, Has.Length.EqualTo(3));
            Assert.That(s1[0], Is.EqualTo("1"));
            Assert.That(s1[1], Is.EqualTo("2"));
            Assert.That(s1[2], Is.EqualTo("3"));
        });
    }
}
