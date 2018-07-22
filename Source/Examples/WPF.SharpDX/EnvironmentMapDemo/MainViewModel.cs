// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EnvironmentMapDemo
{
    using DemoCore;
    using HelixToolkit.Mathematics;
    using HelixToolkit.Wpf.SharpDX;
    using Matrix = System.Numerics.Matrix4x4;
    using System.Collections.Generic;
    using System.IO;
    using System.Numerics;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {        
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D Model1 { get; private set; }
        public PhongMaterial ModelMaterial { get; set; }
        public Media3D.Transform3D ModelTransform { get; private set; }
        public Vector3 DirectionalLightDirection { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }

        public List<Matrix> Instances1 { private set; get; } = new List<Matrix>();
        public List<Matrix> Instances2 { private set; get; } = new List<Matrix>();
        public List<Matrix> Instances3 { private set; get; } = new List<Matrix>();
        public PhongMaterial ModelMaterial1 { get; set; }
        public PhongMaterial ModelMaterial2 { get; set; }
        public PhongMaterial ModelMaterial3 { get; set; }
        public Stream SkyboxTexture { private set; get; }

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
            LoadModel("teapot_quads_tex.obj", MeshFaces.Default);
            this.ModelTransform = new Media3D.TranslateTransform3D();
            this.ModelMaterial = PhongMaterials.PolishedSilver;
            this.ModelMaterial.ReflectiveColor = Color.Silver;
            this.ModelMaterial.RenderEnvironmentMap = true;
            var b1 = new MeshBuilder(true);
            b1.AddSphere(new Vector3(0, 0, 0), 1.0, 64, 64);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 3, BoxFaces.All);
            this.Model1 = b1.ToMeshGeometry3D();

            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];

            SkyboxTexture = LoadFileToMemory("Cubemap_Grandcanyon.dds");
            int t = 5;
            for (int i = 0; i < 10; ++i)
            {
                Instances1.Add(Matrix.CreateTranslation(new Vector3(t, t, (i - 5) * t)));
            }
            for (int i = 0; i < 10; ++i)
            {
                Instances2.Add(Matrix.CreateTranslation(new Vector3(t, (i - 5) * t, t)));
            }
            for (int i = 0; i < 10; ++i)
            {
                Instances3.Add(Matrix.CreateTranslation(new Vector3(-(i - 5) * t, t, (i - 5) * t)));
            }
            //int t = 5;
            //Instances.Add(Matrix.Translation(new Vector3(t, t, t)));
            //Instances.Add(Matrix.Translation(new Vector3(-t, t, t)));
            //Instances.Add(Matrix.Translation(new Vector3(-t, -t, t)));
            //Instances.Add(Matrix.Translation(new Vector3(-t, -t, -t)));
            //Instances.Add(Matrix.Translation(new Vector3(t, -t, t)));
            //Instances.Add(Matrix.Translation(new Vector3(t, -t, -t)));
            //Instances.Add(Matrix.Translation(new Vector3(-t, t, -t)));
            //Instances.Add(Matrix.Translation(new Vector3(t, t, -t)));
            this.ModelMaterial1 = PhongMaterials.Red;
            this.ModelMaterial1.AmbientColor = Color.Red;
            this.ModelMaterial1.RenderEnvironmentMap = true;
            this.ModelMaterial2 = PhongMaterials.Green;
            this.ModelMaterial2.AmbientColor = Color.Green;
            this.ModelMaterial2.RenderEnvironmentMap = true;
            this.ModelMaterial3 = PhongMaterials.Blue;
            this.ModelMaterial3.AmbientColor = Color.Blue;
            this.ModelMaterial3.RenderEnvironmentMap = true;
        }

        /// <summary>
        /// load the model from obj-file
        /// </summary>
        /// <param name="filename">filename</param>
        /// <param name="faces">Determines if facades should be treated as triangles (Default) or as quads (Quads)</param>
        private void LoadModel(string filename, MeshFaces faces)
        {
            // load model
            var reader = new ObjReader();
            var objModel = reader.Read(filename, new ModelInfo() { Faces = faces });
            var model = objModel[0].Geometry as MeshGeometry3D;
            Model = model;
        }
    }
}