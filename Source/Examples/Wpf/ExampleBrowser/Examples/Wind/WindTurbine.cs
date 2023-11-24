namespace Wind;

public sealed class WindTurbine
{
    // Rotor = Blades + Hub
    public double BladeLength { get; set; }
    public double BladeRootChord { get; set; }
    public double BladeTipChord { get; set; }
    public double HubDiameter { get; set; }
    public int Blades { get; set; }

    // Nacelle
    public double NacelleDiameter { get; set; }
    public double NacelleLength { get; set; }
    public double Yaw { get; set; }

    // Tower
    public double BaseHeight { get; set; }
    public double Height { get; set; }
    public double Diameter { get; set; }
    public double ShaftAngle { get; set; }
    public double Overhang { get; set; }

    public double Pre { get; set; }

    public WindTurbine()
    {
        BladeLength = 40;
        Height = 70;
        BaseHeight = 20;
        HubDiameter = 3;
        Diameter = 4;
        Overhang = 5;
        NacelleLength = 8;
        NacelleDiameter = 2;
        BladeRootChord = 1;
        BladeTipChord = 0.2;
        Blades = 3;

        ShaftAngle = 0.5;
        Pre = -2.5;
    }
}
