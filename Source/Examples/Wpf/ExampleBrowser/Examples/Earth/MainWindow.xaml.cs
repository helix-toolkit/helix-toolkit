using DependencyPropertyGenerator;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

namespace Earth;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Earth", "Shows the earth with textures.")]
[DependencyProperty<Material>("Clouds")]
[DependencyProperty<Vector3D>("SunlightDirection")]
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
        this.view3.Camera!.LookDirection = this.SunlightDirection;
        this.view3.Camera!.Position = new Point3D() - this.view3.Camera.LookDirection;
        this.view3.Title = now.ToString("u");
        this.view3.TextBrush = Brushes.White;
        this.DataContext = this;
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
    private async void LoadImage(string url, Action<Material> setter)
    {
        byte[] result = Array.Empty<byte>();

#if NET6_0_OR_GREATER
        using var client = new HttpClient();
        result = await client.GetByteArrayAsync(url);
#else
        using var client = new WebClient();
        result = await client.DownloadDataTaskAsync(url);
#endif

        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = new MemoryStream(result);
        image.EndInit();

        setter(MaterialHelper.CreateImageMaterial(image, 0.5));
    }
}
