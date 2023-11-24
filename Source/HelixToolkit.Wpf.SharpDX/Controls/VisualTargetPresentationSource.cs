using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;

namespace HelixToolkit.Wpf.SharpDX;

/// <summary>
///     The VisualTargetPresentationSource represents the root
///     of a visual subtree owned by a different thread that the
///     visual tree in which is is displayed.
///     https://blogs.msdn.microsoft.com/dwayneneed/2007/04/26/multithreaded-ui-hostvisual/
/// </summary>
/// <remarks>
///     A HostVisual belongs to the same UI thread that owns the
///     visual tree in which it resides.
///     
///     A HostVisual can reference a VisualTarget owned by another
///     thread.
///     
///     A VisualTarget has a root visual.
///     
///     VisualTargetPresentationSource wraps the VisualTarget and
///     enables basic functionality like Loaded, which depends on
///     a PresentationSource being available.
/// </remarks>
public class VisualTargetPresentationSource : PresentationSource, IDisposable
{
    public VisualTargetPresentationSource(HostVisual hostVisual)
    {
        _visualTarget = new VisualTarget(hostVisual);
    }

    public override Visual RootVisual
    {
        get
        {
            return _visualTarget.RootVisual;
        }

        set
        {
            var oldRoot = _visualTarget.RootVisual;


            // Set the root visual of the VisualTarget.  This visual will
            // now be used to visually compose the scene.
            _visualTarget.RootVisual = value;

            // Hook the SizeChanged event on framework elements for all
            // future changed to the layout size of our root, and manually
            // trigger a size change.
            if (value is FrameworkElement rootFE)
            {
                rootFE.SizeChanged += new SizeChangedEventHandler(root_SizeChanged);
                rootFE.DataContext = _dataContext;

                // HACK!
                if (_propertyName != null)
                {
                    var myBinding = new Binding(_propertyName)
                    {
                        Source = _dataContext
                    };
                    rootFE.SetBinding(TextBlock.TextProperty, myBinding);
                }
            }

            // Tell the PresentationSource that the root visual has
            // changed.  This kicks off a bunch of stuff like the
            // Loaded event.
            RootChanged(oldRoot, value);

            // Kickoff layout...
            if (value is UIElement rootElement)
            {
                rootElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                rootElement.Arrange(new Rect(rootElement.DesiredSize));
            }
        }
    }

    public object? DataContext
    {
        get
        {
            return _dataContext;
        }
        set
        {
            _dataContext = value;
            if (_visualTarget.RootVisual is FrameworkElement rootElement)
            {
                rootElement.DataContext = _dataContext;
            }
        }
    }

    // HACK!
    public string? PropertyName
    {
        get
        {
            return _propertyName;
        }
        set
        {
            _propertyName = value;

            if (_visualTarget.RootVisual is TextBlock rootElement)
            {
                if (!rootElement.CheckAccess())
                {
                    throw new InvalidOperationException("What?");
                }

                var myBinding = new Binding(_propertyName)
                {
                    Source = _dataContext
                };
                rootElement.SetBinding(TextBlock.TextProperty, myBinding);
            }
        }
    }

    public event SizeChangedEventHandler? SizeChanged;

    public override bool IsDisposed
    {
        get
        {
            // We don't support disposing this object.
            return false;
        }
    }

    protected override CompositionTarget GetCompositionTargetCore()
    {
        return _visualTarget;
    }

    private void root_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        SizeChanged?.Invoke(this, e);
    }

    private VisualTarget _visualTarget;
    private object? _dataContext;
    private string? _propertyName;

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls
    [SuppressMessage("Microsoft.Usage", "CA2213", Justification = "False positive.")]
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _visualTarget?.Dispose();
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~VisualTargetPresentationSource() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}
