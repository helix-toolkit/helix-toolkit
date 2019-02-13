// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Workitem10053
{
    using System.Windows;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //old issue: [Example("Issue 10053", "SharpDX: Enable touch / implement CameraController.")]
    [Example("Issue 1074-2", "ManipulationBindings: Pan-Rotate, TwoFingerPan-Pan, Pinch-Zoom.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();                                  
        }
    }
}