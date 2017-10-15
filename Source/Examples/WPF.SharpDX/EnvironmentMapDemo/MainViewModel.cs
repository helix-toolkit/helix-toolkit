// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EnvironmentMapDemo
{
    using DemoCore;
    using HelixToolkit.Wpf;
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {        
        public MeshGeometry3D Model { get; private set; }
        public PhongMaterial ModelMaterial { get; set; }
        public Media3D.Transform3D ModelTransform { get; private set; }
        public Vector3 DirectionalLightDirection { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }

        public MainViewModel()
        {
            this.Title = "Environment Mapping Demo";
            this.SubTitle = "HelixToolkitDX";                        

            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(10, 0, 0), LookDirection = new Vector3D(-10, 0, 0), UpDirection = new Vector3D(0, 1, 0) };
            //this.Camera = new OrthographicCamera { Position = new Point3D(3, 3, 5), LookDirection = new Vector3D(-3, -3, -5), UpDirection = new Vector3D(0, 1, 0) };

            // lighting setup            
            this.AmbientLightColor = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
            this.DirectionalLightColor = Color.White;
            this.DirectionalLightDirection = new Vector3(-2, -1, 1);

            // scene model3d
            var b1 = new MeshBuilder(true);
            b1.AddSphere(new Vector3(0, 0, 0), 1.0, 64, 64);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 3, BoxFaces.All);

            this.Model = b1.ToMeshGeometry3D();
            this.ModelTransform = new Media3D.TranslateTransform3D();
            this.ModelMaterial = PhongMaterials.Copper;

            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
        }
    }



}