using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Flights;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Flights", "Shows great circles and calculates distances between airports.")]
public partial class MainWindow : Window
{
    private readonly List<Point3D> points = new();
    public IList<Airport> Airports { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        earth.Radius = FlightVisual3D.EarthRadius;
        Loaded += MainWindowLoaded;
        Flights = new ObservableCollection<FlightVisual3D>();

        var doc = new CsvDocument<Airport>();
        doc.Load(Application.GetResourceStream(new Uri("pack://application:,,,/Examples/Flights/airports.csv")).Stream);
        Airports = doc.Items;

        DataContext = this;
    }

    public ObservableCollection<FlightVisual3D> Flights { get; set; }

    private void MainWindowLoaded(object? sender, RoutedEventArgs e)
    {
        view1.ZoomExtents();
    }

    private void OnMouseDown(object? sender, MouseButtonEventArgs e)
    {
        var pt = view1.FindNearestPoint(e.GetPosition(view1));
        if (pt.HasValue)
            points.Add(pt.Value);

        if (points.Count > 1)
        {
            var ftv = new FlightVisual3D(points[0], points[1]);
            view1.Children.Add(ftv);
            Flights.Add(ftv);
            points.Clear();
            text1.Text = ftv.ToString();

            // todo:
            // zoom the flight path into the view without changing
            // camera target

            //var bounds = Visual3DHelper.FindBounds(ftv, Transform3D.Identity);
            //view1.ZoomExtents(bounds);
            //view1.Camera.Position=new Point3D()-view1.Camera.LookDirection;
            //view1.ZoomExtents(bounds,0,false);
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        var pt = view1.FindNearestPoint(e.GetPosition(view1));
        if (pt.HasValue)
        {
            double lat, lon;
            FlightVisual3D.PointToLatLon(pt.Value, out lat, out lon);
            text2.Text = String.Format("Lat: {0:0.00} Lon: {1:0.00}", lat, lon);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape)
        {
            foreach (var v in Flights)
                view1.Children.Remove(v);
            Flights.Clear();
            points.Clear();
        }
    }

    private void ListViewKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            var itemsToDelete = list1.SelectedItems.Cast<FlightVisual3D>().ToList();

            foreach (var f in itemsToDelete)
            {
                view1.Children.Remove(f);
                Flights.Remove(f);
            }
        }
    }

    private void ToPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var a = FindAirport(From.Text);
            var b = FindAirport(To.Text);
            if (a != null && b != null)
            {
                AddFlight(a, b);
                // From.Text = a.IATA;
                // To.Text = b.IATA;
            }

            To.Text = "";
            // To.SelectAll();
        }
    }

    private void AddFlight(Airport a, Airport b)
    {
        var from = FlightVisual3D.LatLonToPoint(a.Latitude, a.Longitude);
        var to = FlightVisual3D.LatLonToPoint(b.Latitude, b.Longitude);
        FlightVisual3D.PointToLatLon(from, out double lat, out double lon);
        var ftv = new FlightVisual3D(from, to) { From = a.City, To = b.City };
        view1.Children.Add(ftv);
        Flights.Add(ftv);
    }

    private void FromPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var a = FindAirport(From.Text);
            if (a != null)
            {
                From.Text = a.IATA; // +" " + a.AirportName + " " + a.City;
                To.Focus();
            }
        }
    }

    private Airport[] FindAirports(string text)
    {
        text = text.ToUpper().Trim();
        var result = new List<Airport>();

        foreach (var a in Airports)
        {
            if (a.Latitude == 0 && a.Longitude == 0)
                continue;
            if (a.IATA == text)
                result.Add(a);
        }

        foreach (var a in Airports)
        {
            if (a.Latitude == 0 && a.Longitude == 0)
                continue;
            var name = a.ToString().ToUpper();
            if (name.Contains(text) && !result.Contains(a))
                result.Add(a);
        }

        return result.ToArray();
    }

    private Airport? FindAirport(string text)
    {
        var result = FindAirports(text);
        if (result.Length > 0)
            return result[0];
        return null;
    }

    private void FromToChanged(object? sender, KeyEventArgs keyEventArgs)
    {
        if (sender is ComboBox box)
        {
            var result = FindAirports(box.Text);
            box.ItemsSource = result.Length < 1000 ? result : null;
        }
    }
}
