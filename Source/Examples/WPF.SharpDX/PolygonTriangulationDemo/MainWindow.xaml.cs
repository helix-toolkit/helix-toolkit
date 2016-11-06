using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Linq;

namespace PolygonTriangulationDemo
{
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using System.Windows.Media;
    using System;
    using DemoCore;
    using System.Windows.Media.Media3D;
    using System.Globalization;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// List of Polygon Points to display
        /// </summary>
        List<Vector2> mPolygonPoints;
        /// <summary>
        /// The ViewModel
        /// </summary>
        MainViewModel mViewModel;
        /// <summary>
        /// Constructor for the MainWindow
        /// </summary>
        public MainWindow()
        {
            // Initialize all Components
            InitializeComponent();
            
            // Setup the ViewModel
            mViewModel = new MainViewModel();
            this.DataContext = mViewModel;

            // Setup the Camera and raise the EventHandler once at the Start
            mViewModel.Camera.Changed += Camera_Changed;
            Camera_Changed(mViewModel.Camera, new EventArgs());
            
            // Setup the Line Drawing Handler
            mViewModel.PropertyChanged += ((s, e) =>
            {
                // Switch the Line Geometry
                if (e.PropertyName == "ShowTriangleLines")
                {
                    if (mViewModel.ShowTriangleLines)
                    {
                        lineTriangulatedPolygon.Geometry = mViewModel.LineGeometry;
                    }
                    else
                    {
                        lineTriangulatedPolygon.Geometry = null;
                    }
                }
            }); 
        }

        /// <summary>
        /// On Camera change, display Position and Direction Info
        /// </summary>
        /// <param name="sender">The Sender (i.e. the Camera)</param>
        /// <param name="e">The Event Args</param>
        void Camera_Changed(object sender, EventArgs e)
        {
            var cam = (HelixToolkit.Wpf.SharpDX.PerspectiveCamera)sender;
            var pos = String.Format("X: {0:0.###}, ", cam.Position.X) +
                      String.Format("Y: {0:0.###}, ", cam.Position.Y) +
                      String.Format("Z: {0:0.###}", cam.Position.Z);
            var dir = String.Format("X: {0:0.###}, ", cam.LookDirection.X) +
                      String.Format("Y: {0:0.###}, ", cam.LookDirection.Y) +
                      String.Format("Z: {0:0.###}", cam.LookDirection.Z); 
            // Set the Label Content
            statusLabel.Content = "View from " + pos + ", view direction " + dir;
        }

        /// <summary>
        /// Generate a simple Polygon and then triangulate it.
        /// The Result is then Displayed.
        /// </summary>
        /// <param name="sender">The Sender (i.e. the Button)</param>
        /// <param name="e">The routet Event Args</param>
        private void generatePolygonButton_Click(object sender, RoutedEventArgs e)
        {
            // Generate random Polygon
            var random = new Random();
            var cnt = mViewModel.PointCount;
            mPolygonPoints = new List<Vector2>();
            var angle = 0f;
            var angleDiff = 2f * (Single)Math.PI / cnt;
            var radius = 4f;
            // Random Radii for the Polygon
            var radii = new List<float>();
            for (int i = 0; i < cnt; i++)
            {
                radii.Add(radius + random.NextFloat(-radius, radius));
            }
            for (int i = 0; i < cnt; i++)
            {
                var last = i > 0 ? i - 1 : cnt - 1;
                var next = i < cnt - 1 ? i + 1 : 0;
                // Flatten a bit
                var radiusUse = radii[last] * 0.25f + radii[i] * 0.5f + radii[next] * 0.25f;
                mPolygonPoints.Add(new Vector2(radiusUse * (Single)Math.Cos(angle), radiusUse * (Single)Math.Sin(angle)));
                angle += angleDiff;
            }
            // Triangulate and measure the Time needed for the Triangulation
            var before = DateTime.Now;
            var sLTI = SweepLinePolygonTriangulator.Triangulate(mPolygonPoints);
            var after = DateTime.Now;
            
            // Generate the Output
            var geometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
            geometry.Positions = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
            geometry.Normals = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
            foreach (var point in mPolygonPoints)
            {
                geometry.Positions.Add(new Vector3(point.X, 0, point.Y + 5));
                geometry.Normals.Add(new Vector3(0, 1, 0));
            }
            geometry.Indices = new HelixToolkit.Wpf.SharpDX.Core.IntCollection(sLTI);
            triangulatedPolygon.Geometry = geometry;

            var lb = new LineBuilder();
            for (int i = 0; i < sLTI.Count; i += 3)
            {
                lb.AddLine(geometry.Positions[sLTI[i]], geometry.Positions[sLTI[i + 1]]);
                lb.AddLine(geometry.Positions[sLTI[i + 1]], geometry.Positions[sLTI[i + 2]]);
                lb.AddLine(geometry.Positions[sLTI[i + 2]], geometry.Positions[sLTI[i]]);
            }
            mViewModel.LineGeometry  = lb.ToLineGeometry3D();
            
            // Set the Lines if activated
            if (mViewModel.ShowTriangleLines)
            {
                lineTriangulatedPolygon.Geometry = mViewModel.LineGeometry;
            }
            else
            {
                lineTriangulatedPolygon.Geometry = null;
            }
            
            // Set the InfoLabel Text
            var timeNeeded = (after - before).TotalMilliseconds;
            infoLabel.Content = String.Format("Last triangulation of {0} Points took {1:0.##} Milliseconds!", triangulatedPolygon.Geometry.Positions.Count, timeNeeded);
        }

        /// <summary>
        /// Handle the selection Change of the Material
        /// </summary>
        /// <param name="sender">Sender (i.e. the Combobox)</param>
        /// <param name="e">Event Args</param>
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Set the ViewModel's Material and the Polygon-Material
            mViewModel.Material = PhongMaterials.GetMaterial(e.AddedItems[0].ToString());
        }
    }
}