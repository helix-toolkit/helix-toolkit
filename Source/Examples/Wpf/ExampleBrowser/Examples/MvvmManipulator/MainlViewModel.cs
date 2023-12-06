using CommunityToolkit.Mvvm.ComponentModel;

namespace MvvmManipulator;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private double translateValue;

    [ObservableProperty]
    private double rotateValue;
}
