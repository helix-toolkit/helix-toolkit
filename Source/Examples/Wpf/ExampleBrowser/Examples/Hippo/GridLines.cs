using DependencyPropertyGenerator;
using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Hippo;

/// <summary>
/// Represents grid lines.
/// </summary>
[DependencyProperty<Point3D>("Center", OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<double>("MinorDistance", DefaultValue = 1.0, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<Vector3D>("LengthDirection", DefaultValueExpression = "new System.Windows.Media.Media3D.Vector3D(1, 0, 0)", OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<double>("Length", DefaultValue = 100.0, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<double>("MajorDistance", DefaultValue = 10.0, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<Color>("MajorLineColor", DefaultValueExpression = "System.Windows.Media.Color.FromRgb(140, 140, 140)")]
[DependencyProperty<double>("MajorLineThickness", DefaultValue = 1.2)]
[DependencyProperty<Color>("MinorLineColor", DefaultValueExpression = "System.Windows.Media.Color.FromRgb(150, 150, 150)")]
[DependencyProperty<double>("MinorLineThickness", DefaultValue = 1.0)]
[DependencyProperty<Vector3D>("Normal", DefaultValueExpression = "new System.Windows.Media.Media3D.Vector3D(0, 0, 1)", OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<double>("Width", DefaultValue = 100.0, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<Color>("XAxisLineColor", DefaultValueExpression = "System.Windows.Media.Color.FromRgb(150, 75, 75)")]
[DependencyProperty<Color>("YAxisLineColor", DefaultValueExpression = "System.Windows.Media.Color.FromRgb(75, 150, 75)")]
[DependencyProperty<Color>("ZAxisLineColor", DefaultValueExpression = "System.Windows.Media.Color.FromRgb(75, 75, 150)")]
public partial class GridLines : ModelVisual3D
{
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
