// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for the main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
namespace Workitem73
{
    /// <summary>
    /// Interaction logic for the main window.
    /// </summary>
    [ExampleBrowser.Example("Work item 73", "TubeVisual3D")]
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            var path1 = new List<Point3D>() { new Point3D(1,1,0.1), new Point3D(1,1,-0.2), new Point3D(1,1,0.0001) };
            var path2 = new List<Point3D>() { new Point3D(2,2,0.1), new Point3D(2,2,-0.2), new Point3D(2,2,0.1) };

            AddTube(path1, Colors.Green);
            AddTube(path2, Colors.Red);
        }


        void AddTube(List<Point3D> path, Color color)
        {
            var mb = new MeshBuilder();

            mb.AddTube(path, 0.1, 3, false);
            var geom = new GeometryModel3D { Geometry = mb.ToMesh(true), Material = MaterialHelper.CreateMaterial(color) };          // create a model
            var model = new ModelVisual3D();
            model.Content = geom;
            _Helix.Children.Add(model);
        }
    }
}