using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileLoadDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.DataContext = new MainViewModel(this);

        view.AddHandler(Element3D.MouseDown3DEvent, new RoutedEventHandler((s, e) =>
        {
            var arg = e as MouseDown3DEventArgs;

            if (arg?.HitTestResult is null)
            {
                return;
            }

            if (arg.HitTestResult.ModelHit is SceneNode node && node.Tag is AttachedNodeViewModel vm)
            {
                vm.Selected = !vm.Selected;
            }
        }));

        Closed += (s, e) => (DataContext as IDisposable)?.Dispose();
    }
}
