using NUnit.Framework;
using System.Collections;
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

    [Test]
    public void EnumerateToString_Default()
    {
        IEnumerable list = new List<string>()
        {
            "1",
            "2",
            "3"
        };

        string str = list.EnumerateToString();
        Assert.That(str, Is.EqualTo("1 2 3"));
    }

    [Test]
    public void EnumerateToString_Prefix()
    {
        IEnumerable list = new List<string>()
        {
            "1",
            "2",
            "3"
        };

        string str = list.EnumerateToString("v");
        Assert.That(str, Is.EqualTo("v1 v2 v3"));
    }

    [Test]
    public void EnumerateToString_Separator()
    {
        IEnumerable list = new List<string>()
        {
            "1",
            "2",
            "3"
        };

        string str = list.EnumerateToString(null, ";");
        Assert.That(str, Is.EqualTo("1;2;3"));
    }
}
