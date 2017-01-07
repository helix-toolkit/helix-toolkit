using DemoCore;
using HelixToolkit.SharpDX.Shared.Utilities;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Media3D = System.Windows.Media.Media3D;

namespace OctreeDemo
{    
    public class MainViewModel : BaseViewModel
    {
        private Vector3 light1Direction = new Vector3();
        public Vector3 Light1Direction
        {
            set
            {
                if (light1Direction != value)
                {
                    light1Direction = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return light1Direction;
            }
        }
        private FillMode fillMode = FillMode.Solid;
        public FillMode FillMode
        {
            set
            {
                fillMode = value;
                OnPropertyChanged();
            }
            get
            {
                return fillMode;
            }
        }

        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                showWireframe = value;
                OnPropertyChanged();
                if (showWireframe)
                {
                    FillMode = FillMode.Wireframe;
                }
                else
                {
                    FillMode = FillMode.Solid;
                }
            }
            get
            {
                return showWireframe;
            }
        }

        private bool visibility = true;
        public bool Visibility
        {
            set
            {
                visibility = value;
                OnPropertyChanged();
            }
            get
            {
                return visibility;
            }
        }
        public Color4 Light1Color { get; set; }
        public PhongMaterial ModelMaterial { get; set; }

        public PhongMaterial InnerModelMaterial { get; set; }

        //public PhongMaterial OtherMaterial { set; get; }
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D InnerModel { get; private set; }

        //public MeshGeometry3D Other { get; private set; }
        public Color4 AmbientLightColor { get; set; }

        public Color LineColor { set; get; }

        public LineGeometry3D OctreeModel { set; get; }

        private Media3D.Vector3D camLookDir = new Media3D.Vector3D(-10, -10, -10);
        public Media3D.Vector3D CamLookDir
        {
            set
            {
                if (camLookDir != value)
                {
                    camLookDir = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return camLookDir;
            }
        }

        private Vector3Collection initialPosition;
        private IntCollection initialIndicies;
        private Random rnd = new Random();
        private bool isRemoving = true;
        private int removedIndex = 0;
        public MainViewModel()
        {            // titles
            this.Title = "DynamicTexture Demo";
            this.SubTitle = "WPF & SharpDX";
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            this.Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera { Position = new Media3D.Point3D(10, 10, 10),
                LookDirection = new Media3D.Vector3D(-10, -10, -10), UpDirection = new Media3D.Vector3D(0, 1, 0) };
            this.Light1Color = (Color4)Color.White;
            this.Light1Direction = new Vector3(-10, -10, -10);
            this.AmbientLightColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
            SetupCameraBindings(this.Camera);

            var b2 = new MeshBuilder(true, true, true);
            b2.AddSphere(new Vector3(0f, 0f, 0f), 4, 64, 64);
            b2.AddSphere(new Vector3(5f, 0f, 0f), 2, 32, 32);
            b2.AddTube(new Vector3[] { new Vector3(0f, 5f, 0f), new Vector3(0f, 7f, 0f) }, 2, 12, false, true, true);
            this.Model = b2.ToMeshGeometry3D();
            this.InnerModel = new MeshGeometry3D()
            {
                Indices = Model.Indices, Positions = Model.Positions,
                Normals = new Vector3Collection(Model.Normals.Select(x => { return x * -1; })),
                TextureCoordinates = Model.TextureCoordinates,
                Tangents = Model.Tangents,
                BiTangents = Model.BiTangents
            };
            var uri = new System.Uri(@"test.png", System.UriKind.RelativeOrAbsolute);

            this.ModelMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = Color.White,
                SpecularColor = Color.White,
                SpecularShininess = 100f,
            };

            this.InnerModelMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Color.White,
                SpecularShininess = 100f
            };

            //this.OtherMaterial = new PhongMaterial
            //{
            //    AmbientColor = Color.Gray,
            //    DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
            //    SpecularColor = Color.White,
            //    SpecularShininess = 100f,
            //    DiffuseMap = ModelMaterial.DiffuseMap,
            //    NormalMap = ModelMaterial.NormalMap
            //};

            initialPosition = Model.Positions;
            initialIndicies = Model.Indices;

            //var b3 = new MeshBuilder(true, true, true);
            //b3.AddBox(new Vector3(3, 3, 3), 1, 2,  2);
            //Other = b3.ToMeshGeometry3D();

            this.PropertyChanged += MainViewModel_PropertyChanged;

            LineColor = Color.Blue;

            var lineBuilder = new LineBuilder();
            GeometryOctree octree = new GeometryOctree(Model.Positions, Model.Indices);
            octree.UpdateTree();
            CreateOctreeSegments(lineBuilder, octree);
            OctreeModel = lineBuilder.ToLineGeometry3D();
        }

        private void CreateOctreeSegments(LineBuilder builder, GeometryOctree tree)
        {
            if (tree == null) return;
            var box = tree.Region;
            Vector3[] verts = new Vector3[8];
            verts[0] = box.Minimum;
            verts[1] = new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z); //Z
            verts[2] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Minimum.Z); //Y
            verts[3] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z); //X

            verts[7] = box.Maximum;
            verts[4] = new Vector3(box.Maximum.X, box.Maximum.Y, box.Minimum.Z); //Z
            verts[5] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Maximum.Z); //Y
            verts[6] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Maximum.Z); //X
            builder.AddLine(verts[0], verts[1]);
            builder.AddLine(verts[0], verts[2]);
            builder.AddLine(verts[0], verts[3]);
            builder.AddLine(verts[7], verts[4]);
            builder.AddLine(verts[7], verts[5]);
            builder.AddLine(verts[7], verts[6]);

            builder.AddLine(verts[1], verts[6]);
            builder.AddLine(verts[1], verts[5]);
            builder.AddLine(verts[4], verts[2]);
            builder.AddLine(verts[4], verts[3]);
            builder.AddLine(verts[2], verts[6]);
            builder.AddLine(verts[3], verts[5]);

            if (tree.HasChildren)
            {
                foreach(var child in tree.ChildNodes)
                {
                    CreateOctreeSegments(builder, child as GeometryOctree);
                }
            }          
        }


        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(CamLookDir)))
            {
                Light1Direction = CamLookDir.ToVector3();
            }
        }

        public void SetupCameraBindings(Camera camera)
        {
            if (camera is ProjectionCamera)
            {
                SetBinding("CamLookDir", camera, ProjectionCamera.LookDirectionProperty, this);
            }
        }

        private static void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay)
        {
            var binding = new Binding(path);
            binding.Source = viewModel;
            binding.Mode = mode;
            BindingOperations.SetBinding(dobj, property, binding);
        }

    }
}
