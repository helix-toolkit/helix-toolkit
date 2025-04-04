using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class ArrayExtensionsTests
{
    [Test]
    public void ArrayExtensions_Convert_Default()
    {
        int[,] source = { { 1, 2, 3 }, { 4, 5, 6 } };
        long[,] result = source.Convert<int, long>();
        long[,] expected = { { 1, 2, 3 }, { 4, 5, 6 } };

        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void ArrayExtensions_Convert_Conversion()
    {
        int[,] source = { { 1, 2, 3 }, { 4, 5, 6 } };
        long[,] result = source.Convert(t => (long)t * 10);
        long[,] expected = { { 10, 20, 30 }, { 40, 50, 60 } };

        Assert.That(result, Is.EquivalentTo(expected));
    }
}
