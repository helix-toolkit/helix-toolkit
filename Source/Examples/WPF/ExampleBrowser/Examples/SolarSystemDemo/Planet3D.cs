// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Planet3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Inclination to invariable plane
//   http://en.wikipedia.org/wiki/Inclination
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using HelixToolkit.Wpf;

namespace SolarsystemDemo
{
    public class Planet3D : TexturedObject3D
    {
        public double Aphelion { get; set; }
        public double Perifelion { get; set; }
        public double SemiMajorAxis { get; set; }
        public double Eccentricity { get; set; }
        public double OrbitalPeriod { get; set; }

        /// <summary>
        /// Inclination to invariable plane
        /// http://en.wikipedia.org/wiki/Inclination
        /// </summary>
        public double Inclination { get; set; }
        public double LongitudeOfAscendingNode { get; set; }
        public double ArgumentOfPerihelion { get; set; }

        /// <summary>
        /// http://en.wikipedia.org/wiki/Axial_tilt
        /// </summary>
        public double AxialTilt { get; set; }

        /// <summary>
        /// http://en.wikipedia.org/wiki/Rotation_period
        /// </summary>
        public double RotationPeriod { get; set; }

        public SolarSystem3D SolarSystem { get; set; }
        public List<Satellite3D> Satellites { get; set; }
        public double DistanceScale { get; set; }
        public double DiameterScale { get; set; }

        TubeVisual3D orbit;
        public Point3D Position { get; set; }

        public Planet3D()
        {
            Satellites = new List<Satellite3D>();

            orbit = new TubeVisual3D() {Diameter=0.8, ThetaDiv = 16 };
            orbit.Material = MaterialHelper.CreateMaterial(null,Brushes.Blue,Brushes.Gray,0.5, 20);
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
            // http://en.wikipedia.org/wiki/Argument_of_periapsis
            //  tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0,0,1),LongitudeOfAscendingNode)));
            tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), Inclination)));
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
                    path.Add(CalculatePosition((double)i / (n - 1) * Math.PI * 2, SolarSystem.DistanceScale));

                orbit.Path = path;
                orbit.UpdateModel();
            }

            Sphere.Radius = MeanRadius / SolarSystem.DiameterScale;
        }

        void UpdatePosition()
        {
            double ang = 0;
            if (OrbitalPeriod > 0)
                ang = SolarSystem.Days / OrbitalPeriod * Math.PI * 2;
            Position = CalculatePosition(ang, SolarSystem.DistanceScale);

            // http://en.wikipedia.org/wiki/Axial_tilt
            // http://en.wikipedia.org/wiki/Rotation_period
            var rotang = SolarSystem.Days / RotationPeriod * 360;
            var axis = new Vector3D(0, 0, 1); // todo: should be normal to orbital plane

            var tg = new Transform3DGroup();
            tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axis, rotang)));
            tg.Children.Add(new TranslateTransform3D(Position.X, Position.Y, Position.Z));
            Sphere.Transform = tg;
        }

        public void InitModel(SolarSystem3D ss)
        {
            SolarSystem = ss;
            UpdateTransform();
            UpdateOrbit();
            foreach (var s in Satellites)
            {
                s.InitModel(this);
                Children.Add(s);
            }
        }

        public void UpdateModel()
        {
            UpdatePosition();
            foreach (var s in Satellites)
                s.UpdateModel();
        }

    }
}