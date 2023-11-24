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

namespace Workitem1349;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Issue 1349", "SharpDX: BillboardText3D is not shown until origin is moved into sight.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Closed += (s, e) => (DataContext as IDisposable)?.Dispose();
    }
}
