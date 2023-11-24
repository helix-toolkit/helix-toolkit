using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using Color4 = SharpDX.Color4;
using Colors = System.Windows.Media.Colors;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3 = SharpDX.Vector3;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace InstancingDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private MeshGeometry3D? model;

    [ObservableProperty]
    private LineGeometry3D? lines;

    [ObservableProperty]
    private LineGeometry3D? grid;

    [ObservableProperty]
    private Matrix[]? modelInstances;

    [ObservableProperty]
    private Matrix[]? selectedLineInstances;

    [ObservableProperty]
    private InstanceParameter[]? instanceParam;

    [ObservableProperty]
    private BillboardSingleImage3D billboardModel;

    [ObservableProperty]
    private Matrix[]? billboardInstances;

    [ObservableProperty]
    private BillboardInstanceParameter[]? billboardInstanceParams;

    [ObservableProperty]
    private PhongMaterial? modelMaterial;

    [ObservableProperty]
    private Transform3D modelTransform;

    [ObservableProperty]
    private Vector3D directionalLightDirection;

    [ObservableProperty]
    private Color directionalLightColor;

    [ObservableProperty]
    private Color ambientLightColor;

    [ObservableProperty]
    private TextureModel? texture;

    [ObservableProperty]
    private bool enableAnimation;

    private readonly DispatcherTimer timer = new();

    private readonly Random rnd = new();

    private float aniX = 0;

    private float aniY = 0;

    private float aniZ = 0;

    private bool aniDir = true;

    public MainViewModel()
    {
        Title = "Instancing Demo";
        EffectsManager = new DefaultEffectsManager();

        // camera setup
        Camera = new PerspectiveCamera
        {
            Position = new Point3D(40, 40, 40),
            LookDirection = new Vector3D(-40, -40, -40),
            UpDirection = new Vector3D(0, 1, 0)
        };

        // setup lighting            
        this.AmbientLightColor = Colors.DarkGray;
        this.DirectionalLightColor = Colors.White;
        this.DirectionalLightDirection = new Vector3D(-2, -5, -2);

        // scene model3d
        var b1 = new MeshBuilder(true, true, true);
        b1.AddBox(new Vector3(0, 0, 0).ToVector(), 1, 1, 1, BoxFaces.All);
        Model = b1.ToMesh().ToMeshGeometry3D();
        if (Model.TextureCoordinates is not null)
        {
            for (int i = 0; i < Model.TextureCoordinates.Count; ++i)
            {
                var tex = Model.TextureCoordinates[i];
                Model.TextureCoordinates[i] = new Vector2(tex.X * 0.5f, tex.Y * 0.5f);
            }
        }

        var l1 = new LineBuilder();
        l1.AddBox(new Vector3(0, 0, 0), 1.1, 1.1, 1.1);
        Lines = l1.ToLineGeometry3D();
        Lines.Colors = Lines.Positions is null ? null : new Color4Collection(Enumerable.Repeat(Colors.White.ToColor4(), Lines.Positions.Count));

        // model trafo
        ModelTransform = Media3D.Transform3D.Identity;// new Media3D.RotateTransform3D(new Media3D.AxisAngleRotation3D(new Vector3D(0, 0, 1), 45));

        // model material
        ModelMaterial = PhongMaterials.White;
        ModelMaterial.DiffuseMap = TextureModel.Create(new System.Uri(@"TextureCheckerboard2.jpg", System.UriKind.RelativeOrAbsolute).ToString());
        ModelMaterial.NormalMap = TextureModel.Create(new System.Uri(@"TextureCheckerboard2_dot3.jpg", System.UriKind.RelativeOrAbsolute).ToString());

        BillboardModel = new BillboardSingleImage3D(ModelMaterial.DiffuseMap, 20, 20);
        Texture = TextureModel.Create("Cubemap_Grandcanyon.dds");
        CreateModels();
        timer.Interval = TimeSpan.FromMilliseconds(30);
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (!EnableAnimation)
        {
            return;
        }

        CreateModels();
    }

    private const int numInstances = 40;
    private List<Matrix> instancesList = new(numInstances * 2);
    private List<Matrix> selectedLineInstancesList = new();
    private List<InstanceParameter> parametersList = new(numInstances * 2);
    private List<Matrix> billboardinstancesList = new(numInstances * 2);
    private List<BillboardInstanceParameter> billboardParamsList = new(numInstances * 2);

    private void CreateModels()
    {
        instancesList.Clear();
        parametersList.Clear();

        if (aniDir)
        {
            aniX += 0.1f;
            aniY += 0.2f;
            aniZ += 0.3f;
        }
        else
        {
            aniX -= 0.1f;
            aniY -= 0.2f;
            aniZ -= 0.3f;
        }

        if (aniX > 15)
        {
            aniDir = false;
        }
        else if (aniX < -15)
        {
            aniDir = true;
        }

        for (int i = -numInstances - (int)aniX; i < numInstances + aniX; i++)
        {
            for (int j = -numInstances - (int)aniX; j < numInstances + aniX; j++)
            {
                var matrix = Matrix.RotationAxis(new Vector3(0, 1, 0), aniX * Math.Sign(j))
                    * Matrix.Translation(new Vector3(i * 1.2f + Math.Sign(i), j * 1.2f + Math.Sign(j), i * j / 2.0f));
                var color = new Color4(1, 1, 1, 1);//new Color4((float)Math.Abs(i) / num, (float)Math.Abs(j) / num, (float)Math.Abs(i + j) / (2 * num), 1);
                                                   //  var emissiveColor = new Color4( rnd.NextFloat(0,1) , rnd.NextFloat(0, 1), rnd.NextFloat(0, 1), rnd.NextFloat(0, 0.2f));
                var k = Math.Abs(i + j) % 4;
                Vector2 offset;
                if (k == 0)
                {
                    offset = new Vector2(aniX, 0);
                }
                else if (k == 1)
                {
                    offset = new Vector2(0.5f + aniX, 0);
                }
                else if (k == 2)
                {
                    offset = new Vector2(0.5f + aniX, 0.5f);
                }
                else
                {
                    offset = new Vector2(aniX, 0.5f);
                }

                parametersList.Add(new InstanceParameter() { DiffuseColor = color, TexCoordOffset = offset });
                instancesList.Add(matrix);
            }
        }
        InstanceParam = parametersList.ToArray();
        ModelInstances = instancesList.ToArray();
        SubTitle = "Number of Instances: " + parametersList.Count.ToString();

        if (BillboardInstances == null)
        {
            for (int i = 0; i < 2 * numInstances; ++i)
            {
                billboardParamsList.Add(new BillboardInstanceParameter()
                {
                    TexCoordOffset = new Vector2(1f / 6 * rnd.Next(0, 6), 1f / 6 * rnd.Next(0, 6)),
                    TexCoordScale = new Vector2(1f / 6, 1f / 6)
                });

                billboardinstancesList.Add(
                    Matrix.Scaling(rnd.NextFloat(0.5f, 4f), rnd.NextFloat(0.5f, 3f), rnd.NextFloat(0.5f, 3f))
                    * Matrix.Translation(new Vector3(rnd.NextFloat(0, 100), rnd.NextFloat(0, 100), rnd.NextFloat(-50, 50))));
            }
            BillboardInstanceParams = billboardParamsList.ToArray();
            BillboardInstances = billboardinstancesList.ToArray();
        }
        else
        {
            for (int i = 0; i < billboardinstancesList.Count; ++i)
            {
                var current = billboardinstancesList[i];
                current.M41 += i % 3 == 0 ? aniX / 50 : -aniX / 50;
                current.M42 += i % 4 == 0 ? aniY / 50 : -aniY / 30;
                current.M43 += i % 5 == 0 ? aniZ / 100 : -aniZ / 50;
                billboardinstancesList[i] = current;
            }
            BillboardInstances = billboardinstancesList.ToArray();
        }
    }

    public void OnMouseLeftButtonDownHandler(object? sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (EnableAnimation)
        {
            return;
        }

        if (sender is not Viewport3DX viewport)
        {
            return;
        }

        var point = e.GetPosition(viewport);
        var hitTests = viewport.FindHits(point);
        if (hitTests.Count > 0)
        {
            foreach (var hit in hitTests)
            {
                if (hit.ModelHit is InstancingMeshGeometryModel3D)
                {
                    if (hit?.Tag is not null && InstanceParam is not null)
                    {
                        var index = (int)hit.Tag;

                        InstanceParam[index].EmissiveColor = InstanceParam[index].EmissiveColor != Colors.Yellow.ToColor4() ? Colors.Yellow.ToColor4() : Colors.Black.ToColor4();
                        InstanceParam = (InstanceParameter[])InstanceParam.Clone();
                    }

                    break;
                }
                else if (hit.ModelHit is LineGeometryModel3D)
                {
                    if (hit?.Tag is not null && ModelInstances is not null)
                    {
                        var index = (int)hit.Tag;

                        SelectedLineInstances = new Matrix[]
                        {
                            ModelInstances[index]
                        };
                    }

                    break;
                }
            }
        }
    }

    protected override void OnDisposeUnmanaged()
    {
        timer.Stop();
        timer.Tick -= Timer_Tick;

        base.OnDisposeUnmanaged();
    }
}
