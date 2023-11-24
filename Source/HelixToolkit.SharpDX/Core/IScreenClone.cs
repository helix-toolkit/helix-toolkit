using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IScreenClone
{
    /// <summary>
    /// Gets or sets the output.
    /// </summary>
    /// <value>
    /// The output.
    /// </value>
    int Output
    {
        set; get;
    }

    /// <summary>
    /// Gets or sets the clone rectangle.
    /// </summary>
    /// <value>
    /// The clone rectangle.
    /// </value>
    Rectangle CloneRectangle
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether cloned rectangle is stretched during rendering, default is false;
    /// </summary>
    /// <value>
    ///   <c>true</c> if stretch; otherwise, <c>false</c>.
    /// </value>
    bool StretchToFill
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show mouse cursor].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show mouse cursor]; otherwise, <c>false</c>.
    /// </value>
    bool ShowMouseCursor
    {
        set; get;
    }
}
