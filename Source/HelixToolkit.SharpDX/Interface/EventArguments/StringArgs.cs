namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public sealed class StringArgs : EventArgs
{
    /// <summary>
    /// The value
    /// </summary>
    public readonly string Value;
    /// <summary>
    /// Initializes a new instance of the <see cref="StringArgs"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public StringArgs(string value)
    {
        Value = value;
    }
}
