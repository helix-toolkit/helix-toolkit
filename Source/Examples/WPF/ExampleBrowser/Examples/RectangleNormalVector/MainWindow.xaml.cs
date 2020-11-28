// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using ExampleBrowser;

namespace RectangleNormalVector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Test of rectangle3D orientations defined by normal vectors. Just use the sliders to manipulate the normal vector.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }
    }
}