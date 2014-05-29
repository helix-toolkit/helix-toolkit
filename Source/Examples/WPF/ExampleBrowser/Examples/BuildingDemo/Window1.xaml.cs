namespace BuildingDemo
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [Example("BuildingDemo", "Using MeshBuilder to create buildings.")]
    public partial class Window1 : Window
    {
        private ViewModel viewModel;

        public Window1()
        {
            this.InitializeComponent();
            this.DataContext = this.viewModel = new ViewModel();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var viewport = (HelixViewport3D)sender;
            var firstHit = viewport.Viewport.FindHits(e.GetPosition(viewport)).FirstOrDefault();
            if (firstHit != null)
            {
                this.viewModel.Select(firstHit.Visual);
            }
            else
            {
                this.viewModel.Select(null);
            }
        }
    }
}
