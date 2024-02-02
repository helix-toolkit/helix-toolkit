using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D11;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace DynamicTextureDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private Vector3D light1Direction = new();

    [ObservableProperty]
    private FillMode fillMode = FillMode.Solid;

    [ObservableProperty]
    private bool showWireframe = false;

    partial void OnShowWireframeChanged(bool value)
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

    public Color Light1Color { get; set; }

    public PhongMaterial ModelMaterial { get; set; }

    public PhongMaterial InnerModelMaterial { get; set; }

    //public PhongMaterial OtherMaterial { set; get; }

    public MeshGeometry3D Model { get; private set; }

    public MeshGeometry3D InnerModel { get; private set; }

    public PointGeometry3D PointModel { private set; get; }

    public LineGeometry3D LineModel { private set; get; }

    public LineMaterial LineMaterial { private set; get; }

    //public MeshGeometry3D Other { get; private set; }
    public Color AmbientLightColor { get; set; }

    private readonly DispatcherTimer timer = new();

    public bool DynamicTexture { set; get; } = true;

    public bool DynamicVertices { set; get; } = false;

    public bool DynamicTriangles { set; get; } = false;

    public bool DynamicPointColor { set; get; } = true;

    public bool AnimateUVOffset { set; get; } = true;

    public bool ReverseInnerRotation { set; get; } = false;

    [ObservableProperty]
    private Vector3D camLookDir = new(-10, -10, -10);

    partial void OnCamLookDirChanged(Vector3D value)
    {
        Light1Direction = value;
    }

    private Vector3Collection? initialPosition;
    private IntCollection? initialIndicies;
    private readonly Random rnd = new();
    private bool isRemoving = true;
    private int removedIndex = 0;
    private readonly CancellationTokenSource cts = new();
    private readonly SynchronizationContext? context = SynchronizationContext.Current;
    private int counter = 0;

    public MainViewModel()
    {
        // titles
        this.Title = "DynamicTexture Demo";
        this.SubTitle = "WPF & SharpDX";
        EffectsManager = new DefaultEffectsManager();
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

        var image = TextureModel.Create(new System.Uri(@"test.png", System.UriKind.RelativeOrAbsolute).ToString());
        this.ModelMaterial = new PhongMaterial
        {
            AmbientColor = Colors.Gray.ToColor4(),
            DiffuseColor = Colors.White.ToColor4(),
            SpecularColor = Colors.White.ToColor4(),
            SpecularShininess = 100f,
            DiffuseAlphaMap = image,
            DiffuseMap = TextureModel.Create(new System.Uri(@"TextureCheckerboard2.dds", System.UriKind.RelativeOrAbsolute).ToString()),
            NormalMap = TextureModel.Create(new System.Uri(@"TextureCheckerboard2_dot3.dds", System.UriKind.RelativeOrAbsolute).ToString()),
        };

        this.InnerModelMaterial = new PhongMaterial
        {
            AmbientColor = Colors.Gray.ToColor4(),
            DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
            SpecularColor = Colors.White.ToColor4(),
            SpecularShininess = 100f,
            DiffuseAlphaMap = image,
            DiffuseMap = TextureModel.Create(new System.Uri(@"TextureNoise1.jpg", System.UriKind.RelativeOrAbsolute).ToString()),
            NormalMap = ModelMaterial.NormalMap
        };


        initialPosition = Model.Positions;
        initialIndicies = Model.Indices;
        #region Point Model
        PointModel = new PointGeometry3D()
        {
            IsDynamic = true,
            Positions = Model.Positions
        };
        int count = PointModel.Positions?.Count ?? 0;
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
        LineModel = new LineGeometry3D()
        {
            IsDynamic = true,
            Positions = PointModel.Positions is null ? null : new Vector3Collection(PointModel.Positions)
        };
        LineModel.Positions?.Add(Vector3.Zero);
        var indices = new IntCollection(count * 2);
        for (int i = 0; i < count; ++i)
        {
            indices.Add(count);
            indices.Add(i);
        }
        LineModel.Indices = indices;
        colors = new Color4Collection(LineModel.Positions?.Count ?? 0);
        for (int i = 0; i < count; ++i)
        {
            colors.Add(new Color4((float)i / count, 1 - (float)i / count, 0, 1));
        }
        colors.Add(Colors.Blue.ToColor4());
        LineModel.Colors = colors;
        LineMaterial = new LineArrowHeadMaterial()
        {
            Color = Colors.White,
            Thickness = 0.5,
            ArrowSize = 0.02
        };
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

    public static Stream ToStream(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
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
            Vector2Collection? texture = null;
            if (!AnimateUVOffset)
            {
                texture = Model.TextureCoordinates is null ? null : new Vector2Collection(Model.TextureCoordinates);
                if (texture is not null)
                {
                    var t0 = texture[0];
                    for (int i = 1; i < texture.Count; ++i)
                    {
                        texture[i - 1] = texture[i];
                    }
                    texture[^1] = t0;
                }
            }

            context?.Send((o) =>
            {
                if (!AnimateUVOffset)
                {
                    Model.TextureCoordinates = texture;
                    if (ReverseInnerRotation)
                    {
                        var texture1 = texture is null ? null : new Vector2Collection(texture);
                        texture1?.Reverse();
                        InnerModel.TextureCoordinates = texture1;
                    }
                    else
                    {
                        InnerModel.TextureCoordinates = texture;
                    }
                }
                else
                {
                    ModelMaterial.UVTransform = new UVTransform(0, Vector2.One,
                        ModelMaterial.UVTransform.Translation + new Vector2(0.005f, -0.01f));
                    InnerModelMaterial.UVTransform = new UVTransform(0, Vector2.One,
                        InnerModelMaterial.UVTransform.Translation + new Vector2(-0.01f, 0.005f));
                }
            }, null);

        }
        if (DynamicVertices)
        {
            var positions = initialPosition is null ? new Vector3Collection() : new Vector3Collection(initialPosition);
            for (int i = 0; i < positions.Count; ++i)
            {
                var off = (float)Math.Sin(Math.PI * (float)(counter + i) / 64);
                var p = positions[i];
                p *= 0.8f + off * 0.2f;
                positions[i] = p;
            }
            var linePositions = new Vector3Collection(positions)
            {
                Vector3.Zero
            };
            //var normals =  MeshGeometryHelper.CalculateNormals(positions, initialIndicies);
            //var innerNormals =  new Vector3Collection(normals.Select(x => { return x * -1; }));
            context?.Send((o) =>
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
            var indices = initialIndicies is null ? new IntCollection() : new IntCollection(initialIndicies);
            if (isRemoving)
            {
                removedIndex += 3 * 8;
                if (removedIndex >= (initialIndicies?.Count ?? 0))
                {
                    removedIndex = initialIndicies?.Count ?? 0;
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
            context?.Send((o) =>
            {
                Model.Indices = indices;
                InnerModel.Indices = indices;
            }, null);
        }
        if (DynamicTexture)
        {
            var colors = PointModel.Colors is null ? new Color4Collection() : new Color4Collection(PointModel.Colors);
            for (int k = 0; k < 10; ++k)
            {
                var c = colors[^1];
                for (int i = colors.Count - 1; i > 0; --i)
                {
                    colors[i] = colors[i - 1];
                }
                colors[0] = c;
            }
            var lineColors = LineModel.Colors is null ? new Color4Collection() : new Color4Collection(LineModel.Colors);
            for (int k = 0; k < 10; ++k)
            {
                var c = lineColors[colors.Count - 2];
                for (int i = lineColors.Count - 2; i > 0; --i)
                {
                    lineColors[i] = lineColors[i - 1];
                }
                lineColors[0] = c;
            }
            context?.Send((o) =>
            {
                PointModel.Colors = colors;
                LineModel.Colors = lineColors;
            }, null);
        }
    }

    protected override void OnDisposeUnmanaged()
    {
        //timer.Stop();
        //timer.Tick -= Timer_Tick;
        cts.Cancel(true);
        cts.Dispose();

        base.OnDisposeUnmanaged();
    }
}
