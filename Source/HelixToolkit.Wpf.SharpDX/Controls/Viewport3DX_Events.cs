using HelixToolkit.Wpf.SharpDX.Controls;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX;

/// <summary>
/// Provides the events for Viewport3DX.
/// </summary>
public partial class Viewport3DX
{
    /// <summary>
    /// Provide CLR accessors for the event 
    /// </summary>
    public event RoutedEventHandler MouseDown3D
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
    public event RoutedEventHandler MouseUp3D
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
    public event RoutedEventHandler MouseMove3D
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
    /// Occurs when [form mouse move].
    /// </summary>
    public event WinformHostExtend.FormMouseMoveEventHandler FormMouseMove
    {
        add
        {
            this.AddHandler(WinformHostExtend.FormMouseMoveEvent, value);
        }
        remove
        {
            this.RemoveHandler(WinformHostExtend.FormMouseMoveEvent, value);
        }
    }
    /// <summary>
    /// Occurs when [form mouse wheel].
    /// </summary>
    public event WinformHostExtend.FormMouseWheelEventHandler FormMouseWheel
    {
        add
        {
            this.AddHandler(WinformHostExtend.FormMouseWheelEvent, value);
        }
        remove
        {
            this.RemoveHandler(WinformHostExtend.FormMouseWheelEvent, value);
        }
    }

    /// <summary>
    /// Event when a property has been changed
    /// </summary>
    public event RoutedEventHandler CameraChanged
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
    public static readonly RoutedEvent CameraChangedEvent = EventManager.RegisterRoutedEvent(
        "CameraChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Viewport3DX));
}
