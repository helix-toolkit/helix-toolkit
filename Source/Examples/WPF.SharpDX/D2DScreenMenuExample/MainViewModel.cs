using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace D2DScreenMenuExample
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);

            // ----------------------------------------------
            // titles
            this.Title = "D2DScreenMenu Demo";
            this.SubTitle = "WPF & SharpDX";

            // ----------------------------------------------
            // camera setup
            this.Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };
        }
    }
}
