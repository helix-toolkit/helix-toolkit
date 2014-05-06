// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SurfacePlotDemo
{
    using System.Windows;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Plotting a surface in 3D.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}