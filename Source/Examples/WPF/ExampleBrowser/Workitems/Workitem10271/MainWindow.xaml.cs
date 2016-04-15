// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for the main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Windows;
using System;
using System.Linq;

namespace Workitem10271
{
    /// <summary>
    /// Interaction logic for the main window.
    /// </summary>
    [ExampleBrowser.Example("Work item 10271", "Quad Normals")]
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public MeshGeometry3D MbQuad
        {
            get { return (MeshGeometry3D)GetValue(MbQuadProperty); }
            set { SetValue(MbQuadProperty, value); }
        }

        public static readonly DependencyProperty MbQuadProperty = DependencyProperty.Register("MbQuad", typeof(MeshGeometry3D), typeof(MainWindow), new PropertyMetadata(buildQuad()));

        public MeshGeometry3D MbTri
        {
            get { return (MeshGeometry3D)GetValue(MbTriProperty); }
            set { SetValue(MbTriProperty, value); }
        }
        public static readonly DependencyProperty MbTriProperty = DependencyProperty.Register("MbTri", typeof(MeshGeometry3D), typeof(MainWindow), new PropertyMetadata(buildTri()));

        private static object buildTri()
        {
            var mb = new MeshBuilder(true, false);
            var p0 = new Point3D(0, 0, 0);
            var p1 = new Point3D(1, 0, 0);
            var p2 = new Point3D(1, 1, 0);
            mb.AddTriangle(p0, p1, p2);
            mb.Normals.ToList().ForEach(x => System.Diagnostics.Trace.WriteLine(x.ToString()));
            return mb.ToMesh();
        }

        private static object buildQuad()
        {
            var mb = new MeshBuilder(true, false);
            var p0 = new Point3D(0, 0, 0);
            var p1 = new Point3D(1, 0, 0);
            var p2 = new Point3D(1, 1, 0);
            var p3 = new Point3D(0, 1, 0);
            mb.AddQuad(p0, p1, p2, p3);
            mb.Normals.ToList().ForEach(x => System.Diagnostics.Trace.WriteLine(x.ToString()));
            return mb.ToMesh();
        }

    }
}