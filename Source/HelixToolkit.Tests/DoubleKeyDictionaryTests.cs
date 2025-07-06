using NUnit.Framework;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class DoubleKeyDictionaryTests
{
    [Test]
    public void This_Valid()
    {
        var dict = new DoubleKeyDictionary<int, int, int>();

        dict[1, 2] = 3;
        int value = dict[1, 2];

        Assert.That(value, Is.EqualTo(3));
    }

    [Test]
    public void Clear_Valid()
    {

        var dict = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [1, 3] = 6,
            [2, 2] = 7,
            [2, 3] = 8,
        };

        dict.Clear();

        Assert.That(dict.Values.Count, Is.EqualTo(0));
    }

    [Test]
    public void Add_Valid()
    {
        var dict = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [1, 3] = 6,
            [2, 2] = 7,
            [2, 3] = 8,
        };

        IEnumerable<int> expected = [5, 6, 7, 8];

        Assert.That(dict.Values, Is.EquivalentTo(expected));
        Assert.That(dict.ContainsKey(1, 2), Is.True);
        Assert.That(dict.ContainsKey(1, 3), Is.True);
        Assert.That(dict.ContainsKey(2, 2), Is.True);
        Assert.That(dict.ContainsKey(2, 3), Is.True);
    }

    [Test]
    public void Add_SetValue_Valid()
    {
        var dict = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [2, 2] = 7,
        };

        dict[1, 2] = 6;
        dict[2, 2] = 8;

        IEnumerable<int> expected = [6, 8];

        Assert.That(dict.Values, Is.EquivalentTo(expected));
        Assert.That(dict.ContainsKey(1, 2), Is.True);
        Assert.That(dict.ContainsKey(1, 3), Is.False);
        Assert.That(dict.ContainsKey(2, 2), Is.True);
        Assert.That(dict.ContainsKey(2, 3), Is.False);
    }

    [Test]
    public void Remove_Valid()
    {
        var dict = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [1, 3] = 6,
            [2, 2] = 7,
            [2, 3] = 8,
        };

        dict.Remove(1, 3);
        dict.Remove(2, 2);
        dict.Remove(2, 3);

        Assert.That(dict.ContainsKey(1, 2), Is.True);
        Assert.That(dict.ContainsKey(1, 3), Is.False);
        Assert.That(dict.ContainsKey(2, 2), Is.False);
        Assert.That(dict.ContainsKey(2, 3), Is.False);
    }

    [Test]
    public void TryGetValue_Valid()
    {
        var dict = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [1, 3] = 6,
        };

        int v;

        Assert.That(dict.TryGetValue(0, 1, out v), Is.False);
        Assert.That(v, Is.EqualTo(0));

        Assert.That(dict.TryGetValue(1, 1, out v), Is.False);
        Assert.That(v, Is.EqualTo(0));

        Assert.That(dict.TryGetValue(1, 2, out v), Is.True);
        Assert.That(v, Is.EqualTo(5));
    }

    [Test]
    public void GetHashCode_Valid()
    {
        var dict0 = new DoubleKeyDictionary<int, int, int>()
        {
            [10, 20] = 30,
        };

        var dict1 = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [3, 4] = 6,
        };

        int h0 = dict0.GetHashCode();
        int h1 = dict1.GetHashCode();
        int h2 = dict1.GetHashCode();

        Assert.That(h0, Is.Not.EqualTo(h1));
        Assert.That(h1, Is.EqualTo(h2));
    }

    [Test]
    public void GetEnumerator_Valid()
    {
        var dict = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [3, 4] = 6,
        };

        IEnumerator<DoubleKeyPairValue<int, int, int>> enumerator = dict.GetEnumerator();
        DoubleKeyDictionary<int, int, int> values = new();

        while (enumerator.MoveNext())
        {
            values.Add(enumerator.Current.Key1, enumerator.Current.Key2, enumerator.Current.Value);
        }

        var expected = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [3, 4] = 6,
        };

        Assert.That(values, Is.EquivalentTo(expected));
    }

    [Test]
    public void IEnumerable_GetEnumerator_Valid()
    {
        var dict = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [3, 4] = 6,
        };

        IEnumerator<DoubleKeyPairValue<int, int, int>> enumerator = (IEnumerator<DoubleKeyPairValue<int, int, int>>)((IEnumerable)dict).GetEnumerator();
        DoubleKeyDictionary<int, int, int> values = new();

        while (enumerator.MoveNext())
        {
            values.Add(enumerator.Current.Key1, enumerator.Current.Key2, enumerator.Current.Value);
        }

        var expected = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [3, 4] = 6,
        };

        Assert.That(values, Is.EquivalentTo(expected));
    }

    [Test]
    public void Equals_Valid()
    {
        var dict0 = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [3, 4] = 6,
        };

        var dict1 = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
            [3, 4] = 6,
        };

        var dict2 = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 2] = 5,
        };

        var dict3 = new DoubleKeyDictionary<int, int, int>()
        {
            [2, 1] = 5,
        };

        var dict4 = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 20] = 5,
            [3, 40] = 6,
            [3, 50] = 7,
        };

        var dict5 = new DoubleKeyDictionary<int, int, int>()
        {
            [10, 2] = 11,
            [20, 4] = 12,
        };

        var dict6 = new DoubleKeyDictionary<int, int, int>()
        {
            [1, 5] = 2,
            [3, 6] = 4,
        };

        Assert.That(dict0.Equals(dict1), Is.True);
        Assert.That(dict0.Equals(dict2), Is.False);
        Assert.That(dict0.Equals(dict3), Is.False);
        Assert.That(dict0.Equals(dict4), Is.False);
        Assert.That(dict0.Equals(dict5), Is.False);
        Assert.That(dict0.Equals(dict6), Is.False);
        Assert.That(dict0.Equals(null), Is.False);
    }
}
