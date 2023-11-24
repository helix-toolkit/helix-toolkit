using HelixToolkit.SharpDX;

namespace Workitem10044;

public class MainViewModel : DemoCore.BaseViewModel
{
    public MainViewModel()
    {
        // titles
        this.Title = "Simple Demo (Workitem 10044)";
        this.SubTitle = "Please note that this scene is defined completely in XAML.";

        EffectsManager = new DefaultEffectsManager();
    }
}
