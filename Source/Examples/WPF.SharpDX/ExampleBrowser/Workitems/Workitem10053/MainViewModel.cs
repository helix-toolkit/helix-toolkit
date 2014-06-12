 namespace Workitem10053
{
    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;

    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            // titles
            this.Title = "Simple Demo (Workitem 10053)";
            this.SubTitle = "You can pan, rotate and zoom via touch.";

            // default render technique
            this.RenderTechnique = Techniques.RenderBlinn;
        }
    }
}
