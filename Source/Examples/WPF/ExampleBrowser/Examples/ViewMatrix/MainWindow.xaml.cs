// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows;

using HelixToolkit.Wpf;

namespace ViewMatrixDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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