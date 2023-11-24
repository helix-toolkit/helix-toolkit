namespace HelixToolkit;

/// <summary>
/// Represents two keys and a value.
/// </summary>
/// <typeparam name="K">
/// First key type.
/// </typeparam>
/// <typeparam name="T">
/// Second key type.
/// </typeparam>
/// <typeparam name="V">
/// Value type.
/// </typeparam>
public sealed class DoubleKeyPairValue<K, T, V>
    where K : notnull
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleKeyPairValue{K,T,V}"/> class.
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
    public DoubleKeyPairValue(K key1, T key2, V value)
    {
        Key1 = key1;
        Key2 = key2;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the key1.
    /// </summary>
    /// <value>The key1.</value>
    public K Key1
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the key2.
    /// </summary>
    /// <value>The key2.</value>
    public T Key2
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public V Value
    {
        get; set;
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return Key1 + " - " + Key2 + " - " + Value;
    }
}
