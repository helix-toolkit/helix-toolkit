// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridLines.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents grid lines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HippoDemo
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Represents grid lines.
    /// </summary>
    public class GridLines : ModelVisual3D
    {
        /// <summary>
        /// The center property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Point3D), typeof(GridLines), new UIPropertyMetadata(new Point3D(), GeometryChanged));

        /// <summary>
        /// The distance property.
        /// </summary>
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register(
            "MinorDistance", typeof(double), typeof(GridLines), new PropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// The length direction property.
        /// </summary>
        public static readonly DependencyProperty LengthDirectionProperty =
            DependencyProperty.Register(
                "LengthDirection",
                typeof(Vector3D),
                typeof(GridLines),
                new UIPropertyMetadata(new Vector3D(1, 0, 0), GeometryChanged));

        /// <summary>
        /// The length property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(GridLines), new PropertyMetadata(100.0, GeometryChanged));

        /// <summary>
        /// The major distance property.
        /// </summary>
        public static readonly DependencyProperty MajorDistanceProperty = DependencyProperty.Register(
            "MajorDistance", typeof(double), typeof(GridLines), new PropertyMetadata(10.0, GeometryChanged));

        /// <summary>
        /// The major line color property.
        /// </summary>
        public static readonly DependencyProperty MajorLineColorProperty = DependencyProperty.Register(
            "MajorLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(140, 140, 140)));

        /// <summary>
        /// The major line thickness property.
        /// </summary>
        public static readonly DependencyProperty MajorLineThicknessProperty =
            DependencyProperty.Register(
                "MajorLineThickness", typeof(double), typeof(GridLines), new UIPropertyMetadata(1.2));

        /// <summary>
        /// The minor line color property.
        /// </summary>
        public static readonly DependencyProperty MinorLineColorProperty = DependencyProperty.Register(
            "MinorLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(150, 150, 150)));

        /// <summary>
        /// The minor line thickness property.
        /// </summary>
        public static readonly DependencyProperty MinorLineThicknessProperty =
            DependencyProperty.Register(
                "MinorLineThickness", typeof(double), typeof(GridLines), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The normal property.
        /// </summary>
        public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
            "Normal",
            typeof(Vector3D),
            typeof(GridLines),
            new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        /// <summary>
        /// The width property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width", typeof(double), typeof(GridLines), new PropertyMetadata(100.0, GeometryChanged));

        /// <summary>
        /// The x axis line color property.
        /// </summary>
        public static readonly DependencyProperty XAxisLineColorProperty = DependencyProperty.Register(
            "XAxisLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(150, 75, 75)));

        /// <summary>
        /// The y axis line color property.
        /// </summary>
        public static readonly DependencyProperty YAxisLineColorProperty = DependencyProperty.Register(
            "YAxisLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(75, 150, 75)));

        /// <summary>
        /// The z axis line color property.
        /// </summary>
        public static readonly DependencyProperty ZAxisLineColorProperty = DependencyProperty.Register(
            "ZAxisLineColor", typeof(Color), typeof(GridLines), new UIPropertyMetadata(Color.FromRgb(75, 75, 150)));

        /// <summary>
        /// The length direction.
        /// </summary>
        private Vector3D lengthDirection;

        /// <summary>
        /// The width direction.
        /// </summary>
        private Vector3D widthDirection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridLines"/> class.
        /// </summary>
        public GridLines()
        {
            this.CreateGrid();
        }

        /// <summary>
        /// Gets or sets the center of the grid.
        /// </summary>
        /// <value> The center. </value>
        public Point3D Center
        {
            get
            {
                return (Point3D)this.GetValue(CenterProperty);
            }

            set
            {
                this.SetValue(CenterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value> The length. </value>
        public double Length
        {
            get
            {
                return (double)this.GetValue(LengthProperty);
            }

            set
            {
                this.SetValue(LengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length direction.
        /// </summary>
        /// <value> The length direction. </value>
        public Vector3D LengthDirection
        {
            get
            {
                return (Vector3D)this.GetValue(LengthDirectionProperty);
            }

            set
            {
                this.SetValue(LengthDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance between major grid lines.
        /// </summary>
        /// <value> The major distance. </value>
        public double MajorDistance
        {
            get
            {
                return (double)this.GetValue(MajorDistanceProperty);
            }

            set
            {
                this.SetValue(MajorDistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the major line.
        /// </summary>
        /// <value> The color of the major line. </value>
        public Color MajorLineColor
        {
            get
            {
                return (Color)this.GetValue(MajorLineColorProperty);
            }

            set
            {
                this.SetValue(MajorLineColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the major line thickness.
        /// </summary>
        /// <value> The major line thickness. </value>
        public double MajorLineThickness
        {
            get
            {
                return (double)this.GetValue(MajorLineThicknessProperty);
            }

            set
            {
                this.SetValue(MajorLineThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance between minor grid lines.
        /// </summary>
        /// <value> The minor distance. </value>
        public double MinorDistance
        {
            get
            {
                return (double)this.GetValue(DistanceProperty);
            }

            set
            {
                this.SetValue(DistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the minor line.
        /// </summary>
        /// <value> The color of the minor line. </value>
        public Color MinorLineColor
        {
            get
            {
                return (Color)this.GetValue(MinorLineColorProperty);
            }

            set
            {
                this.SetValue(MinorLineColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minor line thickness.
        /// </summary>
        /// <value> The minor line thickness. </value>
        public double MinorLineThickness
        {
            get
            {
                return (double)this.GetValue(MinorLineThicknessProperty);
            }

            set
            {
                this.SetValue(MinorLineThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the normal of the grid.
        /// </summary>
        /// <value> The normal. </value>
        public Vector3D Normal
        {
            get
            {
                return (Vector3D)this.GetValue(NormalProperty);
            }

            set
            {
                this.SetValue(NormalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value> The width. </value>
        public double Width
        {
            get
            {
                return (double)this.GetValue(WidthProperty);
            }

            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the X axis line.
        /// </summary>
        /// <value> The color of the X axis line. </value>
        public Color XAxisLineColor
        {
            get
            {
                return (Color)this.GetValue(XAxisLineColorProperty);
            }

            set
            {
                this.SetValue(XAxisLineColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the Y axis line.
        /// </summary>
        /// <value> The color of the Y axis line. </value>
        public Color YAxisLineColor
        {
            get
            {
                return (Color)this.GetValue(YAxisLineColorProperty);
            }

            set
            {
                this.SetValue(YAxisLineColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the Z axis line.
        /// </summary>
        /// <value> The color of the Z axis line. </value>
        public Color ZAxisLineColor
        {
            get
            {
                return (Color)this.GetValue(ZAxisLineColorProperty);
            }

            set
            {
                this.SetValue(ZAxisLineColorProperty, value);
            }
        }

        /// <summary>
        /// Creates the grid.
        /// </summary>
        protected void CreateGrid()
        {
            var majorLinePoints = new Point3DCollection();
            var minorLinePoints = new Point3DCollection();
            var xlinePoints = new Point3DCollection();
            var ylinePoints = new Point3DCollection();

            this.lengthDirection = this.LengthDirection;
            this.lengthDirection.Normalize();
            this.widthDirection = Vector3D.CrossProduct(this.Normal, this.lengthDirection);
            this.widthDirection.Normalize();

            double minX = -this.Width / 2;
            double minY = -this.Length / 2;
            double maxX = this.Width / 2;
            double maxY = this.Length / 2;

            double z = this.MajorDistance * 1e-4;

            double x = minX;
            double eps = this.MinorDistance / 10;
            while (x <= maxX + eps)
            {
                var pc = IsMultipleOf(x, this.MajorDistance) ? majorLinePoints : minorLinePoints;
                if (Math.Abs(x) < double.Epsilon)
                {
                    this.AddLine(pc, this.GetPoint(x, minY, z), this.GetPoint(x, 0, z));
                    this.AddLine(ylinePoints, this.GetPoint(x, 0, 2 * z), this.GetPoint(x, maxY, 2 * z));
                }
                else
                {
                    this.AddLine(pc, this.GetPoint(x, minY, z), this.GetPoint(x, maxY, z));
                }

                x += this.MinorDistance;
            }

            double y = minY;
            while (y <= maxY + eps)
            {
                var pc = IsMultipleOf(y, this.MajorDistance) ? majorLinePoints : minorLinePoints;
                if (Math.Abs(y) < double.Epsilon)
                {
                    this.AddLine(pc, this.GetPoint(minX, y), this.GetPoint(0, y));
                    this.AddLine(xlinePoints, this.GetPoint(0, y, 2 * z), this.GetPoint(maxX, y, 2 * z));
                }
                else
                {
                    this.AddLine(pc, this.GetPoint(minX, y), this.GetPoint(maxY, y));
                }

                y += this.MinorDistance;
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

            var xlines = new LinesVisual3D
                {
                    Color = this.YAxisLineColor,
                    Thickness = this.MajorLineThickness,
                    Points = xlinePoints,
                    IsRendering = true
                };

            var ylines = new LinesVisual3D
                {
                    Color = this.XAxisLineColor,
                    Thickness = this.MajorLineThickness,
                    Points = ylinePoints,
                    IsRendering = true
                };
            this.Children.Add(majorLines);
            this.Children.Add(minorLines);
            this.Children.Add(xlines);
            this.Children.Add(ylines);
        }

        /// <summary>
        /// The geometry changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GridLines)d).OnGeometryChanged();
        }

        /// <summary>
        /// Determines whether the specified value is a multiple of x.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="x">
        /// The x value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value is a multiple of x; otherwise, <c>false</c> .
        /// </returns>
        private static bool IsMultipleOf(double value, double x)
        {
            double y2 = x * (int)(value / x);
            return Math.Abs(value - y2) < 1e-3;
        }

        /// <summary>
        /// Adds the line.
        /// </summary>
        /// <param name="pc">
        /// The pc.
        /// </param>
        /// <param name="p0">
        /// The p0.
        /// </param>
        /// <param name="p1">
        /// The p1.
        /// </param>
        /// <param name="divisions">
        /// The divisions.
        /// </param>
        private void AddLine(Point3DCollection pc, Point3D p0, Point3D p1, int divisions = 10)
        {
            var v = p1 - p0;
            for (int i = 0; i < divisions; i++)
            {
                pc.Add(p0 + v * i / divisions);
                pc.Add(p0 + v * (i + 1) / divisions);
            }
        }

        /// <summary>
        /// Gets the point at the specified local coordinates.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        /// <returns>
        /// A point.
        /// </returns>
        private Point3D GetPoint(double x, double y, double z = 0)
        {
            return this.Center + this.widthDirection * x + this.lengthDirection * y + this.Normal * z;
        }

        /// <summary>
        /// Called when the geometry changed.
        /// </summary>
        private void OnGeometryChanged()
        {
            foreach (LinesVisual3D lines in this.Children)
            {
                lines.IsRendering = false;
            }

            this.Children.Clear();
            this.CreateGrid();
        }

    }
}