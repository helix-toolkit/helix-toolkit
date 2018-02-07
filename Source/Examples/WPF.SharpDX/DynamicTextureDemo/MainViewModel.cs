using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Color = System.Windows.Media.Color;
using Plane = SharpDX.Plane;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using Color4 = SharpDX.Color4;

namespace DynamicTextureDemo
{
    public class MainViewModel : BaseViewModel
    {
        private Vector3D light1Direction = new Vector3D();
        public Vector3D Light1Direction
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
        public Color Light1Color { get; set; }
        public PhongMaterial ModelMaterial { get; set; }

        public PhongMaterial InnerModelMaterial { get; set; }

        //public PhongMaterial OtherMaterial { set; get; }
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D InnerModel { get; private set; }

        //public MeshGeometry3D Other { get; private set; }
        public Color AmbientLightColor { get; set; }
        DispatcherTimer timer = new DispatcherTimer();

        public bool DynamicTexture { set; get; } = true;
        public bool DynamicVertices { set; get; } = false;
        public bool DynamicTriangles { set; get; } = false;

        public bool ReverseInnerRotation { set; get; } = false;

        private Vector3D camLookDir = new Vector3D(-10, -10, -10);
        public Vector3D CamLookDir
        {
            set
            {
                if (camLookDir != value)
                {
                    camLookDir = value;
                    OnPropertyChanged();
                    Light1Direction = value;
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
            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];            
            this.Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
            {
                Position = new Point3D(10, 10, 10),
                LookDirection = new Vector3D(-10, -10, -10),
                UpDirection = new Vector3D(0, 1, 0)
            };
            this.Light1Color = Colors.White;
            this.Light1Direction = new Vector3D(-10, -10, -10);
            this.AmbientLightColor = Colors.Black;
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
                AmbientColor = Colors.Gray.ToColor4(),
                DiffuseColor = Colors.White.ToColor4(),
                SpecularColor = Colors.White.ToColor4(),
                SpecularShininess = 100f,
                DiffuseAlphaMap = image,
                DiffuseMap = LoadFileToMemory(new System.Uri(@"TextureCheckerboard2.dds", System.UriKind.RelativeOrAbsolute).ToString()),
                NormalMap = LoadFileToMemory(new System.Uri(@"TextureCheckerboard2_dot3.dds", System.UriKind.RelativeOrAbsolute).ToString()),
            };

            this.InnerModelMaterial = new PhongMaterial
            {
                AmbientColor = Colors.Gray.ToColor4(),
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Colors.White.ToColor4(),
                SpecularShininess = 100f,
                DiffuseAlphaMap = image,
                DiffuseMap = LoadFileToMemory(new System.Uri(@"TextureNoise1.jpg", System.UriKind.RelativeOrAbsolute).ToString()),
                NormalMap = ModelMaterial.NormalMap
            };


            initialPosition = Model.Positions;
            initialIndicies = Model.Indices;


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

        protected override void Dispose(bool disposing)
        {
            timer.Stop();
            timer.Tick -= Timer_Tick;
            base.Dispose(disposing);
        }
    }
}
