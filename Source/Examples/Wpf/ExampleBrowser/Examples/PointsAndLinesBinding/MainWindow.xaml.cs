using CommunityToolkit.Mvvm.ComponentModel;
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

namespace PointsAndLinesBinding;

/// <summary>
/// Interaction logic for the main window.
/// </summary>
[ExampleBrowser.Example("PointsAndLinesBinding", "Binding listening to NotifyCollectionChanged.")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[ObservableObject]
public partial class MainWindow : Window
{
    private readonly Stopwatch watch = new();

    [ObservableProperty]
    private Point3DCollection linePoints = new();

    [ObservableProperty]
    private Point3DCollection points = new();

    [ObservableProperty]
    private int numberOfPoints;

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
    public bool ReplacePoints { get; set; }

    private void OnCompositionTargetRendering(object? sender, EventArgs e)
    {
        if (this.ReplacePoints)
        {
            this.SetPoints();
        }
        else
        {
            this.UpdatePoints();
        }
    }

    private void SetPoints()
    {
        if (!this.ShowLinesVisual3D)
        {
            this.LinePoints = new Point3DCollection();
        }

        if (!this.ShowPointsVisual3D)
        {
            this.Points = new Point3DCollection();
        }

        if (this.ShowLinesVisual3D || this.ShowPointsVisual3D)
        {
            var newPoints =
                PointsAndLines.MainWindow.GeneratePoints(this.NumberOfPoints, this.watch.ElapsedMilliseconds * 0.001)
                    .ToArray();
            if (this.ShowPointsVisual3D)
            {
                var pc = new Point3DCollection(newPoints);
                pc.Freeze();
                this.Points = pc;
            }

            if (this.ShowLinesVisual3D)
            {
                var pc = new Point3DCollection(newPoints);
                pc.Freeze();
                this.LinePoints = pc;
            }
        }
    }

    private void UpdatePoints()
    {
        if (this.ShowLinesVisual3D || this.ShowPointsVisual3D)
        {
            var newPoints =
                PointsAndLines.MainWindow.GeneratePoints(this.NumberOfPoints, this.watch.ElapsedMilliseconds * 0.001)
                    .ToArray();
            if (this.ShowPointsVisual3D)
            {
                if (this.Points.IsFrozen)
                {
                    this.Points = new Point3DCollection();
                }
                else
                {
                    this.Points.Clear();
                }

                foreach (var p in newPoints)
                {
                    this.Points.Add(p);
                }
            }

            if (this.ShowLinesVisual3D)
            {
                if (this.LinePoints.IsFrozen)
                {
                    this.LinePoints = new Point3DCollection();
                }
                else
                {
                    this.LinePoints.Clear();
                }

                foreach (var p in newPoints)
                {
                    this.LinePoints.Add(p);
                }
            }
        }
    }

    private void ExitClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
