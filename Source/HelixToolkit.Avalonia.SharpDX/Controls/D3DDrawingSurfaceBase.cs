using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.Avalonia.SharpDX.Controls;

internal class D3DDrawingSurfaceBase : Control, IRenderCanvas
{
    public IRenderHost? RenderHost { get; private set; }

    private double dpiScale = 1;

    public double DpiScale
    {
        get => dpiScale;

        set
        {
            dpiScale = value;

            if (RenderHost != null)
            {
                RenderHost.DpiScale = (float)value;
            }
        }
    }

    private bool enableDpiScale = true;

    public bool EnableDpiScale
    {
        get => enableDpiScale;

        set
        {
            enableDpiScale = value;

            if (RenderHost != null)
            {
                RenderHost.DpiScale = value ? (float)DpiScale : 1;
            }
        }
    }

    public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred = delegate { };

    public D3DDrawingSurfaceBase()
    {
        HorizontalAlignment = UIHorizontalAlignment.Stretch;
        VerticalAlignment = UIVerticalAlignment.Stretch;

        RenderHost = new D3DRenderHost(this);
        RenderHost.DpiScale = EnableDpiScale ? (float)DpiScale : 1;
        //RenderHost.StartRenderLoop += RenderHost_StartRenderLoop;
        //RenderHost.StopRenderLoop += RenderHost_StopRenderLoop;
        RenderHost.ExceptionOccurred += (s, e) => { HandleExceptionOccured(e.Exception); };
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        try
        {
            StartD3D();
        }
        catch (Exception ex)
        {
            // Exceptions in the Loaded event handler are silently swallowed by WPF.
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/9ed3d13d-0b9f-48ac-ae8d-daf0845c9e8f/bug-in-wpf-windowloaded-exception-handling?forum=wpf
            // http://stackoverflow.com/questions/19140593/wpf-exception-thrown-in-eventhandler-is-swallowed
            // tl;dr: M$ says it's "by design" and "working as indended" but may change in the future :).

            if (!HandleExceptionOccured(ex))
            {
                // todo: MessageBox
                //MessageBox.Show($"DPFCanvas: Error while starting rendering: {ex.Message} \n StackTrace: {ex.StackTrace}", "Error");
            }
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        EndD3D();
        base.OnDetachedFromVisualTree(e);
    }

    private DispatcherOperation? resizeOperation = null;

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        if (resizeOperation != null && resizeOperation.Status == DispatcherOperationStatus.Pending)
        {
            resizeOperation.Abort();
        }

        if (RenderHost is null)
        {
            return;
        }

        int width = (int)e.NewSize.Width;
        int height = (int)e.NewSize.Height;

        resizeOperation = Dispatcher.UIThread.InvokeAsync((Action)(() =>
        {
            if (IsLoaded)
            {
                try
                {
                    //RenderHost.Resize((int)ActualWidth, (int)ActualHeight);
                    RenderHost.Resize(width, height);
                }
                catch (Exception ex)
                {
                    if (!HandleExceptionOccured(ex))
                    {
                        // todo: MessageBox
                        //MessageBox.Show($"DPFCanvas: Error during rendering: {ex.Message} \n StackTrace: {ex.StackTrace}", "Error");
                    }
                }
            }
        }),
        DispatcherPriority.Background);
    }

    private void StartD3D()
    {
        //RenderHost?.StartD3D((int)ActualWidth, (int)ActualHeight);
        RenderHost?.StartD3D((int)Width, (int)Height);
    }

    private void EndD3D()
    {
        RenderHost?.EndD3D();
    }

    private bool HandleExceptionOccured(Exception exception)
    {
        EndD3D();

        if (exception is SharpDXException sdxException &&
            (sdxException.Descriptor == global::SharpDX.DXGI.ResultCode.DeviceRemoved ||
             sdxException.Descriptor == global::SharpDX.DXGI.ResultCode.DeviceReset))
        {
            // Try to recover from DeviceRemoved/DeviceReset
            StartD3D();
            return true;
        }
        else
        {
            var args = new RelayExceptionEventArgs(exception);
            ExceptionOccurred(this, args);
            return args.Handled;
        }
    }
}
