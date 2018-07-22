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
    using DemoCore;
    using HelixToolkit.Mathematics;
    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Core;
    using SharpDX.Direct3D11;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using Color = System.Windows.Media.Color;
    using Colors = System.Windows.Media.Colors;
    using Matrix = System.Numerics.Matrix4x4;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {
        public Geometry3D DefaultModel { get; private set; }
        public Geometry3D Grid { get; private set; }
        public Geometry3D FloorModel { private set; get; }
        public PhongMaterial DefaultMaterial { get; private set; }
        public PhongMaterial FloorMaterial { get; } = PhongMaterials.Silver;
        public Color GridColor { get; private set; }

        public Transform3D DefaultTransform { get; private set; }
        public Transform3D GridTransform { get; private set; }

        public Vector3D DirectionalLightDirection1 { get; private set; }
        public Vector3D DirectionalLightDirection2 { get; private set; }
        public Vector3D DirectionalLightDirection3 { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }

        private FillMode fillMode = FillMode.Solid;
        public FillMode FillMode
        {
            set
            {
                SetValue(ref fillMode, value);
            }
            get
            {
                return fillMode;
            }
        }

        private bool wireFrame = false;
        public bool Wireframe
        {
            set
            {
                if(SetValue(ref wireFrame, value))
                {
                    if (value)
                    {
                        FillMode = FillMode.Wireframe;
                    }
                    else
                    {
                        FillMode = FillMode.Solid;
                    }
                }
            }
            get
            {
                return wireFrame;
            }
        }

        private MeshTopologyEnum meshTopology = MeshTopologyEnum.PNTriangles;

        public MeshTopologyEnum MeshTopology
        {
            get { return this.meshTopology; }
            set
            {
                /// if topology is changes, reload the model with proper type of faces
                this.meshTopology = value;
                this.LoadModel(@"./Media/teapot_quads_tex.obj", this.meshTopology == MeshTopologyEnum.PNTriangles ? 
                    MeshFaces.Default : MeshFaces.QuadPatches);
            }
        }

        public IList<Matrix> Instances { private set; get; }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];
            // ----------------------------------------------
            // titles
            this.Title = "Hardware Tessellation Demo";
            this.SubTitle = "WPF & SharpDX";

            // ---------------------------------------------
            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(7, 10, 12), LookDirection = new Vector3D(-7, -10, -12), UpDirection = new Vector3D(0, 1, 0) };

            // ---------------------------------------------
            // setup lighting            
            this.AmbientLightColor = Color.FromArgb(1, 12, 12, 12);
            this.DirectionalLightColor = Colors.White;
            this.DirectionalLightDirection1 = new Vector3D(-0, -20, -20);
            this.DirectionalLightDirection2 = new Vector3D(-0, -1, +50);
            this.DirectionalLightDirection3 = new Vector3D(0, +1, 0);

            // ---------------------------------------------
            // model trafo
            this.DefaultTransform = new Media3D.TranslateTransform3D(0, -0, 0);

            // ---------------------------------------------
            // model material
            this.DefaultMaterial = new PhongMaterial
            {
                AmbientColor = Colors.Gray.ToColor4(),
                DiffuseColor = Colors.Red.ToColor4(), // Colors.LightGray,
                SpecularColor = Colors.White.ToColor4(),
                SpecularShininess = 100f,
                DiffuseMap = LoadFileToMemory(new System.Uri(@"./Media/TextureCheckerboard2.dds", System.UriKind.RelativeOrAbsolute).ToString()),
                NormalMap = LoadFileToMemory(new System.Uri(@"./Media/TextureCheckerboard2_dot3.dds", System.UriKind.RelativeOrAbsolute).ToString()),
                EnableTessellation = true, RenderShadowMap = true
            };
            FloorMaterial.RenderShadowMap = true;
            // ---------------------------------------------
            // init model
            this.LoadModel(@"./Media/teapot_quads_tex.obj", this.meshTopology == MeshTopologyEnum.PNTriangles ?
             MeshFaces.Default : MeshFaces.QuadPatches);
            // ---------------------------------------------
            // floor plane grid
            this.Grid = LineBuilder.GenerateGrid(10);
            this.GridColor = Colors.Black;
            this.GridTransform = new Media3D.TranslateTransform3D(-5, -4, -5);

            var builder = new MeshBuilder(true, true, true);
            builder.AddBox(new Vector3(0, -5, 0), 60, 0.5, 60, BoxFaces.All);
            FloorModel = builder.ToMesh();

            Instances = new Matrix[] { Matrix.Identity, MatrixHelper.Translation(10, 0, 10), MatrixHelper.Translation(-10, 0, 10),
                MatrixHelper.Translation(10, 0, -10), MatrixHelper.Translation(-10, 0, -10), };
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
            model.Colors = new Color4Collection(model.Positions.Select(x => new Color4(1, 0, 0, 1)));
            DefaultModel = model;
        }

    }
}