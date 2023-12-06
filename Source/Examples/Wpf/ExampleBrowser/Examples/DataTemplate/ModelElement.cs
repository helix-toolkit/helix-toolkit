using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace DataTemplate;

[ObservableObject]
public partial class ModelElement : Element
{
    [ObservableProperty]
    private bool isVisible = true;

    public Model3D? Model { get; set; }
}
