// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RubikDemo
{
    using System.Text;
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

            if (e.Key == Key.H)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Keys:");
                sb.AppendLine("h/H = show info text");
                sb.AppendLine("l/L = rotate Left");
                sb.AppendLine("r/R = rotate Right");
                sb.AppendLine("u/U = rotate Up");
                sb.AppendLine("d/D = rotate Down");
                sb.AppendLine("b/B = rotate Back");
                sb.AppendLine("f/F = rotate Front");
                sb.AppendLine("Backspace/Escape = undo");
                sb.AppendLine("Space = do random move");
                sb.AppendLine("Numpad +/- = change size");

                info.Text = sb.ToString();

                e.Handled = true;
                return;
            }

            cube1.Rotate(e.Key);
        }
    }
}