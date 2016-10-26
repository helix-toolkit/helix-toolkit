// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Satellite3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using HelixToolkit.Wpf;
using System.Windows.Media.Imaging;

namespace SolarsystemDemo
{
    public class Satellite3D : TexturedObject3D
    {
        public double Apogee { get; set; }
        public double Perigee { get; set; }
        public double SemiMajorAxis { get; set; }
        public double Eccentricity { get; set; }
        public double OrbitalPeriod { get; set; }
        public double Inclination { get; set; }
        //  public double LongitudeOfAscendingNode {get; set; }
        //  public double ArgumentOfPerihelion {get; set; }
        public double AxialTilt { get; set; }
        public double RotationPeriod { get; set; }

        public Planet3D Planet { get; set; }
        public SolarSystem3D SolarSystem { get; set; }
        TubeVisual3D orbit;

        public Satellite3D()
        {
            orbit = new TubeVisual3D() { Diameter = 0.3, ThetaDiv = 12 };
            orbit.Material = MaterialHelper.CreateMaterial(null, Brushes.Gray, Brushes.White, 0.5, 40);

            Children.Add(orbit);
        }

        public Point3D CalculatePosition(double angle, double scale)
        {
            double a = SemiMajorAxis / scale;
            double e = Eccentricity;
            double b = a * Math.Sqrt(1 - e * e);
            Point3D p = new Point3D(Math.Cos(angle) * a, Math.Sin(angle) * b, 0);
            return p;
        }

        public void UpdateTransform()
        {
            var tg = new Transform3DGroup();
            // tg.Children.Add(Planet.Sphere.Transform);
            // http://en.wikipedia.org/wiki/Argument_of_periapsis
            //  tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0,0,1),LongitudeOfAscendingNode)));
            tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), Inclination)));
            tg.Children.Add(new TranslateTransform3D(Planet.Position.X, Planet.Position.Y, Planet.Position.Z));
            //  tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1,0,0),ArgumentOfPerihelion)));
            Transform = tg;
        }

        public void UpdateOrbit()
        {
            if (SemiMajorAxis > 0)
            {
                int n = 90;
                var path = new Point3DCollection();
                for (int i = 0; i < n; i++)
                    path.Add(CalculatePosition((double)i / (n - 1) * Math.PI * 2, Planet.DistanceScale));

                orbit.Path = path;
                orbit.UpdateModel();
            }

            Sphere.Radius = MeanRadius / (Planet.DiameterScale);
        }

        void UpdatePosition()
        {
            double ang = 0;
            if (OrbitalPeriod != 0)
                ang = SolarSystem.Days / OrbitalPeriod * Math.PI * 2;
            var pos = CalculatePosition(ang, Planet.DistanceScale);

            // http://en.wikipedia.org/wiki/Axial_tilt
            // http://en.wikipedia.org/wiki/Rotation_period
            var rotang = 0.0;
            if (RotationPeriod != 0)
                rotang = SolarSystem.Days / RotationPeriod * 360;
            var axis = new Vector3D(0, 0, 1); // todo: should be normal to orbital plane

            var tg = new Transform3DGroup();
            tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axis, rotang)));
            tg.Children.Add(new TranslateTransform3D(pos.X, pos.Y, pos.Z));
            Sphere.Transform = tg;
        }

        public void InitModel(Planet3D p)
        {
            Planet = p;
            SolarSystem = p.SolarSystem;
            UpdateTransform();
            UpdateOrbit();
        }

        public void UpdateModel()
        {
            UpdateTransform();
            UpdatePosition();
        }

    }
}