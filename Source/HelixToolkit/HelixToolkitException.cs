namespace HelixToolkit;

/// <summary>
/// Represents errors that occurs in the Helix 3D Toolkit.
/// </summary>
[Serializable]
public class HelixToolkitException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelixToolkitException"/> class.
    /// </summary>
    /// <param name="formatString">
    /// The format string.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    public HelixToolkitException(string formatString, params object[] args)
        : base(string.Format(formatString, args))
    {
    }

    public static void Throw(string formatString, params object[] args)
    {
        throw new HelixToolkitException(formatString, args);
    }

    public static T Throw<T>(string formatString, params object[] args)
    {
        throw new HelixToolkitException(formatString, args);
    }
}
