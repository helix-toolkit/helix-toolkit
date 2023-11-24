using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.WinUI.SharpDX;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModelViewerDemo;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainViewModel MainVM
    {
        get;
    }

    public MainWindow()
    {
        this.InitializeComponent();

        Closed += (s, e) => (ContentArea.DataContext as IDisposable)?.Dispose();

        MainVM = (MainViewModel)ContentArea.DataContext;
        MainVM.Target = this;

        viewport.OnMouse3DDown += Viewport_OnMouse3DDown;
    }

    private void Viewport_OnMouse3DDown(object? sender, MouseDown3DEventArgs e)
    {
        if (e.HitTestResult == null)
        {
            return;
        }
        if (e.HitTestResult.ModelHit is SceneNode node && node.Tag is AttachedNodeViewModel vm)
        {
            vm.Selected = !vm.Selected;
        }
    }
}
