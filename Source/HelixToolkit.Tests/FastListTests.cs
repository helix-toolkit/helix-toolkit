using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class FastListTests
{
    [Test]
    public void Ctor_Valid()
    {
        var list = new FastList<int>();

        Assert.That(list.Capacity, Is.EqualTo(0));
        Assert.That(list.Count, Is.EqualTo(0));
    }

    [Test]
    public void Ctor_Capacity_Valid()
    {
        var list = new FastList<int>(5);

        Assert.That(list.Capacity, Is.EqualTo(5));
        Assert.That(list.Count, Is.EqualTo(0));
    }

    [Test]
    public void Ctor_Collection_FastList_Valid()
    {
        var collection = new FastList<int>
        {
            1,
            2,
            3
        };

        var list = new FastList<int>(collection);

        Assert.That(list, Is.EquivalentTo(collection));
    }

    [Test]
    public void Ctor_Collection_ICollection_Valid()
    {
        var collection = new List<int>
        {
            1,
            2,
            3
        };

        var list = new FastList<int>(collection);

        Assert.That(list, Is.EquivalentTo(collection));
    }

    [Test]
    public void Ctor_Collection_IEnumerable_Valid()
    {
        var collection = Enumerable.Range(1, 3);

        var list = new FastList<int>(collection);

        Assert.That(list, Is.EquivalentTo(collection));
    }

    [TestCase(-1, 0)]
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(5, 5)]
    public void SetCapacity_Valid(int capacity, int expected)
    {
        var list = new FastList<int>();
        list.Capacity = capacity;
        Assert.That(list.Capacity, Is.EqualTo(expected));
    }

    [Test]
    public void SetCapacity_Copy_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        list.Capacity = 10;
        Assert.That(list, Is.EquivalentTo([1, 2, 3]));
    }

    [Test]
    public void Clear_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        list.Clear();
        Assert.That(list.Count, Is.Zero);
    }

    [TestCase(false, 0, 3, 0)]
    [TestCase(true, 0, 3, 3)]
    public void Clear_Bool_Valid(bool fast, int count, int capacity, int value)
    {
        var list = new FastList<int>([1, 2, 3]);
        list.Clear(fast);
        Assert.That(list.Count, Is.EqualTo(count));
        Assert.That(list.Capacity, Is.EqualTo(capacity));
        Assert.That(list.GetInternalArray()[2], Is.EqualTo(value));
    }

    [TestCase(null!, false)]
    [TestCase("1", true)]
    [TestCase("2", true)]
    [TestCase("5", false)]
    public void Contains_Valid(string value, bool result)
    {
        var list = new FastList<string>(["1", "2", "3"]);
        bool contains = list.Contains(value);
        Assert.That(contains, Is.EqualTo(result));
    }

    [TestCase(null!, true)]
    public void Contains_Null_Valid(string value, bool result)
    {
        var list = new FastList<string>(["1", null!, "3"]);
        bool contains = list.Contains(value);
        Assert.That(contains, Is.EqualTo(result));
    }

    [TestCase(1, 0)]
    [TestCase(3, 2)]
    [TestCase(10, -1)]
    public void IndexOf_Valid(int value, int expected)
    {
        var list = new FastList<int>([1, 2, 3]);
        int index = list.IndexOf(value);
        Assert.That(index, Is.EqualTo(expected));
    }

    [Test]
    public void Insert_End_Valid()
    {
        var list = new FastList<int>([1, 2]);
        list.Insert(2, 3);
        Assert.That(list, Is.EquivalentTo([1, 2, 3]));
    }

    [Test]
    public void Insert_Middle_Valid()
    {
        var list = new FastList<int>([1, 3]);
        list.Insert(1, 2);
        Assert.That(list, Is.EquivalentTo([1, 2, 3]));
    }

    [Test]
    public void Remove_Exist_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        bool r = list.Remove(2);
        Assert.That(r, Is.True);
        Assert.That(list, Is.EquivalentTo([1, 3]));
    }

    [Test]
    public void Remove_NotExist_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        bool r = list.Remove(5);
        Assert.That(r, Is.False);
        Assert.That(list, Is.EquivalentTo([1, 2, 3]));
    }

    [Test]
    public void RemoveAt_InvalidIndex_Throw()
    {
        var list = new FastList<int>([1, 2, 3]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(3));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(5));
    }

    [Test]
    public void At_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        ref int v = ref list.At(1);
        v = 20;
        Assert.That(list, Is.EquivalentTo([1, 20, 3]));
    }

    [Test]
    public void IsReadOnly_Vaid()
    {
        var list = new FastList<int>();
        bool readOnly = ((ICollection<int>)list).IsReadOnly;
        Assert.That(readOnly, Is.False);
    }

    [Test]
    public void Resize_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        list.Resize(10, true);
        Assert.That(list.Capacity, Is.EqualTo(10));
    }

    [Test]
    public void AsReadOnly_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        var list2 = list.AsReadOnly();
        Assert.That(list2, Is.EquivalentTo([1, 2, 3]));
    }

    [Test]
    public void BinarySearch_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        int index = list.BinarySearch(3);
        Assert.That(index, Is.EqualTo(2));
    }

    [Test]
    public void BinarySearch_Comparer_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        Comparer<int> comparer = Comparer<int>.Create((a, b) => (a + 1).CompareTo(b + 1));
        int index = list.BinarySearch(3, comparer);
        Assert.That(index, Is.EqualTo(2));
    }

    [Test]
    public void BinarySearch_Index_Count_Comparer_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        Comparer<int> comparer = Comparer<int>.Create((a, b) => (a + 1).CompareTo(b + 1));
        int index = list.BinarySearch(1, 2, 3, comparer);
        Assert.That(index, Is.EqualTo(2));
    }

    [Test]
    public void CopyTo_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        var array = new int[5];
        list.CopyTo(array);
        Assert.That(array, Is.EquivalentTo([1, 2, 3, 4, 5]));
    }

    [Test]
    public void CopyTo_Inddex_Count_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        var array = new int[2];
        list.CopyTo(1, array, 0, 2);
        Assert.That(array, Is.EquivalentTo([2, 3]));
    }

    [Test]
    public void Exist_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        bool exist = list.Exists(t => t == 2);
        Assert.That(exist, Is.True);
    }

    [Test]
    public void NotExist_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        bool exist = list.Exists(t => t == 5);
        Assert.That(exist, Is.False);
    }

    [Test]
    public void Find_Valid()
    {
        var list = new FastList<int>([1, 2, 2, 3]);
        int value = list.Find(t => t == 2);
        Assert.That(value, Is.EqualTo(2));
    }

    [Test]
    public void NotFind_Valid()
    {
        var list = new FastList<int>([1, 2, 2, 3]);
        int value = list.Find(t => t == 5);
        Assert.That(value, Is.EqualTo(0));
    }

    [Test]
    public void FindAll_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        FastList<int> values = list.FindAll(t => t >= 2 && t <= 4);
        Assert.That(values, Is.EquivalentTo([2, 3, 4]));
    }

    [Test]
    public void FindIndex_Valid()
    {
        var list = new FastList<int>([1, 2, 2, 3]);
        int index = list.FindIndex(t => t == 2);
        Assert.That(index, Is.EqualTo(1));
    }

    [Test]
    public void FindNotIndex_Valid()
    {
        var list = new FastList<int>([1, 2, 2, 3]);
        int index = list.FindIndex(t => t == 5);
        Assert.That(index, Is.EqualTo(-1));
    }

    [Test]
    public void FindIndex_Start_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 3, 4, 5]);
        int index = list.FindIndex(1, t => t == 3);
        Assert.That(index, Is.EqualTo(2));
    }

    [Test]
    public void FindIndex_Index_Count_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 3, 4, 5]);
        int index = list.FindIndex(1, 3, t => t == 3);
        Assert.That(index, Is.EqualTo(2));
    }

    [Test]
    public void FindNotIndex_Start_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 3, 4, 5]);
        int index = list.FindIndex(1, t => t == 30);
        Assert.That(index, Is.EqualTo(-1));
    }

    [Test]
    public void FindNotIndex_Index_Count_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 3, 4, 5]);
        int index = list.FindIndex(1, 3, t => t == 30);
        Assert.That(index, Is.EqualTo(-1));
    }

    [Test]
    public void FindLast_Valid()
    {
        var list = new FastList<int>([1, 2, 2, 3]);
        int value = list.FindLast(t => t == 2);
        Assert.That(value, Is.EqualTo(2));
    }

    [Test]
    public void NotFindLast_Valid()
    {
        var list = new FastList<int>([1, 2, 2, 3]);
        int value = list.FindLast(t => t == 5);
        Assert.That(value, Is.EqualTo(0));
    }

    [Test]
    public void FindLastIndex_Valid()
    {
        var list = new FastList<int>([1, 2, 2, 3]);
        int index = list.FindLastIndex(t => t == 2);
        Assert.That(index, Is.EqualTo(2));
    }

    [Test]
    public void FindNotLastIndex_Valid()
    {
        var list = new FastList<int>([1, 2, 2, 3]);
        int index = list.FindLastIndex(t => t == 5);
        Assert.That(index, Is.EqualTo(-1));
    }

    [Test]
    public void FindLastIndex_Start_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 3, 4, 5]);
        int index = list.FindLastIndex(4, t => t == 3);
        Assert.That(index, Is.EqualTo(3));
    }

    [Test]
    public void FindLastIndex_Index_Count_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 3, 4, 5]);
        int index = list.FindLastIndex(4, 3, t => t == 3);
        Assert.That(index, Is.EqualTo(3));
    }

    [Test]
    public void FindNotLastIndex_Start_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 3, 4, 5]);
        int index = list.FindLastIndex(4, t => t == 30);
        Assert.That(index, Is.EqualTo(-1));
    }

    [Test]
    public void FindNotLastIndex_Index_Count_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 3, 4, 5]);
        int index = list.FindLastIndex(4, 3, t => t == 30);
        Assert.That(index, Is.EqualTo(-1));
    }

    [Test]
    public void ForEach_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        var items = new List<int>();
        list.ForEach(t => items.Add(t * 2));
        Assert.That(items, Is.EquivalentTo([2, 4, 6]));
    }

    [Test]
    public void GetRange_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        FastList<int> list2 = list.GetRange(1, 3);
        Assert.That(list2, Is.EquivalentTo([2, 3, 4]));
    }

    [TestCase(10, 0, -1)]
    [TestCase(2, 1, 2)]
    public void IndexOf_Index_Valid(int value, int start, int index)
    {
        var list = new FastList<int>([0, 1, 2, 3, 4]);
        int i = list.IndexOf(value, start);
        Assert.That(i, Is.EqualTo(index));
    }

    [TestCase(10, 0, -1)]
    [TestCase(2, 1, 2)]
    public void IndexOf_Index_Count_Valid(int value, int start, int index)
    {
        var list = new FastList<int>([0, 1, 2, 3, 4]);
        int i = list.IndexOf(value, start, 3);
        Assert.That(i, Is.EqualTo(index));
    }

    [TestCase(1, 0)]
    [TestCase(3, 2)]
    [TestCase(10, -1)]
    public void LastIndexOf_Valid(int value, int expected)
    {
        var list = new FastList<int>([1, 2, 3]);
        int index = list.LastIndexOf(value);
        Assert.That(index, Is.EqualTo(expected));
    }

    [TestCase(10, 4, -1)]
    [TestCase(2, 3, 2)]
    public void LastIndexOf_Index_Valid(int value, int start, int index)
    {
        var list = new FastList<int>([0, 1, 2, 3, 4]);
        int i = list.LastIndexOf(value, start);
        Assert.That(i, Is.EqualTo(index));
    }

    [TestCase(10, 4, -1)]
    [TestCase(2, 3, 2)]
    public void LastIndexOf_Index_Count_Valid(int value, int start, int index)
    {
        var list = new FastList<int>([0, 1, 2, 3, 4]);
        int i = list.LastIndexOf(value, start, 3);
        Assert.That(i, Is.EqualTo(index));
    }

    [Test]
    public void LastIndexOf_Empty_Valid()
    {
        var list = new FastList<int>();
        int index = list.LastIndexOf(0);
        Assert.That(index, Is.EqualTo(-1));
    }

    [Test]
    public void LastIndexOf_Index_Emmpty_Valid()
    {
        var list = new FastList<int>();
        int i = list.LastIndexOf(0, 0);
        Assert.That(i, Is.EqualTo(-1));
    }

    [Test]
    public void LastIndexOf_Index_Count_Empty_Valid()
    {
        var list = new FastList<int>();
        int i = list.LastIndexOf(0, 0, 0);
        Assert.That(i, Is.EqualTo(-1));
    }

    [Test]
    public void Swap_Valid()
    {
        var list1 = new FastList<int>([1, 2, 3]);
        var list2 = new FastList<int>([4, 5]);

        list1.Swap(list2);

        Assert.That(list1, Is.EquivalentTo([4, 5]));
        Assert.That(list2, Is.EquivalentTo([1, 2, 3]));
    }

    [Test]
    public void Enumerator_Reset_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        IEnumerator<int> enumerator = list.GetEnumerator();

        enumerator.MoveNext();
        enumerator.MoveNext();
        enumerator.Reset();
        Assert.That(enumerator.Current, Is.Zero);
        enumerator.MoveNext();
        Assert.That(enumerator.Current, Is.EqualTo(1));
    }

    [Test]
    public void AddAll_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        var list2 = new FastList<int>([4, 5]);
        list.AddAll(list2);
        Assert.That(list, Is.EquivalentTo([1, 2, 3, 4, 5]));
    }

    [Test]
    public void TrimExcess_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        list.Capacity = 10;

        list.TrimExcess();
        Assert.That(list.Capacity, Is.EqualTo(3));
        Assert.That(list, Is.EquivalentTo([1, 2, 3]));
    }

    [Test]
    public void TrimExcess_Empty_Valid()
    {
        var list = new FastList<int>();
        list.Capacity = 10;

        list.TrimExcess();
        Assert.That(list.Capacity, Is.EqualTo(0));
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void TrimExcess_Exact_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        list.Capacity = 3;

        list.TrimExcess();
        Assert.That(list.Capacity, Is.EqualTo(3));
        Assert.That(list, Is.EquivalentTo([1, 2, 3]));
    }

    [Test]
    public void TrueForAll_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);

        bool r = list.TrueForAll(t => t >= 1);
        Assert.That(r, Is.True);
    }

    [Test]
    public void NotTrueForAll_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);

        bool r = list.TrueForAll(t => t < 1);
        Assert.That(r, Is.False);
    }

    [Test]
    public void InsertRange_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        list.InsertRange(2, new List<int>([4, 5]));
        Assert.That(list, Is.EquivalentTo([1, 2, 4, 5, 3]));
    }

    [Test]
    public void InsertRange_Self_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        list.InsertRange(3, list);
        Assert.That(list, Is.EquivalentTo([1, 2, 3, 1, 2, 3]));
    }

    [Test]
    public void InsertRange_IEnumerable_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        list.InsertRange(2, [4, 5]);
        Assert.That(list, Is.EquivalentTo([1, 2, 4, 5, 3]));
    }

    [Test]
    public void RemoveAll_Empty_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        list.RemoveAll(t => t > 10);
        Assert.That(list, Is.EquivalentTo([1, 2, 3, 4, 5]));
    }

    [Test]
    public void RemoveAll_After_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        list.RemoveAll(t => t > 3);
        Assert.That(list, Is.EquivalentTo([1, 2, 3]));
    }

    [Test]
    public void RemoveAll_Between_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        list.RemoveAll(t => t > 3 && t < 5);
        Assert.That(list, Is.EquivalentTo([1, 2, 3, 5]));
    }

    [Test]
    public void RemoveRange_Empty_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        list.RemoveRange(0, 0);
        Assert.That(list, Is.EquivalentTo([1, 2, 3, 4, 5]));
    }

    [Test]
    public void RemoveRange_All_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        list.RemoveRange(1, 4);
        Assert.That(list, Is.EquivalentTo([1]));
    }

    [Test]
    public void RemoveRange_Between_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        list.RemoveRange(1, 3);
        Assert.That(list, Is.EquivalentTo([1, 5]));
    }

    [Test]
    public void Reverse_Valid()
    {
        var list = new FastList<int>([1, 2, 3]);
        list.Reverse();
        Assert.That(list, Is.EquivalentTo([3, 2, 1]));
    }

    [Test]
    public void Reverse_Index_Count_Valid()
    {
        var list = new FastList<int>([1, 2, 3, 4, 5]);
        list.Reverse(1, 3);
        Assert.That(list, Is.EquivalentTo([1, 4, 3, 2, 5]));
    }

    [Test]
    public void Sort_Valid()
    {
        var list = new FastList<int>([2, 1, 5, 3, 4]);
        list.Sort();
        Assert.That(list, Is.EquivalentTo([1, 2, 3, 4, 5]));
    }

    [Test]
    public void Sort_Comparer_Valid()
    {
        var list = new FastList<int>([2, 1, 5, 3, 4]);
        Comparer<int> comparer = Comparer<int>.Create((a, b) => -a.CompareTo(b));
        list.Sort(comparer);
        Assert.That(list, Is.EquivalentTo([5, 4, 3, 2, 1]));
    }

    [Test]
    public void Sort_Index_Count_Comparer_Valid()
    {
        var list = new FastList<int>([2, 1, 5, 3, 4]);
        Comparer<int> comparer = Comparer<int>.Create((a, b) => -a.CompareTo(b));
        list.Sort(1, 3, comparer);
        Assert.That(list, Is.EquivalentTo([2, 5, 3, 1, 4]));
    }
}
