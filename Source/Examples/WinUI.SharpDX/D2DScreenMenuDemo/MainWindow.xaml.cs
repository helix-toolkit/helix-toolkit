using Microsoft.UI.Xaml;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace D2DScreenMenuDemo;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.AppWindow.Resize(new(600, 600));

        Closed += (s, e) => (ContentArea.DataContext as IDisposable)?.Dispose();
    }
}
