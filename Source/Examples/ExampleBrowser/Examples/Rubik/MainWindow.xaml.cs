// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Input;

namespace RubikDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomExtents();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            info.Text = "";
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