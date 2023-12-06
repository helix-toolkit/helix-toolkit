using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
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

namespace Lights;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Lights", "Light models.")]
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = new MainWindowViewModel();
    }

    private partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private int divisions = 20;

        [ObservableProperty]
        private bool enableHeadlight = true;

        [ObservableProperty]
        private double brightness = 0.7;

        [ObservableProperty]
        private double ambient;

        [ObservableProperty]
        private double azimuth = 30;

        [ObservableProperty]
        private double altitude = 60;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GreenMaterial))]
        [NotifyPropertyChangedFor(nameof(BlueMaterial))]
        private double specularBrightness = 0.7;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GreenMaterial))]
        [NotifyPropertyChangedFor(nameof(BlueMaterial))]
        private double specularPower = 30;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Headlight1))]
        [NotifyPropertyChangedFor(nameof(Headlight2))]
        private double headlightLateralPosition = 30;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Headlight1))]
        [NotifyPropertyChangedFor(nameof(Headlight2))]
        private double headlightVerticalPosition = 10;

        public Material GreenMaterial
        {
            get
            {
                return MaterialHelper.CreateMaterial(
                    Brushes.Green,
                    this.SpecularBrightness,
                    this.SpecularPower);
            }
        }

        public Material BlueMaterial
        {
            get
            {
                return MaterialHelper.CreateMaterial(
                    Brushes.DodgerBlue,
                    this.SpecularBrightness,
                    this.SpecularPower);
            }
        }

        public Point3D Headlight1
        {
            get { return new Point3D(this.HeadlightLateralPosition, 0, this.HeadlightVerticalPosition); }
        }

        public Point3D Headlight2
        {
            get { return new Point3D(-this.HeadlightLateralPosition, 0, this.HeadlightVerticalPosition); }
        }
    }
}
