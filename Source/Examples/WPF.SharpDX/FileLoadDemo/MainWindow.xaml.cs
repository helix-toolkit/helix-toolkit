// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileLoadDemo
{
    using System.Windows;
    using HelixToolkit.Wpf.SharpDX;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var Main = new MainViewModel();
             Main.modelView = this.view;
      this.DataContext = Main;
            this.MouseMove += MainWindow_MouseMove;
        }

        private void MainWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
           var hits= view.FindHits(e.GetPosition(view));
        }
    }
}