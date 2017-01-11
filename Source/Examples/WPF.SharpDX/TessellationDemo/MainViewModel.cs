// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   load the model from obj-file
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TessellationDemo
{
    using System.Linq;
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using System.Windows.Media.Imaging;
    using HelixToolkit.Wpf.SharpDX.Core;
    using System.IO;

    public class MainViewModel : BaseViewModel
    {
        public Geometry3D DefaultModel { get; private set; }
        public Geometry3D Lines { get; private set; }
        public Geometry3D Grid { get; private set; }

        public PhongMaterial DefaultMaterial { get; private set; }
        public SharpDX.Color GridColor { get; private set; }

        public Transform3D DefaultTransform { get; private set; }
        public Transform3D GridTransform { get; private set; }

        public Vector3 DirectionalLightDirection1 { get; private set; }
        public Vector3 DirectionalLightDirection2 { get; private set; }
        public Vector3 DirectionalLightDirection3 { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }

        public string[] MeshTopologyList { get; set; }

        private string meshTopology = MeshFaces.Default.ToString();
        private RenderTechnique pnQuads;
        private RenderTechnique pnTriangles;

        public string MeshTopology
        {
            get { return this.meshTopology; }
            set
            {
                /// if topology is changes, reload the model with proper type of faces
                this.meshTopology = value;
                this.RenderTechnique = this.meshTopology == "Quads" ?
                    RenderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNQuads] :
                    RenderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNTriangles];
                this.LoadModel(@"./Media/teapot_quads_tex.obj", this.meshTopology == "Quads" ? MeshFaces.QuadPatches : MeshFaces.Default);
            }
        }

        public MainViewModel()
        {
            RenderTechniquesManager = new TessellationTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNQuads];
            EffectsManager = new TessellationEffectsManager(RenderTechniquesManager);

            // ----------------------------------------------
            // titles
            this.Title = "Hardware Tessellation Demo";
            this.SubTitle = "WPF & SharpDX";

            // ---------------------------------------------
            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(7, 10, 12), LookDirection = new Vector3D(-7, -10, -12), UpDirection = new Vector3D(0, 1, 0) };

            // ---------------------------------------------
            // setup lighting            
            this.AmbientLightColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
            this.DirectionalLightColor = Color.White;
            this.DirectionalLightDirection1 = new Vector3(-0, -50, -20);
            this.DirectionalLightDirection2 = new Vector3(-0, -1, +50);
            this.DirectionalLightDirection3 = new Vector3(0, +1, 0);

            // ---------------------------------------------
            // model trafo
            this.DefaultTransform = new Media3D.TranslateTransform3D(0, -0, 0);

            // ---------------------------------------------
            // model material
            this.DefaultMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f), // Colors.LightGray,
                SpecularColor = Color.White,
                SpecularShininess = 100f,
                DiffuseMap = new FileStream(new System.Uri(@"./Media/TextureCheckerboard2.dds", System.UriKind.RelativeOrAbsolute).ToString(), FileMode.Open),
                NormalMap = new FileStream(new System.Uri(@"./Media/TextureCheckerboard2_dot3.dds", System.UriKind.RelativeOrAbsolute).ToString(), FileMode.Open),
                //DisplacementMap = new BitmapImage(new Uri(@"path", UriKind.RelativeOrAbsolute)),                
            };

            // ---------------------------------------------
            // init model
            this.MeshTopologyList = new[] { "Triangles", "Quads" };
            this.MeshTopology = "Triangles";

            // ---------------------------------------------
            // floor plane grid
            this.Grid = LineBuilder.GenerateGrid();
            this.GridColor = SharpDX.Color.Black;
            this.GridTransform = new Media3D.TranslateTransform3D(-5, -4, -5);
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
            this.DefaultModel = objModel[0].Geometry as MeshGeometry3D;
            this.DefaultModel.Colors = new Color4Collection(this.DefaultModel.Positions.Select(x => new Color4(1, 0, 0, 1)));
        }

    }
}