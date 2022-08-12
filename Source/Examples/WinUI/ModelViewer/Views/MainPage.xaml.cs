using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;

using ModelViewer.ViewModels;

namespace ModelViewer.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        DataContext = App.GetService<MainViewModel>();
    }
}
