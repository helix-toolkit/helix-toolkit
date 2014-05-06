// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RubikDemo
{
    using System.Windows;
    using System.Windows.Input;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Rubik's cube visualized.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomExtents();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            this.info.Text = string.Empty;
            if ((e.Key == Key.Back || e.Key == Key.Escape) && cube1.CanUnscramble())
            {
                cube1.Unscramble();
                e.Handled = true;
            }

            if (e.Key == Key.Space)
            {
                cube1.Scramble();
                e.Handled = true;
            }

            if (e.Key == Key.Add)
            {
                cube1.Size++;
                e.Handled = true;
            }

            if (e.Key == Key.Subtract && cube1.Size > 2)
            {
                cube1.Size--;
                e.Handled = true;
            }

            cube1.Rotate(e.Key);
        }
    }
}