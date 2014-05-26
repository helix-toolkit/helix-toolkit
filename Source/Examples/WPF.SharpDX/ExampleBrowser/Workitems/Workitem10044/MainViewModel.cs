 namespace Workitem10044
{
    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;

    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            // titles
            this.Title = "Simple Demo (Workitem 10044)";
            this.SubTitle = "Please note that this scene is defined completely in XAML.";

            // default render technique
            this.RenderTechnique = Techniques.RenderBlinn;
        }
    }
}
