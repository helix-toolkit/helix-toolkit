using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf.SharpDX;

namespace TriangleSelection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void viewport_MouseDown3D(object sender, RoutedEventArgs e)
        {     
            if (e is MouseDown3DEventArgs args && args.OriginalInputEventArgs is MouseButtonEventArgs inputArgs && inputArgs.LeftButton == MouseButtonState.Pressed
                && args.HitTestResult is not null && DataContext is MainViewModel vm)
            {
                vm.HandleMouseDown(args.HitTestResult);
            }
        }
    }
}