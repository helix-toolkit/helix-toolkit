using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace MvvmCommand;

public sealed partial class MainViewModel : ObservableObject
{
    public string Title { get; }

    public ObservableCollection<Visual3D> Objects { get; set; }

    public MainViewModel()
    {
        this.Title = "MVVM demo";
        this.Objects = new ObservableCollection<Visual3D>();

        // Initialize with two objects
        this.Add();
        this.Add();
    }

    public bool CanAdd()
    {
        return this.Objects.Count < 10;
    }

    [RelayCommand(CanExecute = nameof(CanAdd))]
    public void Add()
    {
        if (this.Objects.Count == 0)
        {
            this.Objects.Add(new DefaultLights());
        }

        this.Objects.Add(new BoxVisual3D { Center = this.GetRandomPoint() });
    }

    private readonly Random r = new();

    private Point3D GetRandomPoint()
    {
        int d = 10;
        return new Point3D(this.r.Next(d) - d / 2, this.r.Next(d) - d / 2, this.r.Next(d) - d / 2);
    }

    public bool CanRemove()
    {
        return this.Objects.Count > 0;
    }

    [RelayCommand(CanExecute = nameof(CanRemove))]
    public void Remove()
    {
        this.Objects.RemoveAt(this.Objects.Count - 1);
    }
}
