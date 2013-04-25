// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace LorenzAttractorDemo
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private double beta;
        private bool directionArrows;
        private Model3D model;
        private double rho;
        private double sigma;

        public MainViewModel()
        {
            rho = 28;
            sigma = 10;
            beta = 8.0 / 3;
            directionArrows = true;

            UpdateModel();
        }

        public bool DirectionArrows
        {
            get { return directionArrows; }
            set
            {
                if (directionArrows != value)
                {
                    directionArrows = value;
                    RaisePropertyChanged("DirectionArrows");
                    UpdateModel();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Rayleigh number.
        /// http://en.wikipedia.org/wiki/Rayleigh_number
        /// </summary>
        /// <value>The rho.</value>
        public double Rho
        {
            get { return rho; }
            set
            {
                if (rho != value)
                {
                    rho = value;
                    RaisePropertyChanged("Rho");
                    UpdateModel();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Prandtl number.
        /// http://en.wikipedia.org/wiki/Prandtl_number
        /// </summary>
        /// <value>The sigma.</value>
        public double Sigma
        {
            get { return sigma; }
            set
            {
                if (sigma != value)
                {
                    sigma = value;
                    RaisePropertyChanged("Sigma");
                    UpdateModel();
                }
            }
        }

        public double Beta
        {
            get { return beta; }
            set
            {
                if (beta != value)
                {
                    beta = value;
                    RaisePropertyChanged("Beta");
                    UpdateModel();
                }
            }
        }

        public Model3D Model
        {
            get { return model; }
            set
            {
                model = value;
                RaisePropertyChanged("Model");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        private void UpdateModel()
        {
            // http://en.wikipedia.org/wiki/Lorenz_attractor
            Func<double[], double[]> lorenzAttractor = x =>
                                                           {
                                                               var dx = new double[3];
                                                               dx[0] = sigma * (x[1] - x[0]);
                                                               dx[1] = x[0] * (rho - x[2]) - x[1];
                                                               dx[2] = x[0] * x[1] - beta * x[2];
                                                               return dx;
                                                           };
            // solve the ODE
            var x0 = new[] { 0, 1, 1.05 };
            IEnumerable<double[]> solution = EulerSolver(lorenzAttractor, x0, 25);
            // todo: should have a better ODE solver (R-K(4,5)? http://www.mathworks.com/help/techdoc/ref/ode45.html)
            List<Point3D> path = solution.Select(x => new Point3D(x[0], x[1], x[2])).ToList();

            // create the WPF3D model
            var m = new Model3DGroup();
            var gm = new MeshBuilder();
            gm.AddTube(path, 0.8, 10, false);
            if (directionArrows)
            {
                // sphere at the initial point
                gm.AddSphere(path[0], 1);
                // arrow heads every 100 point
                for (int i = 100; i + 1 < path.Count; i += 100)
                {
                    gm.AddArrow(path[i], path[i + 1], 0.8);
                }
                // arrow head at the end
                Point3D p0 = path[path.Count - 2];
                Point3D p1 = path[path.Count - 1];
                var d = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
                d.Normalize();
                Point3D p2 = p1 + d * 2;
                gm.AddArrow(p1, p2, 0.8);
            }

            m.Children.Add(new GeometryModel3D(gm.ToMesh(), Materials.Gold));

            Model = m;
        }

        private static IEnumerable<double[]> EulerSolver(Func<double[], double[]> dx, double[] x0, double tSpan)
        {
            var results = new List<double[]> { x0 };
            double t = 0;
            const double h = 0.008;
            int j = 0;
            while (t < tSpan)
            {
                t += h;
                double[] xprime = dx(results[j]);
                var x = new double[3];
                for (int i = 0; i < 3; i++)
                    x[i] = results[j][i] + xprime[i] * h;
                j++;
                results.Add(x);
            }
            return results;
        }

        private static IEnumerable<double[]> RungeKutta45Solver(Func<double[], double[]> dx, double[] x0, double tSpan)
        {
            // http://math.fullerton.edu/mathews/n2003/RungeKuttaFehlbergMod.html
            // http://www.trentfguidry.net/post/2009/10/09/Runge-Kutta-Fehlberg.aspx

            throw new NotImplementedException();
        }
    }
}