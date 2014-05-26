namespace SurfacePlotCuttingPlanesDemo
{
    using System.Windows;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    [Example(null, "Applies cutting planes to a surface that utilises texture coordinates.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new SurfacePlotDemo.MainViewModel();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
