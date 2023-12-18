using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Media = System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;

namespace ParticleSystemDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private MeshGeometry3D model;

    [ObservableProperty]
    private Media3D.Transform3D emitterTransform = new Media3D.MatrixTransform3D(new Media3D.Matrix3D(0.5, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0.5, 0, 0, -4, 0, 1));

    partial void OnEmitterTransformChanged(Media3D.Transform3D value)
    {
        EmitterLocation = new Media3D.Point3D(value.Value.OffsetX, value.Value.OffsetY, value.Value.OffsetZ);
    }

    [ObservableProperty]
    private double emitterRadius = 0.5;

    partial void OnEmitterRadiusChanged(double value)
    {
        var matrix = EmitterTransform.Value;
        matrix.M11 = matrix.M22 = matrix.M33 = value;
        EmitterTransform = new Media3D.MatrixTransform3D(matrix);
    }

    [ObservableProperty]
    private Media3D.Point3D emitterLocation = new(0, -4, 0);

    [ObservableProperty]
    private Media3D.Transform3D consumerTransform = new Media3D.MatrixTransform3D(new Media3D.Matrix3D(0.5, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0.5, 0, 0, 4, 0, 1));

    partial void OnConsumerTransformChanged(Media3D.Transform3D value)
    {
        ConsumerLocation = new Media3D.Point3D(value.Value.OffsetX, value.Value.OffsetY, value.Value.OffsetZ);
    }

    [ObservableProperty]
    private Media3D.Point3D consumerLocation = new(0, 4, 0);

    [ObservableProperty]
    private double consumerRadius = 0.5;

    partial void OnConsumerRadiusChanged(double value)
    {
        var matrix = ConsumerTransform.Value;
        matrix.M11 = matrix.M22 = matrix.M33 = value;
        ConsumerTransform = new Media3D.MatrixTransform3D(matrix);
    }

    public Material EmitterMaterial { get; } = new PhongMaterial() { DiffuseColor = new Color4(1, 0, 1, 1) };

    public Material ConsumerMaterial { get; } = new PhongMaterial() { DiffuseColor = new Color4(0.5f, 1f, 0.5f, 1) };

    [ObservableProperty]
    private Stream? particleTexture;

    [ObservableProperty]
    private Media3D.Vector3D acceleration = new(0, 1, 0);

    [ObservableProperty]
    private int accelerationX = 0;

    partial void OnAccelerationXChanged(int value)
    {
        UpdateAcceleration();
    }

    [ObservableProperty]
    private Size particleSize = new(0.1, 0.1);

    [ObservableProperty]
    private int sizeSlider = 10;

    partial void OnSizeSliderChanged(int value)
    {
        ParticleSize = new Size(((double)value) / 100, ((double)value) / 100);
    }

    [ObservableProperty]
    private int accelerationY = 100;

    partial void OnAccelerationYChanged(int value)
    {
        UpdateAcceleration();
    }

    [ObservableProperty]
    private int accelerationZ = 0;

    partial void OnAccelerationZChanged(int value)
    {
        UpdateAcceleration();
    }

    const int DefaultBoundScale = 10;
    public LineGeometry3D BoundingLines { private set; get; }

    public Media3D.ScaleTransform3D BoundingLineTransform { private set; get; } = new Media3D.ScaleTransform3D(DefaultBoundScale, DefaultBoundScale, DefaultBoundScale);

    [ObservableProperty]
    private Media3D.Rect3D particleBounds = new(0, 0, 0, DefaultBoundScale, DefaultBoundScale, DefaultBoundScale);

    [ObservableProperty]
    private int boundScale = DefaultBoundScale;

    partial void OnBoundScaleChanged(int value)
    {
        ParticleBounds = new Media3D.Rect3D(0, 0, 0, value, value, value);
        BoundingLineTransform.ScaleX = BoundingLineTransform.ScaleY = BoundingLineTransform.ScaleZ = value;
    }

    [ObservableProperty]
    private Media.Color blendColor = Media.Colors.White;

    partial void OnBlendColorChanged(Media.Color value)
    {
        BlendColorBrush = new Media.SolidColorBrush(value);
    }

    [ObservableProperty]
    private int redValue = 255;

    partial void OnRedValueChanged(int value)
    {
        BlendColor = Media.Color.FromRgb((byte)RedValue, (byte)GreenValue, (byte)BlueValue);
    }

    [ObservableProperty]
    private int greenValue = 255;

    partial void OnGreenValueChanged(int value)
    {
        BlendColor = Media.Color.FromRgb((byte)RedValue, (byte)GreenValue, (byte)BlueValue);
    }

    [ObservableProperty]
    private int blueValue = 255;

    partial void OnBlueValueChanged(int value)
    {
        BlendColor = Media.Color.FromRgb((byte)RedValue, (byte)GreenValue, (byte)BlueValue);
    }

    [ObservableProperty]
    private Media.SolidColorBrush blendColorBrush = new(Media.Colors.White);

    [ObservableProperty]
    private int numTextureRows;

    [ObservableProperty]
    private int numTextureColumns;

    [ObservableProperty]
    private int selectedTextureIndex = 0;

    partial void OnSelectedTextureIndexChanged(int value)
    {
        LoadTexture(value);
    }

    public Array BlendOperationArray { get; } = Enum.GetValues(typeof(BlendOperation));

    public Array BlendOptionArray { get; } = Enum.GetValues(typeof(BlendOption));

    [ObservableProperty]
    private BlendOption sourceBlendOption = BlendOption.One;

    [ObservableProperty]
    private BlendOption sourceAlphaBlendOption = BlendOption.One;

    [ObservableProperty]
    private BlendOption destBlendOption = BlendOption.One;

    [ObservableProperty]
    private BlendOption destAlphaBlendOption = BlendOption.Zero;

    [ObservableProperty]
    private Media.Color blendFactorColor = Media.Colors.White;

    partial void OnBlendFactorColorChanged(Media.Color value)
    {
        BlendFactorColorBrush = new Media.SolidColorBrush(value);
    }

    [ObservableProperty]
    private int redFactorValue = 255;

    partial void OnRedFactorValueChanged(int value)
    {
        BlendFactorColor = Media.Color.FromRgb((byte)RedFactorValue, (byte)GreenFactorValue, (byte)BlueFactorValue);
    }

    [ObservableProperty]
    private int greenFactorValue = 255;

    partial void OnGreenFactorValueChanged(int value)
    {
        BlendFactorColor = Media.Color.FromRgb((byte)RedFactorValue, (byte)GreenFactorValue, (byte)BlueFactorValue);
    }

    [ObservableProperty]
    private int blueFactorValue = 255;

    partial void OnBlueFactorValueChanged(int value)
    {
        BlendFactorColor = Media.Color.FromRgb((byte)RedFactorValue, (byte)GreenFactorValue, (byte)BlueFactorValue);
    }

    [ObservableProperty]
    private Media.SolidColorBrush blendFactorColorBrush = new(Media.Colors.White);

    public IList<Matrix4x4> Instances { private set; get; }

    public readonly Tuple<int, int>[] TextureColumnsRows = new Tuple<int, int>[] { new(1, 1), new(4, 4), new(4, 4), new(6, 5) };
    public readonly string[] Textures = new string[] { @"Snowflake.png", @"FXT_Explosion_Fireball_Atlas_d.png", @"FXT_Sparks_01_Atlas_d.png", @"Smoke30Frames_0.png" };
    public readonly int[] DefaultParticleSizes = new int[] { 20, 90, 40, 90 };

    public Media.Color Light1Color { get; set; } = Media.Colors.White;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        var lineBuilder = new LineBuilder();
        lineBuilder.AddBox(new Vector3(), 1, 1, 1);
        BoundingLines = lineBuilder.ToLineGeometry3D();
        LoadTexture(SelectedTextureIndex);
        var meshBuilder = new MeshBuilder();
        meshBuilder.AddSphere(new Vector3(0, 0, 0), 0.5f, 16, 16);
        Model = meshBuilder.ToMeshGeometry3D();
        Camera = new PerspectiveCamera()
        {
            Position = new Media3D.Point3D(0, 0, 20),
            UpDirection = new Media3D.Vector3D(0, 1, 0),
            LookDirection = new Media3D.Vector3D(0, 0, -20)
        };
        Instances = new Matrix[] {
                Matrix.Identity, Matrix.CreateScale(1,-1, 1) * Matrix.CreateTranslation(10, 0, 10), Matrix.CreateTranslation(-10, 0, 10), Matrix.CreateTranslation(10, 0, -10),
                Matrix.CreateFromAxisAngle(new Vector3(1,0,0), (float)Math.PI / 2) *  Matrix.CreateTranslation(-10, 0, -10), };
    }

    private void LoadTexture(int index)
    {
        using (var file = new FileStream(new Uri(Textures[index], UriKind.RelativeOrAbsolute).ToString(), FileMode.Open))
        {
            var mem = new MemoryStream();
            file.CopyTo(mem);
            ParticleTexture = mem;
        }

        NumTextureColumns = TextureColumnsRows[index].Item1;
        NumTextureRows = TextureColumnsRows[index].Item2;
        SizeSlider = DefaultParticleSizes[index];
        switch (index)
        {
            case 3:
                SourceBlendOption = BlendOption.SourceAlpha;
                SourceAlphaBlendOption = BlendOption.Zero;
                DestBlendOption = BlendOption.BlendFactor;
                DestAlphaBlendOption = BlendOption.Zero;
                break;
            default:
                SourceBlendOption = BlendOption.One;
                SourceAlphaBlendOption = BlendOption.One;
                DestBlendOption = BlendOption.One;
                DestAlphaBlendOption = BlendOption.Zero;
                break;
        }
    }

    private void UpdateAcceleration()
    {
        Acceleration = new Media3D.Vector3D((double)AccelerationX / 100, (double)AccelerationY / 100, (double)AccelerationZ / 100);
    }
}
