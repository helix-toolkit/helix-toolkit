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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Vector2> mPolygonPoints;
        System.Windows.Point start;
        System.Windows.Point origin;
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainViewModel();
            this.DataContext = viewModel;

            /*mPolygonPoints = new List<Vector2>(){
                new Vector2(5.2f, 1f),
                new Vector2(2.8f, 11.4f),
                new Vector2(6.7f, 5.6f),
                new Vector2(5.2f, 5.2f),
                new Vector2(13.2f, 1f),
                new Vector2(6.6f, 8.8f),
                new Vector2(11.2f, 9.8f),
                new Vector2(12.2f, 6.2f),
                new Vector2(9.6f, 8.2f),
                new Vector2(12.1f, 3.6f),
                new Vector2(14.2f, 5.7f),
                new Vector2(12.1f, 13.9f),
                new Vector2(6.7f, 10.3f),
                new Vector2(5.2f, 12.8f),
                new Vector2(9.2f, 13.9f),
                new Vector2(12.2f, 18.1f),
                new Vector2(10.6f, 21.7f),
                new Vector2(8.7f, 18.7f),
                new Vector2(6.2f, 20.7f),
                new Vector2(8.7f, 22.2f),
                new Vector2(2.7f, 21.6f),
                new Vector2(5.3f, 18f),
                new Vector2(7.7f, 17.6f),
                new Vector2(3.7f, 15.4f),
                new Vector2(3.2f, 19.6f),
                new Vector2(.8f, 8.8f),
            };*/

            /*mPolygonPoints = new List<Vector2>(){
                new Vector2(2, 3),
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(3, 2),
                new Vector2(5, 0),
                new Vector2(6, 1),
                new Vector2(4, 3),
                new Vector2(6, 5),
                new Vector2(5, 6),
                new Vector2(3, 4),
                new Vector2(1, 6),
                new Vector2(0, 5),
            };*/

            /*mPolygonPoints = new List<Vector2>(){
                new Vector2(0, 0),
                new Vector2(2, 0),
                new Vector2(5, 1),
                new Vector2(5, 0),
                new Vector2(8, 0),
                new Vector2(8, 3),
                new Vector2(10, 3),
                new Vector2(10, 0),
                new Vector2(12, 2),
                new Vector2(10, 5),
                new Vector2(6.5f, 5),
                new Vector2(3, 6.5f),
                new Vector2(1.5f, 5),
                new Vector2(5, 5),
                new Vector2(3, 3),
                new Vector2(1.5f, 2.5f),
                new Vector2(0, 8),
                new Vector2(0, 2.5f),
                new Vector2(1.5f, 1),
            };*/

            /*mPolygonPoints = new List<Vector2>(){
                new Vector2(0, 0),
                new Vector2(4, -1),
                new Vector2(5, -5),
                new Vector2(11, -3),
                new Vector2(12, -7),
                new Vector2(15, -4),
                new Vector2(12, 0),
                new Vector2(7, -2),
                new Vector2(6, 6),
                new Vector2(10, 7),
                new Vector2(6, 9),
                new Vector2(2, 8),
                new Vector2(4, 3),
                new Vector2(-2, 2),
                new Vector2(-1, 7),
                new Vector2(-6, 5),
                new Vector2(-4, 1),
                new Vector2(-8, -1),
                new Vector2(1, -4),
            };*/

            /*mPolygonPoints = new List<Vector2>(){
                new Vector2(0, 2),
                new Vector2(2, 4),
                new Vector2(3, 0),
                new Vector2(4, 5),
                new Vector2(5, 2),
                new Vector2(8, 3),
                new Vector2(4, 7),
                new Vector2(5, 10),
                new Vector2(0, 8),
            };*/

            /*mPolygonPoints = new List<Vector2>(){
                new Vector2(2.855037f, 0),
                new Vector2(3.996893f, 1.779532f),
                new Vector2(1.00407267f, 1.11513579f),
                new Vector2(1.43404579f, 4.41353941f),
                new Vector2(-0.1384051f, 1.316836f),
                new Vector2(-2.411618f, 4.177045f),
                new Vector2(-3.416859f, 2.482493f),
                new Vector2(-2.944617f, 0.6258974f),
                new Vector2(-1.061315f, -0.2255896f),
                new Vector2(-1.905763f, -1.384618f),
                new Vector2(-0.6136396f, -1.062855f),
                new Vector2(-0.1183329f, -1.125863f),
                new Vector2(0.8013254f, -2.466225f),
                new Vector2(1.803385f, -2.002861f),
                new Vector2(3.597711f, -1.601804f),
            };*/
            
            mPolygonPoints = new List<Vector2>(){
                new Vector2(2.100612f, 0),
                new Vector2(1.875363f, 0.8349656f),
                new Vector2(0.9324085f, 1.035545f),
                new Vector2(0.4189189f, 1.2893f),
                new Vector2(-0.2101708f, 1.999641f),
                new Vector2(-0.9643731f, 1.670343f),
                new Vector2(-2.840736f, 2.063915f),
                new Vector2(-3.779511f, 0.8033596f),
                new Vector2(-1.629403f, -0.3463405f),
                new Vector2(-3.347722f, -2.432263f),
                new Vector2(-0.7758992f, -1.343897f),
                new Vector2(-0.2864465f, -2.72536f),
                new Vector2(0.3424261f, -1.053879f),
                new Vector2(2.696841f, -2.995144f),
                new Vector2(2.698487f, -1.201443f),
            };
        }

        private void generatePolygonButton_Click(object sender, RoutedEventArgs e)
        {
            /*var cnt = 15;
            var random = new Random();
            mPolygonPoints = new List<Vector2>();
            var angle = 0f;
            var angleDiff = 2f * (Single)Math.PI / cnt;
            for (int i = 0; i < cnt; i++)
            {
                var radius = random.NextFloat(1f, 5f);
                mPolygonPoints.Add(new Vector2(radius * (Single)Math.Cos(angle), radius * (Single)Math.Sin(angle)));
                angle += angleDiff;
            }*/

            polygon.Points.Clear();

            var minx = float.PositiveInfinity;
            var maxx = float.NegativeInfinity;
            var miny = float.PositiveInfinity;
            var maxy = float.NegativeInfinity;
            foreach (var point in mPolygonPoints)
            {
                minx = (float)Math.Min(point.X, minx);
                maxx = (float)Math.Max(point.X, maxx);
                miny = (float)Math.Min(point.Y, miny);
                maxy = (float)Math.Max(point.Y, maxy);
            }
            var width = (float)polygonCanvas.ActualWidth;
            var height = (float)polygonCanvas.ActualHeight;

            var xFact = width / (maxx - minx);
            var yFact = height / (maxy - miny);
            var middlex = (maxx + minx) / 2f;
            var middley = (maxy + miny) / 2f;

            var factToUse = (float)Math.Min(xFact, yFact) * 0.8f;

            foreach (var point in mPolygonPoints)
            {
                polygon.Points.Add(new System.Windows.Point((point.X - middlex) * factToUse, (maxy - point.Y - middley) * factToUse));
            }

            var sLTI = SweepLinePolygonTriangulator.Triangulate(mPolygonPoints);
            if (sLTI.Count > 0)
            {
                var geometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
                geometry.Positions = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
                xFact = 10f / (maxx - minx);
                yFact = 10f / (maxy - miny);
                factToUse = (float)Math.Min(xFact, yFact) * 0.8f;
                foreach (var point in mPolygonPoints)
                    geometry.Positions.Add(new Vector3((point.X - middlex) * factToUse, 0, (maxy - (point.Y - middley)) * factToUse));
                geometry.Indices = new HelixToolkit.Wpf.SharpDX.Core.IntCollection(sLTI);
                triangulatedPolygon.Geometry = geometry;
                var lb = new LineBuilder();
                for (int i = 0; i < sLTI.Count; i += 3)
                {
                    lb.AddLine(geometry.Positions[sLTI[i]], geometry.Positions[sLTI[i + 1]]);
                    lb.AddLine(geometry.Positions[sLTI[i + 1]], geometry.Positions[sLTI[i + 2]]);
                    lb.AddLine(geometry.Positions[sLTI[i + 2]], geometry.Positions[sLTI[i]]);
                }
                lineTriangulatedPolygon.Geometry = lb.ToLineGeometry3D();
            }
            
            var tt = (TranslateTransform)((TransformGroup)polygonCanvas.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
            Vector v = new Vector(width / 2f, height / 2f);
            origin = new System.Windows.Point(v.X, v.Y);
            tt.X = origin.X;
            tt.Y = origin.Y;
        }

        private void polygonCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var st = (ScaleTransform)((TransformGroup)polygonCanvas.RenderTransform)
                .Children.First(tr => tr is ScaleTransform); ;
            double zoom = e.Delta > 0 ? .2 : -.2;
            st.ScaleX += zoom;
            st.ScaleY += zoom;
        }

        private void polygonCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            start = e.GetPosition(border);
            polygonCanvas.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)polygonCanvas.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
        }

        private void polygonCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (polygonCanvas.IsMouseCaptured)
            {
                var tt = (TranslateTransform)((TransformGroup)polygonCanvas.RenderTransform)
                    .Children.First(tr => tr is TranslateTransform);
                Vector v = start - e.GetPosition(border);
                tt.X = origin.X - v.X;
                tt.Y = origin.Y - v.Y;
            }
        }

        private void polygonCanvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            polygonCanvas.ReleaseMouseCapture();
            var tt = (TranslateTransform)((TransformGroup)polygonCanvas.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
            Vector v = start - e.GetPosition(border);
            origin -= v;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ///generatePolygonButton_Click(sender, new RoutedEventArgs());
        }
    }
}
