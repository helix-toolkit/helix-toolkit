// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PolyhedronDemo
{
    using System;

    public enum ModelTypes { Tetrahedron, Hexahedron, Octahedron, Icosahedron, Dodecahedron, StellatedOctahedron }

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            CreateModel();
        }

        private ModelTypes currentModelType;
        public ModelTypes CurrentModelType
        {
            get { return currentModelType; }
            set
            {
                currentModelType = value; RaisePropertyChanged("CurrentModelType");
                CreateModel();
            }
        }

        private void CreateModel()
        {
            // http://paulbourke.net/geometry/platonic/
            // http://en.wikipedia.org/wiki/Compound_of_two_tetrahedra

            var pmb = new PanelModelBuilder();
            switch (CurrentModelType)
            {
                case ModelTypes.Tetrahedron:
                    {
                        double a = 0.5;
                        pmb.AddPanel(a, a, a, -a, a, -a, a, -a, -a);
                        pmb.AddPanel(-a, a, -a, -a, -a, a, a, -a, -a);
                        pmb.AddPanel(a, a, a, a, -a, -a, -a, -a, a);
                        pmb.AddPanel(a, a, a, -a, -a, a, -a, a, -a);
                        break;
                    }

                case ModelTypes.Octahedron:
                    {
                        double a = 1.0 / (2 * Math.Sqrt(2));
                        double b = 0.5;
                        pmb.AddPanel(-a, 0, a, -a, 0, -a, 0, b, 0);
                        pmb.AddPanel(-a, 0, -a, a, 0, -a, 0, b, 0);
                        pmb.AddPanel(a, 0, -a, a, 0, a, 0, b, 0);
                        pmb.AddPanel(a, 0, a, -a, 0, a, 0, b, 0);
                        pmb.AddPanel(a, 0, -a, -a, 0, -a, 0, -b, 0);
                        pmb.AddPanel(-a, 0, -a, -a, 0, a, 0, -b, 0);
                        pmb.AddPanel(a, 0, a, a, 0, -a, 0, -b, 0);
                        pmb.AddPanel(-a, 0, a, a, 0, a, 0, -b, 0);
                        break;
                    }
                case ModelTypes.Hexahedron:
                    {
                        double a = 0.5;
                        pmb.AddPanel(-a, -a, a, a, -a, a, a, -a, -a, -a, -a, -a);
                        pmb.AddPanel(-a, a, -a, -a, a, a, -a, -a, a, -a, -a, -a);
                        pmb.AddPanel(-a, a, a, a, a, a, a, -a, a, -a, -a, a);
                        pmb.AddPanel(a, a, -a, a, a, a, -a, a, a, -a, a, -a);
                        pmb.AddPanel(a, -a, a, a, a, a, a, a, -a, a, -a, -a);
                        pmb.AddPanel(a, -a, -a, a, a, -a, -a, a, -a, -a, -a, -a);
                        break;
                    }
                case ModelTypes.Icosahedron:
                    {
                        double phi = (1 + Math.Sqrt(5)) / 2;
                        double a = 0.5;
                        double b = 1.0 / (2 * phi);
                        pmb.AddPanel(0, b, -a, b, a, 0, -b, a, 0);
                        pmb.AddPanel(0, b, a, -b, a, 0, b, a, 0);
                        pmb.AddPanel(0, b, a, 0, -b, a, -a, 0, b);
                        pmb.AddPanel(0, b, a, a, 0, b, 0, -b, a);
                        pmb.AddPanel(0, b, -a, 0, -b, -a, a, 0, -b);
                        pmb.AddPanel(0, b, -a, -a, 0, -b, 0, -b, -a);
                        pmb.AddPanel(0, -b, a, b, -a, 0, -b, -a, 0);
                        pmb.AddPanel(0, -b, -a, -b, -a, 0, b, -a, 0);
                        pmb.AddPanel(-b, a, 0, -a, 0, b, -a, 0, -b);
                        pmb.AddPanel(-b, -a, 0, -a, 0, -b, -a, 0, b);
                        pmb.AddPanel(b, a, 0, a, 0, -b, a, 0, b);
                        pmb.AddPanel(b, -a, 0, a, 0, b, a, 0, -b);
                        pmb.AddPanel(0, b, a, -a, 0, b, -b, a, 0);
                        pmb.AddPanel(0, b, a, b, a, 0, a, 0, b);
                        pmb.AddPanel(0, b, -a, -b, a, 0, -a, 0, -b);
                        pmb.AddPanel(0, b, -a, a, 0, -b, b, a, 0);
                        pmb.AddPanel(0, -b, -a, -a, 0, -b, -b, -a, 0);
                        pmb.AddPanel(0, -b, -a, b, -a, 0, a, 0, -b);
                        pmb.AddPanel(0, -b, a, -b, -a, 0, -a, 0, b);
                        pmb.AddPanel(0, -b, a, a, 0, b, b, -a, 0);
                        break;
                    }
                case ModelTypes.Dodecahedron:
                    {
                        double phi = (1 + Math.Sqrt(5)) / 2;
                        double a = 0.5;
                        double b = 0.5 / phi;
                        double c = 0.5 * (2 - phi);
                        pmb.AddPanel(c, 0, a, -c, 0, a, -b, b, b, 0, a, c, b, b, b);
                        pmb.AddPanel(-c, 0, a, c, 0, a, b, -b, b, 0, -a, c, -b, -b, b);
                        pmb.AddPanel(c, 0, -a, -c, 0, -a, -b, -b, -b, 0, -a, -c, b, -b, -b);
                        pmb.AddPanel(-c, 0, -a, c, 0, -a, b, b, -b, 0, a, -c, -b, b, -b);
                        pmb.AddPanel(b, b, -b, a, c, 0, b, b, b, 0, a, c, 0, a, -c);

                        pmb.AddPanel(-b, b, b, -a, c, 0, -b, b, -b, 0, a, -c, 0, a, c);
                        pmb.AddPanel(-b, -b, -b, -a, -c, 0, -b, -b, b, 0, -a, c, 0, -a, -c);

                        pmb.AddPanel(b, -b, b, a, -c, 0, b, -b, -b, 0, -a, -c, 0, -a, c);
                        pmb.AddPanel(a, c, 0, a, -c, 0, b, -b, b, c, 0, a, b, b, b);
                        pmb.AddPanel(a, -c, 0, a, c, 0, b, b, -b, c, 0, -a, b, -b, -b);
                        pmb.AddPanel(-a, c, 0, -a, -c, 0, -b, -b, -b, -c, 0, -a, -b, b, -b);
                        pmb.AddPanel(-a, -c, 0, -a, c, 0, -b, b, b, -c, 0, a, -b, -b, b);
                        break;
                    }
                case ModelTypes.StellatedOctahedron:
                    {
                        double a = 0.5;
                        pmb.AddPanel(a, a, a, -a, a, -a, a, -a, -a);
                        pmb.AddPanel(-a, a, -a, -a, -a, a, a, -a, -a);
                        pmb.AddPanel(a, a, a, a, -a, -a, -a, -a, a);
                        pmb.AddPanel(a, a, a, -a, -a, a, -a, a, -a);
                        pmb.AddPanel(-a, a, a, a, a, -a, -a, -a, -a);
                        pmb.AddPanel(a, a, -a, a, -a, a, -a, -a, -a);
                        pmb.AddPanel(-a, a, a, -a, -a, -a, a, -a, a);
                        pmb.AddPanel(-a, a, a, a, -a, a, a, a, -a);
                        break;
                    }

            }
            Model = pmb.ToModel3D();
        }

        private Model3D model;
        public Model3D Model
        {
            get { return model; }
            set { model = value; RaisePropertyChanged("Model"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}