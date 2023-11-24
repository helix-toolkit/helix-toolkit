namespace Flights;

public sealed class Airport
{
    public string? ICAO { get; set; }
    public string? IATA { get; set; }
    public string? AirportName { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }

    public override string ToString()
    {
        return string.Format("{0}, {1} {2} ({3})", AirportName, City, Country, IATA);
    }
}
