// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Workitem10051
{
    using System.Windows;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // old issue: [Example("Issue 10051", "SharpDX: Line shader issues.")]
    [Example("Issue 1074-1", "ManipulationBindings: TwoFingerPan-Rotate, Pan-Pan, Pinch-Zoom.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();                                  
        }
    }
}