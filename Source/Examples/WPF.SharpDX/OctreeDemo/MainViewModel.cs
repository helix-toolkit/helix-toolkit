using DemoCore;
using HelixToolkit.SharpDX.Shared.Utilities;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public Color GroupLineColor { set; get; }

        private LineGeometry3D groupOctreeModel = null;
        public LineGeometry3D GroupOctreeModel
        {
            set
            {
                groupOctreeModel = value;
                OnPropertyChanged();
            }
            get
            {
                return groupOctreeModel;
            }
        }

        public ObservableCollection<DataModel> Items { set; get; }

        private IOctree groupOctree = null;
        public IOctree GroupOctree
        {
            set
            {
                if (groupOctree == value)
                    return;
                groupOctree = value;
                OnPropertyChanged();
                if (value != null)
                {
                    GroupOctreeModel = value.CreateOctreeLineModel();
                }
                else
                {
                    GroupOctreeModel = null;
                }
            }
            get
            {
                return groupOctree;
            }
        }

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

        public MainViewModel()
        {            // titles
            this.Title = "DynamicTexture Demo";
            this.SubTitle = "WPF & SharpDX";
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            this.Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
            {
                Position = new Media3D.Point3D(10, 10, 10),
                LookDirection = new Media3D.Vector3D(-10, -10, -10),
                UpDirection = new Media3D.Vector3D(0, 1, 0)
            };
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
                Indices = Model.Indices,
                Positions = Model.Positions,
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

            this.PropertyChanged += MainViewModel_PropertyChanged;

            LineColor = Color.Blue;
            GroupLineColor = Color.Red;
            Model.UpdateOctree();
            OctreeModel = Model.Octree.CreateOctreeLineModel();

            Items = new ObservableCollection<DataModel>();

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    var builder = new MeshBuilder(true, false, false);
                    builder.AddSphere(new Vector3(10f + i + (float)Math.Pow((float)j / 2, 2), 10f + (float)Math.Pow((float)i / 2, 2), 5f + (float)Math.Pow(j, ((float)i / 5))), 1);
                    Items.Add(new DataModel() { Model = builder.ToMeshGeometry3D() });
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
