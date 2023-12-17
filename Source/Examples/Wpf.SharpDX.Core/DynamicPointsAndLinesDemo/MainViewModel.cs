using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace DynamicPointsAndLinesDemo;

public partial class MainViewModel : ObservableObject, IDisposable
{
    public LineGeometry3D Lines { get; }
    public PointGeometry3D Points { get; }
    public Transform3D Lines1Transform { get; }
    public Transform3D Lines2Transform { get; }
    public Transform3D Points1Transform { get; }
    public Vector3D DirectionalLightDirection { get; }
    public Color DirectionalLightColor { get; }
    public Color AmbientLightColor { get; }
    public Stopwatch StopWatch { get; }

    public IEffectsManager EffectsManager { get; }
    public Camera Camera { get; }

    [ObservableProperty]
    private int numberOfPoints;

    partial void OnNumberOfPointsChanged(int value)
    {
        StopWatch.Stop();

        Lines.Indices = new IntCollection(value * 2);

        for (int i = 0; i < value * 2; i++)
        {
            Lines.Indices.Add(i);
        }

        StopWatch.Start();
    }

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        Camera = new PerspectiveCamera
        {
            Position = new Point3D(-90, 0, 95),
            LookDirection = new Vector3D(110, 0, -105),
            UpDirection = new Vector3D(0, 1, 0)
        };

        // setup lighting            
        AmbientLightColor = Colors.DimGray;
        DirectionalLightColor = Colors.White;
        DirectionalLightDirection = new Vector3D(-2, -5, -2);

        // model trafos
        Lines1Transform = new TranslateTransform3D(0, 0, 45);
        Lines2Transform = new TranslateTransform3D(0, 0, -45);
        Points1Transform = new TranslateTransform3D(0, 0, 0);

        Lines = new LineGeometry3D { IsDynamic = true, Positions = new Vector3Collection() };
        Points = new PointGeometry3D { IsDynamic = true, Positions = new Vector3Collection() };

        StopWatch = new Stopwatch();
        StopWatch.Start();

        NumberOfPoints = 900;
    }

    public void UpdatePoints()
    {
        if (StopWatch.IsRunning)
        {
            Points.Positions?.Clear();
            Points.Positions?.AddRange(GeneratePoints(NumberOfPoints, StopWatch.ElapsedMilliseconds * 0.003));
            Lines.Positions?.Clear();

            if (Points.Positions is not null)
            {
                Lines.Positions?.AddRange(Points.Positions);
            }

            Points.UpdateVertices();
            Lines.UpdateVertices();
        }
    }

    private static IEnumerable<Vector3> GeneratePoints(int n, double time)
    {
        const double R = 30;
        const double Q = 5;
        for (int i = 0; i < n; i++)
        {
            double t = Math.PI * 2 * i / (n - 1);
            double u = (t * 24) + (time * 5);
            var pt = new Vector3(
                (float)(Math.Cos(t) * (R + (Q * Math.Cos(u)))),
                (float)(Math.Sin(t) * (R + (Q * Math.Cos(u)))),
                (float)(Q * Math.Sin(u)));
            yield return pt;
            if (i > 0 && i < n - 1)
            {
                yield return pt;
            }
        }
    }

    public void Dispose()
    {
        EffectsManager?.Dispose();
    }
}
