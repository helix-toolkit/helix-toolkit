using Avalonia.Input;
using Avalonia.Interactivity;

namespace HelixToolkit.Avalonia.SharpDX;

/// <summary>
/// Provides the events for Viewport3DX.
/// </summary>
public partial class Viewport3DX
{
    /// <summary>
    /// Provide CLR accessors for the event 
    /// </summary>
    public event EventHandler<PointerPressedEventArgs> MouseDown3D
    {
        add
        {
            this.AddHandler(GeometryModel3D.MouseDown3DEvent, value);
        }
        remove
        {
            this.RemoveHandler(GeometryModel3D.MouseDown3DEvent, value);
        }
    }

    /// <summary>
    /// Provide CLR accessors for the event 
    /// </summary>
    public event EventHandler<PointerPressedEventArgs> MouseUp3D
    {
        add
        {
            this.AddHandler(GeometryModel3D.MouseUp3DEvent, value);
        }
        remove
        {
            this.RemoveHandler(GeometryModel3D.MouseUp3DEvent, value);
        }
    }

    /// <summary>
    /// Provide CLR accessors for the event 
    /// </summary>
    public event EventHandler<PointerEventArgs> MouseMove3D
    {
        add
        {
            this.AddHandler(GeometryModel3D.MouseMove3DEvent, value);
        }
        remove
        {
            this.RemoveHandler(GeometryModel3D.MouseMove3DEvent, value);
        }
    }

    /// <summary>
    /// Event when a property has been changed
    /// </summary>
    public event EventHandler<RoutedEventArgs> CameraChanged
    {
        add
        {
            this.AddHandler(CameraChangedEvent, value);
        }

        remove
        {
            this.RemoveHandler(CameraChangedEvent, value);
        }
    }

    /// <summary>
    /// The camera changed event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> CameraChangedEvent =
        RoutedEvent.Register<Viewport3DX, RoutedEventArgs>("CameraChanged", RoutingStrategies.Bubble);
}
