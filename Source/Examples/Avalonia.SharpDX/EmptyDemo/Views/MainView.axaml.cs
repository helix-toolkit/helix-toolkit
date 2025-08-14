using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;

namespace EmptyDemo.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();

        base.OnDetachedFromVisualTree(e);
    }

    private int _frameCount;

    private void OnRendered(object? sender, EventArgs e)
    {
        if (viewport.RenderHost is null)
        {
            return;
        }

        viewport.RenderHost.ClearColor = new(0.5f, 0.5f, 1.0f, 1);
        var now = _frameCount++ / 20.0f;
        var colorOff = (float)(Math.Sin(now) + 1) / 2;
        viewport.RenderHost.ClearColor = new(1 - colorOff, colorOff, 0.5f + colorOff / 2, 1);
    }
}
