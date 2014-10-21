// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ExtrudedTextDemo
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Demonstrates extruding text.")]
    public partial class MainWindow : Window
    {
        private MeshGeometry3D textGeometry;

        public MainWindow()
        {
            this.InitializeComponent();
            var builder = new MeshBuilder(false, false);
            builder.ExtrudeText(
                "Helix Toolkit",
                "Arial",
                FontStyles.Normal,
                FontWeights.Bold,
                20,
                new Vector3D(1, 0, 0),
                new Point3D(0, 0, 0),
                new Point3D(0, 0, 1));

            this.textGeometry = builder.ToMesh(true);
            this.DataContext = this;
        }

        public MeshGeometry3D TextGeometry
        {
            get
            {
                return this.textGeometry;
            }
        }
    }
}