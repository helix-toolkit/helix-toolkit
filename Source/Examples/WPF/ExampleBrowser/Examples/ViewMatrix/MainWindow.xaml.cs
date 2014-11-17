// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ViewMatrixDemo
{
    using System.Windows;

    using HelixToolkit.Wpf;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Visualization of view and projection matrices (under construction).")]
    public partial class MainWindow : Window
    {
        private MainViewModel vm;

        public MainWindow()
        {
            this.InitializeComponent();
            this.vm = new MainViewModel();
            this.DataContext = this.vm;
        }

        private void OnCameraChanged(object sender, RoutedEventArgs e)
        {
            var vp = (HelixViewport3D)sender;
            vm.ViewMatrix = vp.Viewport.GetViewMatrix();
            vm.ViewportMatrix = vp.Viewport.GetViewportTransform();
            vm.ProjectionMatrix = vp.Viewport.GetProjectionMatrix();
        }
    }
}