﻿using SharpDX;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// 
/// </summary>
/// <seealso cref="System.EventArgs" />
public sealed class BoundChangedEventArgs : EventArgs
{
    /// <summary>
    /// The new bound
    /// </summary>
    public readonly BoundingBox NewBound;
    /// <summary>
    /// The old bound
    /// </summary>
    public readonly BoundingBox OldBound;
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundChangedEventArgs"/> class.
    /// </summary>
    /// <param name="newBound">The new bound.</param>
    /// <param name="oldBound">The old bound.</param>
    public BoundChangedEventArgs(BoundingBox newBound, BoundingBox oldBound)
    {
        NewBound = newBound;
        OldBound = oldBound;
    }
}
