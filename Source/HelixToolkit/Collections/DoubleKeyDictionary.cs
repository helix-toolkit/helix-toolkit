using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit;

/// <summary>
/// A double key dictionary.
/// </summary>
/// <typeparam name="K">
/// The first key type.
/// </typeparam>
/// <typeparam name="T">
/// The second key type.
/// </typeparam>
/// <typeparam name="V">
/// The value type.
/// </typeparam>
/// <remarks>
/// See http://noocyte.wordpress.com/2008/02/18/double-key-dictionary/
/// A Remove method was added.
/// </remarks>
public class DoubleKeyDictionary<K, T, V> : IEnumerable<DoubleKeyPairValue<K, T, V>>,
                                            IEquatable<DoubleKeyDictionary<K, T, V>>
    where K : notnull
    where T : notnull
{
    /// <summary>
    /// Gets or sets OuterDictionary.
    /// </summary>
    private Dictionary<K, Dictionary<T, V>> OuterDictionary { get; } = new();

    /// <summary>
    /// Gets or sets the value with the specified indices.
    /// </summary>
    /// <value></value>
    public V this[K index1, T index2]
    {
        get
        {
            return OuterDictionary[index1][index2];
        }

        set
        {
            Add(index1, index2, value);
        }
    }

    /// <summary>
    /// Clears this dictionary.
    /// </summary>
    public void Clear()
    {
        foreach (var dict in OuterDictionary.Values)
        {
            dict?.Clear();
        }

        OuterDictionary.Clear();
    }

    /// <summary>
    /// Adds the specified key.
    /// </summary>
    /// <param name="key1">
    /// The key1.
    /// </param>
    /// <param name="key2">
    /// The key2.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void Add(K key1, T key2, V value)
    {

        if (OuterDictionary.TryGetValue(key1, out Dictionary<T, V>? inner))
        {
            if (inner.ContainsKey(key2))
            {
                inner[key2] = value;
            }
            else
            {
                inner.Add(key2, value);
            }
        }
        else
        {
            inner = new Dictionary<T, V>
            {
                { key2, value }
            };

            OuterDictionary.Add(key1, inner);
        }
    }

    /// <summary>
    /// Determines whether the specified dictionary contains the key.
    /// </summary>
    /// <param name="index1">
    /// The index1.
    /// </param>
    /// <param name="index2">
    /// The index2.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified index1 contains key; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsKey(K index1, T index2)
    {
        if (!OuterDictionary.ContainsKey(index1))
        {
            return false;
        }

        if (!OuterDictionary[index1].ContainsKey(index2))
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as DoubleKeyDictionary<K, T, V>);
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">
    /// The other.
    /// </param>
    /// <returns>
    /// The equals.
    /// </returns>
    public bool Equals(DoubleKeyDictionary<K, T, V>? other)
    {
        if (OuterDictionary.Keys.Count != other?.OuterDictionary.Keys.Count)
        {
            return false;
        }

        var isEqual = true;

        foreach (var innerItems in OuterDictionary)
        {
            if (!other.OuterDictionary.ContainsKey(innerItems.Key))
            {
                isEqual = false;
            }

            if (!isEqual)
            {
                break;
            }

            // here we can be sure that the key is in both lists,
            // but we need to check the contents of the inner dictionary
            var otherInnerDictionary = other.OuterDictionary[innerItems.Key];
            foreach (var innerValue in innerItems.Value)
            {
                if (!otherInnerDictionary.ContainsValue(innerValue.Value))
                {
                    isEqual = false;
                }

                if (!otherInnerDictionary.ContainsKey(innerValue.Key))
                {
                    isEqual = false;
                }
            }

            if (!isEqual)
            {
                break;
            }
        }

        return isEqual;
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// </returns>
    public IEnumerator<DoubleKeyPairValue<K, T, V>> GetEnumerator()
    {
        foreach (var outer in OuterDictionary)
        {
            foreach (var inner in outer.Value)
            {
                yield return new DoubleKeyPairValue<K, T, V>(outer.Key, inner.Key, inner.Value);
            }
        }
    }

    /// <summary>
    /// Removes the specified key.
    /// </summary>
    /// <param name="key1">
    /// The key1.
    /// </param>
    /// <param name="key2">
    /// The key2.
    /// </param>
    public void Remove(K key1, T key2)
    {
        OuterDictionary[key1].Remove(key2);
        if (OuterDictionary[key1].Count == 0)
        {
            OuterDictionary.Remove(key1);
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool TryGetValue(K key1, T key2, [MaybeNullWhen(false)] out V obj)
    {
        if (OuterDictionary.TryGetValue(key1, out Dictionary<T, V>? inner) && inner.TryGetValue(key2, out obj))
        {
            return true;
        }

        obj = default;
        return false;
    }

#if NET6_0_OR_GREATER
    public override int GetHashCode()
    {
        return HashCode.Combine(OuterDictionary);
    }
#else
    public override int GetHashCode()
    {
        return -377771656 + EqualityComparer<Dictionary<K, Dictionary<T, V>>>.Default.GetHashCode(OuterDictionary);
    }
#endif

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <value>
    /// The values.
    /// </value>
    public IEnumerable<V> Values
    {
        get
        {
            foreach (var dict in OuterDictionary.Values)
            {
                if (dict != null)
                {
                    foreach (var item in dict.Values)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
