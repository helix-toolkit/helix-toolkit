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
        List<Vector2> mPolygonPoints;

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
            
            /*mPolygonPoints = new List<Vector2>(){
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
            };*/
        }

        private void generatePolygonButton_Click(object sender, RoutedEventArgs e)
        {
            /*var random = new Random();
            var cnt = random.Next(15, 2000);
            mPolygonPoints = new List<Vector2>();
            var angle = 0f;
            var angleDiff = 2f * (Single)Math.PI / cnt;
            for (int i = 0; i < cnt; i++)
            {
                var radius = random.NextFloat(3f, 5f);
                mPolygonPoints.Add(new Vector2(radius * (Single)Math.Cos(angle), radius * (Single)Math.Sin(angle)));
                angle += angleDiff;
            }*/
            var pointsParts = GetTextOutlines("cCEfFGhHIJkKlLmMnNrsStTuUvVwWxXyYzZ", "Times new Roman", FontStyles.Normal, FontWeights.Normal, 1).Select(ch => ch.ElementAt(0).Select(p => p.ToVector2()).ToList());
            var cnt = 0;
            var geometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
            geometry.Positions = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
            geometry.Indices = new HelixToolkit.Wpf.SharpDX.Core.IntCollection();
            var lb = new LineBuilder();
            var before = DateTime.Now; 
            foreach (var outline in pointsParts)
            {
                var sLTI = SweepLinePolygonTriangulator.Triangulate(outline);
                foreach (var point in outline)
                    geometry.Positions.Add(new Vector3(point.X - 5, 0, point.Y + 5));
                geometry.Indices.AddRange(sLTI.Select(i => i + cnt));
                for (int i = 0; i < sLTI.Count; i += 3)
                {
                    lb.AddLine(geometry.Positions[sLTI[i] + cnt], geometry.Positions[sLTI[i + 1] + cnt]);
                    lb.AddLine(geometry.Positions[sLTI[i + 1] + cnt], geometry.Positions[sLTI[i + 2] + cnt]);
                    lb.AddLine(geometry.Positions[sLTI[i + 2] + cnt], geometry.Positions[sLTI[i] + cnt]);
                }
                cnt += outline.Count;
            }
            var after = DateTime.Now;
            triangulatedPolygon.Geometry = geometry;
            lineTriangulatedPolygon.Geometry = lb.ToLineGeometry3D();

            /*var before = DateTime.Now;
            var sLTI = SweepLinePolygonTriangulator.Triangulate(mPolygonPoints);
            var after = DateTime.Now;
            var geometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
            geometry.Positions = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
            foreach (var point in mPolygonPoints)
                geometry.Positions.Add(new Vector3(point.X, 0, point.Y + 5));
            geometry.Indices = new HelixToolkit.Wpf.SharpDX.Core.IntCollection(sLTI);
            triangulatedPolygon.Geometry = geometry;
            var lb = new LineBuilder();
            for (int i = 0; i < sLTI.Count; i += 3)
            {
                lb.AddLine(geometry.Positions[sLTI[i]], geometry.Positions[sLTI[i + 1]]);
                lb.AddLine(geometry.Positions[sLTI[i + 1]], geometry.Positions[sLTI[i + 2]]);
                lb.AddLine(geometry.Positions[sLTI[i + 2]], geometry.Positions[sLTI[i]]);
            }
            lineTriangulatedPolygon.Geometry = lb.ToLineGeometry3D();*/

            infoLabel.Content = String.Format("Last triangulation of {0} Points took {1:#.##} Milliseconds!", triangulatedPolygon.Geometry.Positions.Count, (after - before).TotalMilliseconds);
        }
        public IEnumerable<IList<System.Windows.Point[]>> GetTextOutlines(string text, string fontName, FontStyle fontStyle, FontWeight fontWeight, double fontSize)
        {
            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily(fontName), fontStyle, fontWeight, FontStretches.Normal),
                fontSize,
                Brushes.Black);

            var textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));
            var outlines = new List<List<System.Windows.Point[]>>();
            AppendOutlines(textGeometry, outlines);
            return outlines;
        }
        private void AppendOutlines(Geometry geometry, List<List<System.Windows.Point[]>> outlines)
        {
            var group = geometry as GeometryGroup;
            if (group != null)
            {
                foreach (var g in group.Children)
                {
                    AppendOutlines(g, outlines);
                }

                return;
            }

            var pathGeometry = geometry as PathGeometry;
            if (pathGeometry != null)
            {
                var figures = pathGeometry.Figures.Select(figure => figure.ToPolyLine()).ToList();
                outlines.Add(figures);
                return;
            }

            throw new NotImplementedException();
        }
    }
    public static class Extensions{
        public static System.Windows.Point[] ToPolyLine(this PathFigure figure)
        {
            var outline = new List<System.Windows.Point> { figure.StartPoint };
            var previousPoint = figure.StartPoint;
            foreach (var segment in figure.Segments)
            {
                var polyline = segment as PolyLineSegment;
                if (polyline != null)
                {
                    outline.AddRange(polyline.Points);
                    previousPoint = polyline.Points.Last();
                    continue;
                }

                var polybezier = segment as PolyBezierSegment;
                if (polybezier != null)
                {
                    for (int i = -1; i + 3 < polybezier.Points.Count; i += 3)
                    {
                        var p1 = i == -1 ? previousPoint : polybezier.Points[i];
                        outline.AddRange(FlattenBezier(p1, polybezier.Points[i + 1], polybezier.Points[i + 2], polybezier.Points[i + 3], 10));
                    }

                    previousPoint = polybezier.Points.Last();
                    continue;
                }

                var lineSegment = segment as LineSegment;
                if (lineSegment != null)
                {
                    outline.Add(lineSegment.Point);
                    previousPoint = lineSegment.Point;
                    continue;
                }

                var bezierSegment = segment as BezierSegment;
                if (bezierSegment != null)
                {
                    outline.AddRange(FlattenBezier(previousPoint, bezierSegment.Point1, bezierSegment.Point2, bezierSegment.Point3, 10));
                    previousPoint = bezierSegment.Point3;
                    continue;
                }

                throw new NotImplementedException();
            }

            return outline.ToArray();
        }
        private static IEnumerable<System.Windows.Point> FlattenBezier(System.Windows.Point p1, System.Windows.Point p2, System.Windows.Point p3, System.Windows.Point p4, int n)
        {
            // http://tsunami.cis.usouthal.edu/~hain/general/Publications/Bezier/bezier%20cccg04%20paper.pdf
            // http://en.wikipedia.org/wiki/De_Casteljau's_algorithm
            for (int i = 1; i <= n; i++)
            {
                var t = (double)i / n;
                var u = 1 - t;
                yield return new System.Windows.Point(
                    (u * u * u * p1.X) + (3 * t * u * u * p2.X) + (3 * t * t * u * p3.X) + (t * t * t * p4.X),
                    (u * u * u * p1.Y) + (3 * t * u * u * p2.Y) + (3 * t * t * u * p3.Y) + (t * t * t * p4.Y));
            }
        }
    }
}
