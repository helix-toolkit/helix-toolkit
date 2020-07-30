// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightsDemo
{
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    using PropertyTools;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Light models.")]
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }

        private class MainWindowViewModel : Observable
        {
            private int divisions = 20;
            private bool enableHeadlight = true;
            private double brightness = 0.7;
            private double ambient;
            private double azimuth = 30;
            private double altitude = 60;
            private double specularBrightness = 0.7;
            private double specularPower = 30;
            private double headlightLateralPosition = 30;
            private double headlightVerticalPosition = 10;

            public double Ambient
            {
                get { return this.ambient; }
                set { this.SetValue(ref this.ambient, value, nameof(this.Ambient)); }
            }

            public double Azimuth
            {
                get { return this.azimuth; }
                set { this.SetValue(ref this.azimuth, value, nameof(this.Azimuth)); }
            }

            public double Altitude
            {
                get { return this.altitude; }
                set { this.SetValue(ref this.altitude, value, nameof(this.Altitude)); }
            }

            public int Divisions
            {
                get { return this.divisions; }
                set { this.SetValue(ref this.divisions, value, nameof(this.Divisions)); }
            }

            public double Brightness
            {
                get { return this.brightness; }
                set { this.SetValue(ref this.brightness, value, nameof(this.Brightness)); }
            }

            public bool EnableHeadlight
            {
                get { return this.enableHeadlight; }
                set { this.SetValue(ref this.enableHeadlight, value, nameof(this.EnableHeadlight)); }
            }

            public double SpecularBrightness
            {
                get { return this.specularBrightness; }
                set { this.SetValue(ref this.specularBrightness, value, nameof(this.SpecularBrightness)); RaisePropertyChanged(nameof(this.GreenMaterial)); RaisePropertyChanged(nameof(this.BlueMaterial)); }
            }

            public double SpecularPower
            {
                get { return this.specularPower; }
                set { this.SetValue(ref this.specularPower, value, nameof(this.SpecularPower)); RaisePropertyChanged(nameof(this.GreenMaterial)); RaisePropertyChanged(nameof(this.BlueMaterial)); }
            }

            public double HeadlightLateralPosition
            {
                get { return this.headlightLateralPosition; }
                set { this.SetValue(ref this.headlightLateralPosition, value, nameof(this.HeadlightLateralPosition)); RaisePropertyChanged(nameof(this.Headlight1)); RaisePropertyChanged(nameof(this.Headlight2)); }
            }

            public double HeadlightVerticalPosition
            {
                get { return this.headlightVerticalPosition; }
                set { this.SetValue(ref this.headlightVerticalPosition, value, nameof(this.HeadlightVerticalPosition)); RaisePropertyChanged(nameof(this.Headlight1)); RaisePropertyChanged(nameof(this.Headlight2)); }
            }

            public Material GreenMaterial
            {
                get
                {
                    return MaterialHelper.CreateMaterial(
                        Brushes.Green,
                        this.specularBrightness,
                        this.specularPower);
                }
            }

            public Material BlueMaterial
            {
                get
                {
                    return MaterialHelper.CreateMaterial(
                        Brushes.DodgerBlue,
                        this.specularBrightness,
                        this.specularPower);
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
}