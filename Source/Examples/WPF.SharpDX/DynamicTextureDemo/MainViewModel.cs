using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Media3D = System.Windows.Media.Media3D;

namespace DynamicTextureDemo
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
        public Color4 Light1Color { get; set; }
        public PhongMaterial ModelMaterial { get; set; }

        public PhongMaterial InnerModelMaterial { get; set; }

        //public PhongMaterial OtherMaterial { set; get; }
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D InnerModel { get; private set; }

        //public MeshGeometry3D Other { get; private set; }
        public Color4 AmbientLightColor { get; set; }
        DispatcherTimer timer = new DispatcherTimer();

        public bool DynamicTexture { set; get; } = true;
        public bool DynamicVertices { set; get; } = false;
        public bool DynamicTriangles { set; get; } = false;

        public bool ReverseInnerRotation { set; get; } = false;

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

            var image = LoadFileToMemory(new System.Uri(@"test.png", System.UriKind.RelativeOrAbsolute).ToString());
            this.ModelMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = Color.White,
                SpecularColor = Color.White,
                SpecularShininess = 100f,
                DiffuseAlphaMap = image,
                DiffuseMap = LoadFileToMemory(new System.Uri(@"TextureCheckerboard2.dds", System.UriKind.RelativeOrAbsolute).ToString()),
                NormalMap = LoadFileToMemory(new System.Uri(@"TextureCheckerboard2_dot3.dds", System.UriKind.RelativeOrAbsolute).ToString()),
            };

            this.InnerModelMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Color.White,
                SpecularShininess = 100f,
                DiffuseAlphaMap = image,
                DiffuseMap = LoadFileToMemory(new System.Uri(@"TextureNoise1.jpg", System.UriKind.RelativeOrAbsolute).ToString()),
                NormalMap = ModelMaterial.NormalMap
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
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public static Stream ToStream(System.Drawing.Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (DynamicTexture)
            {
                var texture = new Vector2Collection(Model.TextureCoordinates);
                for (int i = 1; i < Model.TextureCoordinates.Count; ++i)
                {
                    texture[i - 1] = Model.TextureCoordinates[i];
                }
                texture[texture.Count - 1] = Model.TextureCoordinates[0];
                Model.TextureCoordinates = texture;
                if (ReverseInnerRotation)
                {
                    var texture1 = new Vector2Collection(texture);
                    texture1.Reverse();
                    InnerModel.TextureCoordinates = texture1;
                }
                else
                {
                    InnerModel.TextureCoordinates = texture;
                }
            }
            if (DynamicVertices)
            {
                var positions = new Vector3Collection(initialPosition);
                for (int i = 0; i < positions.Count; ++i)
                {
                    positions[i] = positions[i] * (float)rnd.Next(95, 105) / 100;
                }
                Model.Normals = MeshGeometryHelper.CalculateNormals(positions, Model.Indices);
                InnerModel.Normals = new Vector3Collection(Model.Normals.Select(x => { return x * -1; }));
                Model.Positions = positions;
                InnerModel.Positions = positions;
                //Alternative implementation
                //Floor.DisablePropertyChangedEvent = true;
                //Floor.Positions = positions;
                //Floor.CalculateNormals();
                //Floor.DisablePropertyChangedEvent = false;
                //Floor.UpdateVertex();
            }
            if (DynamicTriangles)
            {
                var indices = new IntCollection(initialIndicies);
                if (isRemoving)
                {
                    removedIndex += 3 * 8;
                    if (removedIndex >= initialIndicies.Count)
                    {
                        removedIndex = initialIndicies.Count;
                        isRemoving = false;
                    }
                }
                else
                {
                    removedIndex -= 3 * 8;
                    if (removedIndex <= 0)
                    {
                        isRemoving = true;
                        removedIndex = 0;
                    }
                }
                indices.RemoveRange(0, removedIndex);
                Model.Indices = indices;
                InnerModel.Indices = indices;
            }
        }
    }
}
