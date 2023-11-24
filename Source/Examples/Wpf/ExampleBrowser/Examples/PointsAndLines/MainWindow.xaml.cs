using _3DTools;
using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.Wpf;
using Petzold.Media3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace PointsAndLines;

/// <summary>
/// Interaction logic for the main window.
/// </summary>
[ExampleBrowser.Example("PointsAndLines", "Renders text and lines.")]
[ObservableObject]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public partial class MainWindow : Window
{
    private readonly Stopwatch watch = new();

    [ObservableProperty]
    private int numberOfPoints;

    [ObservableProperty]
    private Point3DCollection? points;

    private LinesVisual3D? linesVisual;
    private PointsVisual3D? pointsVisual;
    private ScreenSpaceLines3D? screenSpaceLines;
    private WireLines? wireLines;

    public MainWindow()
    {
        this.InitializeComponent();
        this.watch.Start();

        this.NumberOfPoints = 100;
        this.DataContext = this;

        CompositionTarget.Rendering += this.OnCompositionTargetRendering;
    }

    public bool ShowLinesVisual3D { get; set; }

    public bool ShowPointsVisual3D { get; set; }

    public bool ShowScreenSpaceLines3D { get; set; }

    public bool ShowWireLines { get; set; }

    public bool IsMirrored { get; set; }

    public static IEnumerable<Point3D> GeneratePoints(int n, double time)
    {
        const double R = 2;
        const double Q = 0.5;
        for (int i = 0; i < n; i++)
        {
            double t = Math.PI * 2 * i / (n - 1);
            double u = (t * 24) + (time * 5);
            var pt = new Point3D(Math.Cos(t) * (R + (Q * Math.Cos(u))), Math.Sin(t) * (R + (Q * Math.Cos(u))), Q * Math.Sin(u));
            yield return pt;
            if (i > 0 && i < n - 1)
            {
                yield return pt;
            }
        }
    }

    private void OnCompositionTargetRendering(object? sender, EventArgs e)
    {
        if (this.ShowLinesVisual3D && this.linesVisual == null)
        {
            this.linesVisual = new LinesVisual3D { Color = Colors.Blue };
            View1.Children.Add(this.linesVisual);
        }

        if (!this.ShowLinesVisual3D && this.linesVisual != null)
        {
            this.linesVisual.IsRendering = false;
            View1.Children.Remove(this.linesVisual);
            this.linesVisual = null;
        }

        if (this.ShowPointsVisual3D && this.pointsVisual == null)
        {
            this.pointsVisual = new PointsVisual3D { Color = Colors.Red, Size = 6 };
            View1.Children.Add(this.pointsVisual);
        }

        if (!this.ShowPointsVisual3D && this.pointsVisual != null)
        {
            this.pointsVisual.IsRendering = false;
            View1.Children.Remove(this.pointsVisual);
            this.pointsVisual = null;
        }

        if (this.ShowScreenSpaceLines3D && this.screenSpaceLines == null)
        {
            this.screenSpaceLines = new ScreenSpaceLines3D { Color = Colors.Green };
            View1.Children.Add(this.screenSpaceLines);
        }

        if (!this.ShowScreenSpaceLines3D && this.screenSpaceLines != null)
        {
            /* Please close and reopen the Window to remove screenSpaceLines. */
            //this.screenSpaceLines.IsRendering = false; // property IsRendering does not exist
            //View1.Children.Remove(this.screenSpaceLines);
            //this.screenSpaceLines = null;
        }

        if (this.ShowWireLines && this.wireLines == null)
        {
            this.wireLines = new WireLines { Color = Colors.Pink };
            View1.Children.Add(this.wireLines);
        }

        if (!this.ShowWireLines && this.wireLines != null)
        {
            View1.Children.Remove(this.wireLines);
            this.wireLines = null;
        }

        if (this.Points == null || this.Points.Count != this.NumberOfPoints)
        {
            this.Points = new Point3DCollection(GeneratePoints(this.NumberOfPoints, this.watch.ElapsedMilliseconds * 0.001));
        }

        if (this.linesVisual != null)
        {
            this.linesVisual.Points = this.Points;
            this.linesVisual.Transform = IsMirrored ? new ScaleTransform3D(-1, 1, 1) : new ScaleTransform3D(1, 1, 1);
        }

        if (this.pointsVisual != null)
        {
            this.pointsVisual.Points = this.Points;
            this.pointsVisual.Transform = IsMirrored ? new ScaleTransform3D(-1, 1, 1) : new ScaleTransform3D(1, 1, 1);
        }

        if (this.screenSpaceLines != null)
        {
            this.screenSpaceLines.Points = this.Points;
            this.screenSpaceLines.Transform = IsMirrored ? new ScaleTransform3D(-1, 1, 1) : new ScaleTransform3D(1, 1, 1);
        }

        if (this.wireLines != null)
        {
            this.wireLines.Lines = this.Points;
            this.wireLines.Transform = IsMirrored ? new ScaleTransform3D(-1, 1, 1) : new ScaleTransform3D(1, 1, 1);
        }
    }

    private void ExitClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
