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
using System.Windows.Shapes;

namespace Workitem10053;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
//old issue: [ExampleBrowser.Example("Issue 10053", "SharpDX: Enable touch / implement CameraController.")]
[ExampleBrowser.Example("Issue 1074-2", "ManipulationBindings: Pan-Rotate, TwoFingerPan-Pan, Pinch-Zoom.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Closed += (s, e) => (DataContext as IDisposable)?.Dispose();
    }
}
