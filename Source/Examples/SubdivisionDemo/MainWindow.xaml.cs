// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.ComponentModel;

namespace SubdivisionDemo
{
    using System.IO;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Subdivision_surface
    /// http://en.wikipedia.org/wiki/Loop_subdivision_surface
    /// http://research.microsoft.com/en-us/um/people/cloop/
    /// http://en.wikipedia.org/wiki/Barycentric_subdivision
    /// http://www.subdivision.org/
    /// </remarks>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region PropertyChanged Block
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion

        OffReader off;

        int levels;
        public int Levels
        {
            get { return levels; }
            set
            {
                if (value == levels)
                    return;
                levels = value; RaisePropertyChanged("Levels"); UpdateModel();
            }
        }

        bool barycentricTriangulation;
        public bool BarycentricTriangulation { get { return barycentricTriangulation; } set { barycentricTriangulation = value; RaisePropertyChanged("BarycentricTriangulation"); UpdateModel(); } }

        SubdivisionScheme subdivisionScheme;
        public SubdivisionScheme SubdivisionScheme { get { return subdivisionScheme; } set { subdivisionScheme = value; RaisePropertyChanged("SubdivisionScheme"); UpdateModel(); } }

        bool viewEdges;
        public bool ViewEdges { get { return viewEdges; } set { viewEdges = value; RaisePropertyChanged("ViewEdges"); UpdateModel(); } }
        bool viewVertices;
        public bool ViewVertices { get { return viewVertices; } set { viewVertices = value; RaisePropertyChanged("ViewVertices"); UpdateModel(); } }

        public MainWindow()
        {
            InitializeComponent();
            Levels = 1;
            ViewEdges = true;
            DataContext = this;
            Loaded += this.MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Load("Models\\cubix.off");
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            var d = new OpenFileDialog() { Filter = "OFF files (*.off)|*.off", InitialDirectory = Directory.GetCurrentDirectory() };
            if (d.ShowDialog().Value)
            {
                Load(d.FileName);
            }
        }

        public void Load(string fileName)
        {

            this.off = new OffReader();
            this.off.Load(fileName);
            Levels = 1;
            UpdateModel();
            view1.ZoomExtents();
        }

        public void UpdateModel()
        {
            if (this.off == null)
                return;

            this.Cursor = Cursors.Wait;

            double edge = 0.004;
            double vertex = 0.006;
            var w0 = new Stopwatch();
            w0.Start();

            // The original mesh
            var originalMesh = this.off.CreateMesh();
            var bounds = originalMesh.GetBounds();
            var l = Math.Max(bounds.Size.X, Math.Max(bounds.Size.Y, bounds.Size.Z));

            model1.Mesh = null;
            model2.Mesh = null;
            model3.Mesh = null;

            model1.EdgeDiameter = l * edge;
            model1.VertexRadius = l * vertex;
            model2.EdgeDiameter = l * edge;
            model2.VertexRadius = l * vertex;
            model3.EdgeDiameter = l * edge;
            model3.VertexRadius = l * vertex;

            if (!ViewEdges)
            {
                model1.EdgeDiameter = 0;
                model2.EdgeDiameter = 0;
                model3.EdgeDiameter = 0;
            }
            if (!ViewVertices)
            {
                model1.VertexRadius = 0;
                model2.VertexRadius = 0;
                model3.VertexRadius = 0;
            }

            model1.Mesh = originalMesh;

            // Triangulated mesh
            var mesh2 = originalMesh.Clone() as Mesh3D;
            mesh2.Triangulate(BarycentricTriangulation);

            model2.Mesh = mesh2;
            model2.Transform = new TranslateTransform3D(bounds.SizeX * 1.2, 0, 0);

            var triangularMesh = mesh2.ToMeshGeometry3D();

            var loop = new LoopSubdivision(triangularMesh) { Scheme = this.SubdivisionScheme };
            var w = new Stopwatch();
            w.Start();
            loop.Subdivide(this.Levels);
            long subDivisionTime = w.ElapsedMilliseconds;
            var mesh3 = loop.ToMesh3D();          
            w.Restart();
            model3.Mesh = mesh3; // new Mesh3D(mesh3.Positions, mesh3.TriangleIndices);
            long meshGenerationTime = w.ElapsedMilliseconds;
            model3.Transform = new TranslateTransform3D(bounds.SizeX * 2.4, 0, 0);

            subdivisionStatus.Text = string.Format("Subdivision time: {0} ms, mesh generation time: {1} ms,  triangles: {2} (original mesh: {3})", subDivisionTime, meshGenerationTime, (mesh3.Faces.Count), (triangularMesh.TriangleIndices.Count / 3));

            this.Cursor = Cursors.Arrow;
        }
    }
}