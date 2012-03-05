// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridLines.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace RhinoDemo
{
    using HelixToolkit.Wpf;

    public class GridLines : ModelVisual3D
    {
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point3D), typeof(GridLines),
                                        new UIPropertyMetadata(new Point3D(), GeometryChanged));

        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("MinorDistance", typeof(double), typeof(GridLines),
                                        new PropertyMetadata(1.0, GeometryChanged));

        private static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GridLines)d).OnGeometryChanged();
        }

        private void OnGeometryChanged()
        {
            foreach (LinesVisual3D lines in Children)
                lines.IsRendering = false;

            Children.Clear();
            CreateGrid();
        }

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(double), typeof(GridLines),
                                        new PropertyMetadata(100.0, GeometryChanged));

        public static readonly DependencyProperty LengthDirectionProperty =
            DependencyProperty.Register("LengthDirection", typeof(Vector3D), typeof(GridLines),
                                        new UIPropertyMetadata(new Vector3D(1, 0, 0), GeometryChanged));

        public static readonly DependencyProperty MajorDistanceProperty =
            DependencyProperty.Register("MajorDistance", typeof(double), typeof(GridLines),
                                        new PropertyMetadata(10.0, GeometryChanged));

        public static readonly DependencyProperty NormalProperty =
            DependencyProperty.Register("Normal", typeof(Vector3D), typeof(GridLines),
                                        new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        public double MajorLineThickness
        {
            get { return (double)GetValue(MajorLineThicknessProperty); }
            set { SetValue(MajorLineThicknessProperty, value); }
        }

        public static readonly DependencyProperty MajorLineThicknessProperty =
            DependencyProperty.Register("MajorLineThickness", typeof(double), typeof(GridLines), new UIPropertyMetadata(1.2));

        public double MinorLineThickness
        {
            get { return (double)GetValue(MinorLineThicknessProperty); }
            set { SetValue(MinorLineThicknessProperty, value); }
        }

        public static readonly DependencyProperty MinorLineThicknessProperty =
            DependencyProperty.Register("MinorLineThickness", typeof(double), typeof(GridLines), new UIPropertyMetadata(1.0));


        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(GridLines),
                                        new PropertyMetadata(100.0, GeometryChanged));

        public Color MajorLineColor
        {
            get { return (Color)GetValue(MajorLineColorProperty); }
            set { SetValue(MajorLineColorProperty, value); }
        }

        public static readonly DependencyProperty MajorLineColorProperty =
            DependencyProperty.Register("MajorLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(140, 140, 140)));

        public Color MinorLineColor
        {
            get { return (Color)GetValue(MinorLineColorProperty); }
            set { SetValue(MinorLineColorProperty, value); }
        }

        public static readonly DependencyProperty MinorLineColorProperty =
            DependencyProperty.Register("MinorLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(150, 150, 150)));

        public Color XAxisLineColor
        {
            get { return (Color)GetValue(XAxisLineColorProperty); }
            set { SetValue(XAxisLineColorProperty, value); }
        }

        public static readonly DependencyProperty XAxisLineColorProperty =
            DependencyProperty.Register("XAxisLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(150, 75, 75)));

        public Color YAxisLineColor
        {
            get { return (Color)GetValue(YAxisLineColorProperty); }
            set { SetValue(YAxisLineColorProperty, value); }
        }

        public static readonly DependencyProperty YAxisLineColorProperty =
            DependencyProperty.Register("YAxisLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(75, 150, 75)));


        public Color ZAxisLineColor
        {
            get { return (Color)GetValue(ZAxisLineColorProperty); }
            set { SetValue(ZAxisLineColorProperty, value); }
        }

        public static readonly DependencyProperty ZAxisLineColorProperty =
            DependencyProperty.Register("ZAxisLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(75, 75, 150)));

        private Vector3D lengthDirection;
        private Vector3D widthDirection;

        public Point3D Center
        {
            get { return (Point3D)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public Vector3D Normal
        {
            get { return (Vector3D)GetValue(NormalProperty); }
            set { SetValue(NormalProperty, value); }
        }

        public Vector3D LengthDirection
        {
            get { return (Vector3D)GetValue(LengthDirectionProperty); }
            set { SetValue(LengthDirectionProperty, value); }
        }

        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public double MinorDistance
        {
            get { return (double)GetValue(DistanceProperty); }
            set { SetValue(DistanceProperty, value); }
        }

        public double MajorDistance
        {
            get { return (double)GetValue(MajorDistanceProperty); }
            set { SetValue(MajorDistanceProperty, value); }
        }

        public GridLines()
        {
            CreateGrid();
        }

        protected void CreateGrid()
        {

            var majorLinePoints = new Point3DCollection();
            var minorLinePoints = new Point3DCollection();
            var xLinePoints = new Point3DCollection();
            var yLinePoints = new Point3DCollection();

            lengthDirection = LengthDirection;
            lengthDirection.Normalize();
            widthDirection = Vector3D.CrossProduct(Normal, lengthDirection);
            widthDirection.Normalize();

            var mesh = new MeshBuilder(true, false);
            double minX = -Width / 2;
            double minY = -Length / 2;
            double maxX = Width / 2;
            double maxY = Length / 2;

            double x = minX;
            double eps = MinorDistance / 10;
            while (x <= maxX + eps)
            {
                var pc = IsMultipleOf(x, MajorDistance) ? majorLinePoints : minorLinePoints;
                if (x == 0)
                {
                    AddLine(pc,GetPoint(x, minY),GetPoint(x, 0));
                    AddLine(yLinePoints,GetPoint(x, 0),GetPoint(x, maxY));
                }
                else
                {
                        AddLine(pc,GetPoint(x, minY),GetPoint(x,maxY));
                }
                x += MinorDistance;
            }

            double y = minY;
            while (y <= maxY + eps)
            {
                var pc = IsMultipleOf(y, MajorDistance) ? majorLinePoints : minorLinePoints;
                if (y == 0)
                {
                    AddLine(pc,GetPoint(minX, y),GetPoint(0, y));
                    AddLine(xLinePoints,GetPoint(0, y),GetPoint(maxX, y));

                }
                else
                {
                   AddLine(pc,GetPoint(minX, y),GetPoint(maxY, y));
                }
                y += MinorDistance;
            }

            var majorLines = new LinesVisual3D
                {
                    Color = this.MajorLineColor,
                    Thickness = this.MajorLineThickness,
                    Points = majorLinePoints,
                    IsRendering = true
                };

            var minorLines = new LinesVisual3D
                {
                    Color = this.MinorLineColor,
                    Thickness = this.MinorLineThickness,
                    Points = minorLinePoints,
                    IsRendering = true
                };
            var xLines = new LinesVisual3D
            {
                Color = this.YAxisLineColor,
                Thickness = this.MajorLineThickness,
                Points = xLinePoints,
                IsRendering = true
            };
            var yLines = new LinesVisual3D
            {
                Color = this.XAxisLineColor,
                Thickness = this.MajorLineThickness,
                Points = yLinePoints,
                IsRendering = true
            };
            Children.Add(majorLines);
            Children.Add(minorLines);
            Children.Add(xLines);
            Children.Add(yLines);
        }

        private void AddLine(Point3DCollection pc, Point3D p0, Point3D p1, int divisions = 10)
        {
            var v = p1 - p0;
            for (int i=0;i<divisions;i++)
            {
                pc.Add(p0 + v * (double)i/divisions);
                pc.Add(p0 + v * (double)(i+1)/divisions);
            }
        }

        private static bool IsMultipleOf(double y, double d)
        {
            double y2 = d * (int)(y / d);
            return Math.Abs(y - y2) < 1e-3;
        }

        private Point3D GetPoint(double x, double y)
        {
            return Center + widthDirection * x + lengthDirection * y;
        }
    }
}