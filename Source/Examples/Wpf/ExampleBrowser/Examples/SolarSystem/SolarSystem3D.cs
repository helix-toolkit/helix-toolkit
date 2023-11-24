using System.ComponentModel;
using System.Windows.Media.Media3D;
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SolarSystem;

[ObservableObject]
public sealed partial class SolarSystem3D : ModelVisual3D
{
    public double DistanceScale { get; set; }
    public double DiameterScale { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Time))]
    private double _days = 0;

    private readonly DateTime Time0;

    public DateTime Time
    {
        get { return Time0.AddDays(Days); }
        set { Days = (value - Time0).TotalDays; }
    }

    partial void OnDaysChanged(double value)
    {
        UpdateModel();
    }

    public SolarSystem3D()
    {
        Time0 = DateTime.Now;
    }

    public void InitModel()
    {
        foreach (Planet3D p in Children)
            p.InitModel(this);
    }

    public void UpdateModel()
    {
        foreach (Planet3D p in Children)
            p.UpdateModel();
    }
}
