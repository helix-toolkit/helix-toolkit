using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;

namespace D2DScreenMenuExample
{
    public class MainViewModel : BaseViewModel
    {
        public ViewModel3D VM3D { set; get; } = new ViewModel3D();

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
            this.Camera = new PerspectiveCamera { Position = new Media3D.Point3D(8, 9, 7), LookDirection = new Media3D.Vector3D(-5, -12, -5), UpDirection = new Media3D.Vector3D(0, 1, 0) };
        }
    }

    public class ViewModel3D : ObservableObject
    {
        public MeshGeometry3D Model { set; get; }
        public Material ModelMaterial { set; get; } = PhongMaterials.Gray;

        public Vector3 Light1Direction { get; set; } = new Vector3(-1, -1, -1);

        public Color4 Light1Color { set; get; } = Color.White;

        public ViewModel3D()
        {
            var builder = new MeshBuilder(true);
            builder.AddBox(new Vector3(0, 0, 0), 5, 5, 5);
            builder.AddBox(new Vector3(0, 0, 0), 10, 0.1, 10);
            Model = builder.ToMeshGeometry3D();
        }
    }
}
