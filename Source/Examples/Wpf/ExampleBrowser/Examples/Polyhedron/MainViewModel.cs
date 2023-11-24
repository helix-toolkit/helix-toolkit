using System.ComponentModel;
using System.Windows.Media.Media3D;
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Polyhedron;

public sealed partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        CreateModel();
    }

    [ObservableProperty]
    private ModelTypes currentModelType;

    [ObservableProperty]
    private Model3D? model;

    partial void OnCurrentModelTypeChanged(ModelTypes value)
    {
        CreateModel();
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
}
