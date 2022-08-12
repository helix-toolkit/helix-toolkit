using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;

namespace ModelViewer.ViewModels;

public class MainViewModel : ObservableRecipient
{
    public IEffectsManager EffectsManager
    {
        get;
    } = new DefaultEffectsManager();

    public Camera Camera
    {
        get;
    } = new OrthographicCamera();

    public MainViewModel()
    {

    }
}

