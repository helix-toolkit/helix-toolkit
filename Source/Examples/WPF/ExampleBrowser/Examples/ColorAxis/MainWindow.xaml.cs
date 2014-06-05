namespace ColorAxisDemo
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Displays color axes (defined by gradient brushes) over the 3D view.")]
    public partial class MainWindow : Window
    {
        public Brush ColorScheme { get; set; }
        public Brush ColorScheme2 { get; set; }

        public IList<string> Categories { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.ColorScheme = BrushHelper.CreateRainbowBrush(false);
            this.Categories = new[] { "Asia", "Africa", "North America", "South America", "Antarctica", "Europe", "Australia" };
            this.ColorScheme2 = BrushHelper.CreateSteppedGradientBrush(new[] { Colors.Yellow, Colors.Brown, Colors.Red, Colors.Green, Colors.White, Colors.Orange, Colors.Blue },false);
        }
    }
}
