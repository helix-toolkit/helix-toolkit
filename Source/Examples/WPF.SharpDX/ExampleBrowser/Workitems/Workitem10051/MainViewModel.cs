 namespace Workitem10051
{
    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;

    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            // titles
            this.Title = "Simple Demo (Workitem 10051)";
            this.SubTitle = "LineGeometryModel3D now works with OrthographicCamera and Intel HD 3000.";

            // default render technique
            this.RenderTechnique = Techniques.RenderBlinn;
        }
    }
}
