namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public sealed class BoolArgs : EventArgs
{
    /// <summary>
    /// The true arguments
    /// </summary>
    public static readonly BoolArgs TrueArgs = new(true);
    /// <summary>
    /// The false arguments
    /// </summary>
    public static readonly BoolArgs FalseArgs = new(false);
    /// <summary>
    /// The value
    /// </summary>
    public readonly bool Value;
    /// <summary>
    /// Initializes a new instance of the <see cref="BoolArgs"/> class.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public BoolArgs(bool value)
    {
        Value = value;
    }
}
