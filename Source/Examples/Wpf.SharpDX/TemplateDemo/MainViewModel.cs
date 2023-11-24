using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace TemplateDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    public ObservableCollection<SelectionViewModel> ViewModels { get; } = new();

    [ObservableProperty]
    private SelectionViewModel? selectedViewModel = null;

    private readonly PhongMaterialCollection materials = new();

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        CreateViewModels();
    }

    private void CreateViewModels()
    {
        var vm = new SelectionViewModel(nameof(Sphere));
        for (int i = 0; i < 10; ++i)
        {
            vm.Items.Add(new Sphere()
            {
                Transform = new TranslateTransform3D(0, i, 0),
                Material = materials[i]
            });
        }
        ViewModels.Add(vm);

        vm = new SelectionViewModel(nameof(Cube));
        for (int i = 0; i < 10; ++i)
        {
            vm.Items.Add(new Cube()
            {
                Transform = new TranslateTransform3D(i, i, 0),
                Material = materials[i]
            });
        }
        ViewModels.Add(vm);
    }
}
