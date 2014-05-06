// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TerrainDemo
{
    using System.Windows;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Rendering a terrain loaded from a .bt file.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.MainWindowLoaded;
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomExtents();
        }
    }
}