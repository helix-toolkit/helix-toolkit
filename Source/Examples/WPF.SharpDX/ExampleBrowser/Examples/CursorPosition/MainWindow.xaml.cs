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

namespace CursorPosition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example("CursorPosition", "Shows the position of the mouse cursor in the Viewport3DX.")]
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }
    }
}