using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Utilities;
using System;
using System.Windows;

namespace Workitem10051;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private Exception? renderException;

    partial void OnRenderExceptionChanged(Exception? value)
    {
        this.ViewportMessage = this.RenderException is null ? string.Empty : this.RenderException.ToString();
    }

    [ObservableProperty]
    private string viewportMessage = string.Empty;

    public MainViewModel()
    {
        // titles
        this.Title = "Simple Demo (Workitem 10051)";
        this.SubTitle = "ManipulationBindings: TwoFingerPan-Rotate, Pan-Pan, Pinch-Zoom";
        // old issue: this.SubTitle = "LineGeometryModel3D now works with OrthographicCamera and Intel HD 3000.";

        this.EffectsManager = new DefaultEffectsManager();
    }

    /// <summary>
    /// Handles exceptions at the rendering subsystem.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments.</param>
    public void HandleRenderException(object? sender, RelayExceptionEventArgs e)
    {
        if (e.Exception is not null)
        {
            MessageBox.Show(e.Exception.ToString(), "RenderException");
        }
    }
}
