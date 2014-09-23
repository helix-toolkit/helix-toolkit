// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Workitem10048
{
    using System.Windows;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example("Issue 10048 and 10052", "SharpDX: Implement hit testing for lines.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();                                  
        }
    }
}