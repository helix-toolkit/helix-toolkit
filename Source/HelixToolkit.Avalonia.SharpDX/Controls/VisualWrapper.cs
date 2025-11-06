using Avalonia;
using Avalonia.Metadata;

namespace HelixToolkit.Avalonia.SharpDX;

/// <summary>
///     The VisualWrapper simply integrates a raw Visual child into a tree
///     of FrameworkElements.
///      https://blogs.msdn.microsoft.com/dwayneneed/2007/04/26/multithreaded-ui-hostvisual/
/// </summary>
public class VisualWrapper<T> : FrameworkContentElement where T : Visual
{
    [Content]
    public T? Child
    {
        get
        {
            return _child;
        }

        set
        {
            if (_child != null)
            {
                VisualChildren.Remove(_child);
            }

            _child = value;

            if (_child != null)
            {
                VisualChildren.Add(_child);
            }
        }
    }

    protected Visual GetVisualChild(int index)
    {
        if (_child != null && index == 0)
        {
            return _child;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    protected int VisualChildrenCount
    {
        get
        {
            return _child != null ? 1 : 0;
        }
    }

    private T? _child;
}

/// <summary>
///     The VisualWrapper simply integrates a raw Visual child into a tree
///     of FrameworkElements.
/// </summary>
public class VisualWrapper : VisualWrapper<Visual>
{
}
