 namespace Workitem10048
{
    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;

    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            // titles
            this.Title = "Simple Demo (Workitem 10048 and 10052)";
            this.SubTitle = "Select lines with left mouse button.\nRotate or zoom around a point on a line if the cursor is above one.";

            // default render technique
            this.RenderTechnique = Techniques.RenderBlinn;
        }
    }
}
