// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindTurbineVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace WindDemo
{
    public class WindTurbineVisual3D : ModelVisual3D
    {
        public static readonly DependencyProperty WindTurbineProperty =
            DependencyProperty.Register("WindTurbine", typeof(WindTurbine), typeof(WindTurbineVisual3D),
                                        new UIPropertyMetadata(null, TurbineChanged));

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(WindTurbineVisual3D),
                                        new UIPropertyMetadata(0.0));

        public int Blades
        {
            get { return WindTurbine.Blades; }
            set
            {
                WindTurbine.Blades = value;
                UpdateVisuals();
            }
        }

        public double Height
        {
            get { return WindTurbine.Height; }
            set
            {
                WindTurbine.Height = value;
                UpdateVisuals();
            }
        }

        public WindTurbine WindTurbine
        {
            get { return (WindTurbine)GetValue(WindTurbineProperty); }
            set { SetValue(WindTurbineProperty, value); }
        }


        /// <summary>
        /// Gets or sets the rotation angle (angle of the rotor).
        /// </summary>
        /// <value>The rotation angle.</value>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the rotation speed (rpm).
        /// </summary>
        /// <value>The rotation speed.</value>
        public double RotationSpeed
        {
            get { return (double)GetValue(RotationSpeedProperty); }
            set { SetValue(RotationSpeedProperty, value); }
        }

        public static readonly DependencyProperty RotationSpeedProperty =
            DependencyProperty.Register("RotationSpeed", typeof(double), typeof(WindTurbineVisual3D), new UIPropertyMetadata(10.0));


        protected static void TurbineChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((WindTurbineVisual3D)obj).UpdateVisuals();
        }

        private readonly Stopwatch watch = new Stopwatch();

        private readonly RenderingEventListener renderingEventListener;

        public WindTurbineVisual3D()
        {
            renderingEventListener=new RenderingEventListener(this.OnCompositionTargetRendering);
            RenderingEventManager.AddListener(renderingEventListener);
        }

        private void OnCompositionTargetRendering(object sender, RenderingEventArgs e)
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

            var b = new Binding("RotationAngle") { Source = this };
            BindingOperations.SetBinding(rotation, AxisAngleRotation3D.AngleProperty, b);
        }
    }
}