using HelixToolkit.Wpf;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Flights;

// http://www.sasems.port.se/Emissioncalc.cfm
// http://www.partow.net/miscellaneous/airportdatabase/

public sealed class FlightVisual3D : ModelVisual3D
{
    public const double EarthRadius = 6371; // km
    public const double DefaultCruisingAltitude = 300; // km  // real value is 12.5...
    public const double DefaultTakeoffLength = 200; // km   // real value is much less..
    public const double DefaultCruisingSpeed = 890; // km/h
    public double CabinFactor { get; set; }

    /// <summary>
    /// Gets the estimated CO2 emission per passenger for the given cabin factor.
    /// </summary>
    public double CO2
    {
        get
        {
            return 24 + Distance * 0.0535; // km
        }
    }

    public FlightVisual3D(Point3D p1, Point3D p2)
    {
        var tube = new TubeVisual3D
        {
            // Material = MaterialHelper.CreateMaterial(Color.FromArgb(80, 255, 255, 255)), // Materials.Yellow;
            Fill = new SolidColorBrush(Color.FromArgb(80, 255, 255, 255))
        };
        Children.Add(tube);
        Children.Add(new SphereVisual3D() { Center = p1, Radius = 100, Material = Materials.Green });
        Children.Add(new SphereVisual3D() { Center = p2, Radius = 100, Material = Materials.Red });

        PointToLatLon(p1, out double lat1, out double lon1);
        PointToLatLon(p2, out double lat2, out double lon2);
        From = string.Format("{0:0.00} {1:0.00}", lat1, lon1);
        To = string.Format("{0:0.00} {1:0.00}", lat2, lon2);

        CruisingSpeed = DefaultCruisingSpeed;
        double cruisingRadius = EarthRadius + DefaultCruisingAltitude;
        double takeoffLength = DefaultTakeoffLength;
        double groundRadius = EarthRadius;
        const double tubeDiameter = 60;

        var o = new Point3D(0, 0, 0);
        var v1 = p1 - o;
        var v2 = p2 - o;
        var z = Vector3D.CrossProduct(v1, v2);
        var x = v1;
        var y = Vector3D.CrossProduct(x, z);
        x.Normalize();
        y.Normalize();
        double v2X = Vector3D.DotProduct(v2, x);
        double v2Y = Vector3D.DotProduct(v2, y);
        double v2A = Math.Atan2(v2Y, v2X);

        const int n = 100;

        var pts = new Point3DCollection();

        double da = v2A / (n - 1);

        double distance = cruisingRadius * Math.Abs(v2A);
        double landingLength = takeoffLength;

        double l = 0;
        for (int i = 0; i < n; i++)
        {
            double a = i * da;
            Vector3D v = x * Math.Cos(a) + y * Math.Sin(a);
            double r = cruisingRadius;

            //if (l < takeoffLength)
            //{
            //    r = groundRadius + Math.Sin(Math.PI/2*l/takeoffLength)*(cruisingRadius - groundRadius);
            //}
            //if (l > distance - landingLength)
            //{
            //    r = groundRadius + Math.Sin(Math.PI/2*(distance - l)/takeoffLength)*(cruisingRadius - groundRadius);
            //}

            r = groundRadius + Math.Sin(Math.PI * i / (n - 1)) * (cruisingRadius - groundRadius);

            var p = o + v * r;
            //  Children.Add(new SphereVisual3D() { Center = p, Radius = 60, Material = Materials.Gray});

            pts.Add(p);
            l += Math.Abs(cruisingRadius * da);
        }
        tube.Diameter = tubeDiameter;
        tube.ThetaDiv = 16;
        tube.Path = pts;
        Distance = distance;
    }

    public string? From { get; set; }
    public string? To { get; set; }

    public string Time
    {
        get
        {
            double totalHours = Distance / CruisingSpeed;
            double hours = (int)totalHours;
            var minutes = (int)Math.Round((totalHours - hours) * 60);
            return string.Format("{0}h{1:00}m", hours, minutes);
        }
    }

    public double CruisingSpeed { get; set; }
    public double Distance { get; set; }

    public static void PointToLatLon(Point3D pt, out double lat, out double lon)
    {
        lon = Math.Atan2(pt.Y, pt.X) * 180 / Math.PI;
        lon += 180;
        if (lon > 180) lon -= 360;
        if (lon < -180) lon += 360;
        double a = Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y);
        lat = Math.Atan2(pt.Z, a) * 180 / Math.PI;
    }

    public static Point3D LatLonToPoint(double latitude, double longitude)
    {
        longitude -= 180;
        latitude = latitude / 180 * Math.PI;
        longitude = longitude / 180 * Math.PI;
        return new Point3D(EarthRadius * Math.Cos(latitude) * Math.Cos(longitude), EarthRadius * Math.Cos(latitude) * Math.Sin(longitude), EarthRadius * Math.Sin(latitude));
    }

    public override string ToString()
    {
        return string.Format("Distance: {0:0} km, Time: {1}", Distance, Time);
    }
}
