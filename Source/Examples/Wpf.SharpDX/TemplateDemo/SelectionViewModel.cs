using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TemplateDemo;

public partial class SelectionViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    public ObservableCollection<Shape> Items { get; } = new();

    public SelectionViewModel(string name)
    {
        Name = name;
    }
}
