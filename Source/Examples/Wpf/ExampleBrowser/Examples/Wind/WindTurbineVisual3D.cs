using DependencyPropertyGenerator;
using HelixToolkit.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Wind;

[DependencyProperty<WindTurbine>("WindTurbine")]
[DependencyProperty<double>("RotationAngle", DefaultValue = 0.0)]
[DependencyProperty<double>("RotationSpeed", DefaultValue = 10.0)]
public sealed partial class WindTurbineVisual3D : ModelVisual3D
{
    public int Blades
    {
        get { return WindTurbine!.Blades; }
        set
        {
            if (WindTurbine is not null)
            {
                WindTurbine.Blades = value;
            }

            UpdateVisuals();
        }
    }

    public double Height
    {
        get { return WindTurbine!.Height; }
        set
        {
            if (WindTurbine is not null)
            {
                WindTurbine.Height = value;
            }

            UpdateVisuals();
        }
    }

    partial void OnWindTurbineChanged()
    {
        UpdateVisuals();
    }

    private readonly Stopwatch watch = new();

    private readonly RenderingEventListener renderingEventListener;

    public WindTurbineVisual3D()
    {
        renderingEventListener = new RenderingEventListener(this.OnCompositionTargetRendering);
        RenderingEventManager.AddListener(renderingEventListener);
    }

    private void OnCompositionTargetRendering(object? sender, RenderingEventArgs? e)
    {
        double delta = watch.ElapsedMilliseconds * 0.001;
        RotationAngle += 360 * RotationSpeed / 60 * delta;
        watch.Restart();
    }

    private void UpdateVisuals()
    {
        Children.Clear();
        if (WindTurbine == null) return;

        var baseTower = new TruncatedConeVisual3D
        {
            Fill = Brushes.Yellow,
            Origin = new Point3D(0, 0, -WindTurbine.BaseHeight)
        };
        baseTower.Height = -baseTower.Origin.Z + 2;
        baseTower.BaseRadius = baseTower.TopRadius = WindTurbine.Diameter;

        var tower = new TruncatedConeVisual3D
        {
            Fill = Brushes.White,
            Origin = new Point3D(0, 0, 2),
            Height = WindTurbine.Height,
            BaseRadius = WindTurbine.Diameter
        };
        tower.TopRadius = tower.BaseRadius * (1 - WindTurbine.Height * Math.Sin(WindTurbine.ShaftAngle / 180.0 * Math.PI));

        var nacelle = new TruncatedConeVisual3D
        {
            Fill = Brushes.White,
            Origin = new Point3D(WindTurbine.Overhang, 0, tower.Origin.Z + tower.Height),
            Normal = new Vector3D(-1, 0, 0),
            TopRadius = WindTurbine.NacelleDiameter
        };
        nacelle.BaseRadius = nacelle.TopRadius * 0.7;
        nacelle.Height = WindTurbine.NacelleLength;

        Children.Add(baseTower);
        Children.Add(tower);
        Children.Add(nacelle);


        var endcap = new SphereVisual3D
        {
            Center = new Point3D(WindTurbine.Overhang - WindTurbine.NacelleLength, 0,
                                                  tower.Origin.Z + tower.Height),
            Radius = nacelle.TopRadius,
            Fill = Brushes.White
        };
        Children.Add(endcap);

        var rotor = new ModelVisual3D();

        for (int i = 0; i < WindTurbine.Blades; i++)
        {
            double angle = (double)i / WindTurbine.Blades * Math.PI * 2;

            // todo: the blade is simplified to a cone... it should be a real profile...
            var blade = new TruncatedConeVisual3D
            {
                Origin = nacelle.Origin,
                Normal = new Vector3D(0, Math.Cos(angle), Math.Sin(angle)),
                Height = WindTurbine.BladeLength,
                BaseRadius = WindTurbine.BladeRootChord,
                TopRadius = WindTurbine.BladeTipChord,
                Fill = Brushes.White
            };
            rotor.Children.Add(blade);
        }

        var hub = new SphereVisual3D
        {
            Fill = Brushes.White,
            Center = nacelle.Origin,
            Radius = WindTurbine.HubDiameter / 2
        };
        rotor.Children.Add(hub);
        Children.Add(rotor);

        var rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
        var rotorTransform = new RotateTransform3D(null, hub.Center) { Rotation = rotation };
        rotor.Transform = rotorTransform;

        var b = new Binding(nameof(RotationAngle)) { Source = this };
        BindingOperations.SetBinding(rotation, AxisAngleRotation3D.AngleProperty, b);
    }
}
