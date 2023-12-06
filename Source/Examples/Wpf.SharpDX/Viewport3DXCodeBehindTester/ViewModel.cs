using CommunityToolkit.Mvvm.ComponentModel;

namespace Viewport3DXCodeBehindTester;

public partial class ViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private bool enableButtons = false;

    [ObservableProperty]
    private bool enableEnvironmentButtons = true;
}
