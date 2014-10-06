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
    using ExampleBrowser;

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

            public double Ambient
            {
                get { return this.ambient; }
                set { this.SetValue(ref this.ambient, value, () => this.Ambient); }
            }

            public double Azimuth
            {
                get { return this.azimuth; }
                set { this.SetValue(ref this.azimuth, value, () => this.Azimuth); }
            }

            public double Altitude
            {
                get { return this.altitude; }
                set { this.SetValue(ref this.altitude, value, () => this.Altitude); }
            }

            public int Divisions
            {
                get { return this.divisions; }
                set { this.SetValue(ref this.divisions, value, () => this.Divisions); }
            }

            public double Brightness
            {
                get { return this.brightness; }
                set { this.SetValue(ref this.brightness, value, () => this.Brightness); }
            }

            public bool EnableHeadlight
            {
                get { return this.enableHeadlight; }
                set { this.SetValue(ref this.enableHeadlight, value, () => this.EnableHeadlight); }
            }

        }
    }
}