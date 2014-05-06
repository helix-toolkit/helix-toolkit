// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EarthDemo
{
    using System;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Shows the earth with textures.")]
    public partial class MainWindow : Window
    {
        // Clouds
        // http://xplanet.sourceforge.net/clouds.php

        // Planet textures
        // http://planetpixelemporium.com/earth.html
        // http://blogs.msdn.com/b/pantal/archive/2007/08/03/details-on-the-3d-earth-rendering-sample.aspx
        // http://celestia.h-schmidt.net/earth-vt/
        // http://worldwindcentral.com/wiki/Add-on:Global_Clouds_(near_realtime_clouds)
        // http://www.unity3dx.com/index.php/products/earth-3d
        // http://en.wikipedia.org/wiki/DirectDraw_Surface
        // http://www.celestiamotherlode.net/catalog/earth.php
        // http://www.oera.net/How2/TextureMaps2.htm
        /// <summary>
        /// The clouds property.
        /// </summary>
        public static readonly DependencyProperty CloudsProperty = DependencyProperty.Register(
            "Clouds", typeof(Material), typeof(MainWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// The sunlight direction property.
        /// </summary>
        public static readonly DependencyProperty SunlightDirectionProperty =
            DependencyProperty.Register(
                "SunlightDirection", typeof(Vector3D), typeof(MainWindow), new UIPropertyMetadata(new Vector3D()));

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.Clouds = MaterialHelper.CreateImageMaterial("pack://application:,,,/Examples/Earth/clouds.jpg", 0.5);

            // download the latest cloud image
            if (false)
            {
                // todo: the link does not work
                // LoadImage("http://xplanet.sourceforge.net/clouds/clouds_2048.jpg", m => Clouds = m);
            }

            // http://wiki.naturalfrequency.com/wiki/Solar_Position
            // http://www.geoastro.de/elevaz/basics/index.htm
            // http://stjarnhimlen.se/comp/ppcomp.html
            // http://www.daylightmap.com/about.php
            // http://www.sjsu.edu/faculty/watkins/elevsun.htm
            // http://www.physicalgeography.net/fundamentals/6h.html
            // http://www.die.net/earth/
            // http://static.die.net/earth/mercator/1600.jpg
            var now = DateTime.UtcNow;
            var juneSolstice = new DateTime(now.Year, 6, 22);

            // todo: check calculation - this is probably not correct
            var declination = 23.45 * Math.Cos((now.DayOfYear - juneSolstice.DayOfYear) / 365.25 * 2 * Math.PI);
            var phi = -now.Hour / 24.0 * Math.PI * 2;
            var theta = declination / 180 * Math.PI;
            this.SunlightDirection =
                -new Vector3D(Math.Cos(phi) * Math.Cos(theta), Math.Sin(phi) * Math.Cos(theta), Math.Sin(theta));
            this.view3.Camera.LookDirection = this.SunlightDirection;
            this.view3.Camera.Position = new Point3D() - this.view3.Camera.LookDirection;
            this.view3.Title = now.ToString("u");
            this.view3.TextBrush = Brushes.White;
            this.DataContext = this;
        }

        /// <summary>
        /// Gets or sets Clouds.
        /// </summary>
        public Material Clouds
        {
            get
            {
                return (Material)this.GetValue(CloudsProperty);
            }

            set
            {
                this.SetValue(CloudsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SunlightDirection.
        /// </summary>
        public Vector3D SunlightDirection
        {
            get
            {
                return (Vector3D)this.GetValue(SunlightDirectionProperty);
            }

            set
            {
                this.SetValue(SunlightDirectionProperty, value);
            }
        }

        /// <summary>
        /// The load image.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="setter">
        /// The setter.
        /// </param>
        private void LoadImage(string url, Action<Material> setter)
        {
            var client = new WebClient();
            client.DownloadDataCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        return;
                    }

                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(e.Result);
                    image.EndInit();

                    setter(MaterialHelper.CreateImageMaterial(image, 0.5));
                };
            client.DownloadDataAsync(new Uri(url));
        }

    }
}