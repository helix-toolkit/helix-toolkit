using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
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
        public PhongMaterial FloorMaterial { get; set; }

        public PhongMaterial InnerFloorMaterial { get; set; }
        public MeshGeometry3D Floor { get; private set; }
        public MeshGeometry3D InnerFloor { get; private set; }
        public Color4 AmbientLightColor { get; set; }
        DispatcherTimer timer = new DispatcherTimer();

        public bool DynamicTexture { set; get; } = true;
        public bool DynamicVertices { set; get; } = false;
        public bool DynamicTriangles { set; get; } = false;

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
            this.Floor = b2.ToMeshGeometry3D();
            this.InnerFloor = new MeshGeometry3D()
            {
                Indices = Floor.Indices, Positions = Floor.Positions,
                Normals = new Vector3Collection(Floor.Normals.Select(x => { return x * -1; })),
                TextureCoordinates = Floor.TextureCoordinates,
                Tangents = Floor.Tangents,
                BiTangents = Floor.BiTangents
            };
            this.FloorMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Color.White,
                SpecularShininess = 100f,
                DiffuseMap = new BitmapImage(new System.Uri(@"TextureCheckerboard2.jpg", System.UriKind.RelativeOrAbsolute)),
                NormalMap = new BitmapImage(new System.Uri(@"TextureCheckerboard2_dot3.jpg", System.UriKind.RelativeOrAbsolute)),
            };

            this.InnerFloorMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Color.White,
                SpecularShininess = 100f,
                DiffuseMap = new BitmapImage(new System.Uri(@"TextureNoise1.jpg", System.UriKind.RelativeOrAbsolute)),
                NormalMap = new BitmapImage(new System.Uri(@"TextureCheckerboard2_dot3.jpg", System.UriKind.RelativeOrAbsolute)),
            };

            initialPosition = Floor.Positions;
            initialIndicies = Floor.Indices;
            this.PropertyChanged += MainViewModel_PropertyChanged;
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += Timer_Tick;
            timer.Start();
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
                var texture = new Vector2Collection(Floor.TextureCoordinates);
                for(int i=1; i < Floor.TextureCoordinates.Count; ++i)
                {
                    texture[i-1] = Floor.TextureCoordinates[i];
                }
                texture[texture.Count-1] = Floor.TextureCoordinates[0];
                Floor.TextureCoordinates = texture;
                InnerFloor.TextureCoordinates = texture;
            }
            if(DynamicVertices)
            {
                var positions = new Vector3Collection(initialPosition);
                for(int i=0; i<positions.Count; ++i)
                {
                    positions[i] = positions[i] * (float)rnd.Next(95, 105)/100;
                }
                Floor.Normals = MeshGeometryHelper.CalculateNormals(positions, Floor.Indices);
                InnerFloor.Normals = new Vector3Collection(Floor.Normals.Select(x => { return x * -1; }));
                Floor.Positions = positions;
                InnerFloor.Positions = positions;
                //Alternative implementation
                //Floor.DisablePropertyChangedEvent = true;
                //Floor.Positions = positions;
                //Floor.CalculateNormals();
                //Floor.DisablePropertyChangedEvent = false;
                //Floor.UpdateVertex();
            }
            if(DynamicTriangles)
            {
                var indices = new IntCollection(initialIndicies);
                if (isRemoving)
                {
                    removedIndex += 12;
                    if(removedIndex >= initialIndicies.Count)
                    {
                        removedIndex = initialIndicies.Count;
                        isRemoving = false;
                    }
                }
                else
                {
                    removedIndex -= 12;
                    if (removedIndex <= 0)
                    {
                        isRemoving = true;
                        removedIndex = 0;
                    }                   
                }
                indices.RemoveRange(0, removedIndex);
                Floor.Indices = indices;
                InnerFloor.Indices = indices;
            }
        }
    }
}
