using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX.Direct3D11;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using Color4 = HelixToolkit.Mathematics.Color4;
using Colors = System.Windows.Media.Colors;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3 = System.Numerics.Vector3;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

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
        public PointGeometry3D PointModel { private set; get; }
        public LineGeometry3D LineModel { private set; get; }

        //public MeshGeometry3D Other { get; private set; }
        public Color AmbientLightColor { get; set; }
        DispatcherTimer timer = new DispatcherTimer();

        public bool DynamicTexture { set; get; } = true;
        public bool DynamicVertices { set; get; } = false;
        public bool DynamicTriangles { set; get; } = false;
        public bool DynamicPointColor { set; get; } = true;

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
        private CancellationTokenSource cts = new CancellationTokenSource();
        private SynchronizationContext context = SynchronizationContext.Current;
        private int counter = 0;

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

            var b2 = new MeshBuilder(true, true, true);
            b2.AddSphere(new Vector3(0f, 0f, 0f), 4, 64, 64);
            this.Model = b2.ToMeshGeometry3D();
            Model.IsDynamic = true;
            this.InnerModel = new MeshGeometry3D()
            {
                Indices = Model.Indices,
                Positions = Model.Positions,
                Normals = Model.Normals,
                TextureCoordinates = Model.TextureCoordinates,
                Tangents = Model.Tangents,
                BiTangents = Model.BiTangents,
                IsDynamic = true
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
            #region Point Model
            PointModel = new PointGeometry3D()
            {
                IsDynamic = true, Positions = Model.Positions
            };
            int count = PointModel.Positions.Count;
            var colors = new Color4Collection(count);
            for (int i = 0; i < count / 2; ++i)
            {
                colors.Add(new Color4(0, 1, 1, 1));
            }
            for (int i = 0; i < count / 2; ++i)
            {
                colors.Add(new Color4(0, 0, 0, 0));
            }
            PointModel.Colors = colors;
            #endregion

            #region Line Model
            LineModel = new LineGeometry3D() { IsDynamic =true, Positions = new Vector3Collection(PointModel.Positions) };
            LineModel.Positions.Add(Vector3.Zero);
            var indices = new IntCollection(count * 2);
            for(int i = 0; i < count; ++i)
            {
                indices.Add(count);
                indices.Add(i);
            }
            LineModel.Indices = indices;
            colors = new Color4Collection(LineModel.Positions.Count);           
            for(int i = 0; i < count; ++i)
            {
                colors.Add(new Color4((float)i / count,1-(float)i / count, 0, 1));
            }
            colors.Add(Colors.Blue.ToColor4());
            LineModel.Colors = colors;
            #endregion
            var token = cts.Token;
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Timer_Tick();
                    Task.Delay(16).Wait();
                }
            }, token);
            //timer.Interval = TimeSpan.FromMilliseconds(16);
            //timer.Tick += Timer_Tick;
            //timer.Start();
        }

        public static Stream ToStream(System.Drawing.Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        private void Timer_Tick()
        {
            ++counter;
            counter %= 128;
            if (DynamicTexture)
            {
                var texture = new Vector2Collection(Model.TextureCoordinates);
                var t0 = texture[0];
                for (int i = 1; i < texture.Count; ++i)
                {
                    texture[i - 1] = texture[i];
                }
                texture[texture.Count - 1] = t0;                
                context.Send((o) =>
                {
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
                }, null);

            }
            if (DynamicVertices)
            {
                var positions = new Vector3Collection(initialPosition);
                for (int i = 0; i < positions.Count; ++i)
                {
                    var off = (float)Math.Sin(Math.PI*(float)(counter + i)/64);
                    var p = positions[i];
                    p *= 0.8f + off * 0.2f;
                    positions[i] = p;
                }
                var linePositions = new Vector3Collection(positions);
                linePositions.Add(Vector3.Zero);
                //var normals =  MeshGeometryHelper.CalculateNormals(positions, initialIndicies);
                //var innerNormals =  new Vector3Collection(normals.Select(x => { return x * -1; }));
                context.Send((o) =>
                {
                    //Model.Normals = normals;
                    //InnerModel.Normals = innerNormals;
                    //Model.Positions = positions;
                    //InnerModel.Positions = positions;
                    PointModel.Positions = positions;
                    LineModel.Positions = linePositions;
                }, null);
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
                context.Send((o) =>
                {
                    Model.Indices = indices;
                    InnerModel.Indices = indices;
                }, null);
            }
            if (DynamicTexture)
            {
                var colors = new Color4Collection(PointModel.Colors);
                for(int k = 0; k < 10; ++k)
                {
                    var c = colors[colors.Count - 1];
                    for (int i = colors.Count - 1; i > 0; --i)
                    {
                        colors[i] = colors[i - 1];
                    }
                    colors[0] = c;
                }
                var lineColors = new Color4Collection(LineModel.Colors);
                for (int k = 0; k < 10; ++k)
                {
                    var c = lineColors[colors.Count - 2];
                    for (int i = lineColors.Count - 2; i > 0; --i)
                    {
                        lineColors[i] = lineColors[i - 1];
                    }
                    lineColors[0] = c;
                }
                context.Send((o) =>
                {
                    PointModel.Colors = colors;
                    LineModel.Colors = lineColors;
                }, null);
            }
        }

        protected override void Dispose(bool disposing)
        {
            //timer.Stop();
            //timer.Tick -= Timer_Tick;
            cts.Cancel(true);
            cts.Dispose();
            base.Dispose(disposing);
        }
    }
}
