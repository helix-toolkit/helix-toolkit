// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2016 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TorusDemo
{
    using System;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Shows three torus objects using the TorusVisual3D.")]
    public partial class MainWindow : Window
    {
        public Transform3D Transform1 = new TranslateTransform3D(new Vector3D(-4, 0, 0));
        public Transform3D Transform3 = new TranslateTransform3D(new Vector3D(4, 0, 0));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            torus1.Transform = Transform1;
            torus3.Transform = Transform3;
            viewPort.Camera.Position = new Point3D(0, 0, 10);
        }
    }
}