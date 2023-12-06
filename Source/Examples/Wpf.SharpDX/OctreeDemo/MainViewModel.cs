using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3 = SharpDX.Vector3;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace OctreeDemo;

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

    [ObservableProperty]
    private bool visibility = true;

    [ObservableProperty]
    private Color light1Color;

    //[ObservableProperty]
    //private MeshGeometry3D other;

    [ObservableProperty]
    private Color ambientLightColor;

    public Color PointColor => Colors.Green;

    public Color PointHitColor => Colors.Red;

    [ObservableProperty]
    private Color lineColor;

    [ObservableProperty]
    private PhongMaterial? material;

    [ObservableProperty]
    private MeshGeometry3D? defaultModel;

    [ObservableProperty]
    private PointGeometry3D? pointsModel;

    [ObservableProperty]
    private PointGeometry3D? pointsHitModel;

    [ObservableProperty]
    private LineGeometry3D? linesModel;

    [ObservableProperty]
    private ObservableCollection<DataModel> items = new();

    [ObservableProperty]
    private List<DataModel>? landerItems;

    [ObservableProperty]
    private Vector3D camLookDir = new(-10, -10, -10);

    partial void OnCamLookDirChanged(Vector3D value)
    {
        Light1Direction = value;
    }

    [ObservableProperty]
    private bool hitThrough;

    private readonly IList<DataModel> HighlightItems = new List<DataModel>();

    [ObservableProperty]
    private int sphereSize = 1;

    partial void OnSphereSizeChanged(int value)
    {
        if (HighlightItems.Count == 0)
        {
            return;
        }

        foreach (SphereModel item in HighlightItems)
        {
            item.Radius = value;
        }
    }

    [ObservableProperty]
    private bool autoDeleteEmptyNode = true;

    [ObservableProperty]
    private bool octreeFrameVisible = false;

    public MainViewModel()
    {
        // titles
        this.Title = "DynamicTexture Demo";
        this.SubTitle = "WPF & SharpDX";

        EffectsManager = new DefaultEffectsManager();

        this.Camera = new PerspectiveCamera
        {
            Position = new Point3D(30, 30, 30),
            LookDirection = new Vector3D(-30, -30, -30),
            UpDirection = new Vector3D(0, 1, 0)
        };
        this.Light1Color = Colors.White;
        this.Light1Direction = new Vector3D(-10, -10, -10);
        this.AmbientLightColor = Colors.DimGray;
        SetupCameraBindings(this.Camera);
        LineColor = Colors.Blue;
        Items = new ObservableCollection<DataModel>();
        var sw = Stopwatch.StartNew();
        CreateDefaultModels();
        sw.Stop();
        Console.WriteLine("Create Models total time =" + sw.ElapsedMilliseconds + " ms");
        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        timer.Tick += Timer_Tick;
    }

    private void CreateDefaultModels()
    {
        Material = PhongMaterials.White;
        var b2 = new MeshBuilder(true, true, true);
        b2.AddSphere(new Vector3(15f, 0f, 0f).ToVector(), 4, 64, 64);
        b2.AddSphere(new Vector3(25f, 0f, 0f).ToVector(), 2, 32, 32);
        b2.AddTube(new[] { new Vector3(10f, 5f, 0f).ToVector(), new Vector3(10f, 7f, 0f).ToVector() }, 2, 12, false, true, true);
        DefaultModel = b2.ToMesh().ToMeshGeometry3D();
        DefaultModel.OctreeParameter.RecordHitPathBoundingBoxes = true;

        PointsModel = new PointGeometry3D();
        var offset = new Vector3(1, 1, 1);

        PointsModel.Positions = DefaultModel.Positions is null ? null : new Vector3Collection(DefaultModel.Positions.Select(x => x + offset));
        PointsModel.Indices = PointsModel.Positions is null ? null : new IntCollection(Enumerable.Range(0, PointsModel.Positions.Count));
        PointsModel.OctreeParameter.RecordHitPathBoundingBoxes = true;
        for (int i = 0; i < 50; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                Items.Add(new SphereModel(new Vector3(i - 50, j - 25, i + j - 75), rnd.NextDouble(1, 3)));
            }
        }

        var b3 = new LineBuilder();
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                for (int k = 0; k < 5; ++k)
                {
                    b3.AddBox(new Vector3(-10 - i * 5, j * 5, k * 5), 5, 5, 5);
                }
            }
        }
        LinesModel = b3.ToLineGeometry3D();
        LinesModel.OctreeParameter.RecordHitPathBoundingBoxes = true;
        PointsHitModel = new PointGeometry3D() { Positions = new Vector3Collection(), Indices = new IntCollection() };
        //var landerItems = Load3ds("Car.3ds").Select(x => new DataModel() { Model = x.Geometry as MeshGeometry3D, Material = PhongMaterials.Copper }).ToList();
        //var scale = new Vector3(0.007f);
        //var offset = new Vector3(15, 15, 15);
        //foreach (var item in landerItems)
        //{
        //    for (int i = 0; i < item.Model.Positions.Count; ++i)
        //    {
        //        item.Model.Positions[i] = item.Model.Positions[i] * scale + offset;
        //    }

        //    item.Model.UpdateOctree();
        //}
        //LanderItems = landerItems;
    }

    public List<Object3D> Load3ds(string path)
    {
        var reader = new StudioReader();
        var list = reader.Read(path);
        return list ?? new List<Object3D>();
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
        var binding = new Binding(path)
        {
            Source = viewModel,
            Mode = mode
        };

        BindingOperations.SetBinding(dobj, property, binding);
    }

    public void OnMouseLeftButtonDownHandler(object? sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        foreach (var item in HighlightItems)
        {
            item.Highlight = false;
        }
        HighlightItems.Clear();
        Material = PhongMaterials.White;

        if (sender is not Viewport3DX viewport)
        {
            return;
        }

        var point = e.GetPosition(viewport);
        var hitTests = viewport.FindHits(point);
        if (hitTests != null && hitTests.Count > 0)
        {
            if (HitThrough)
            {
                foreach (var hit in hitTests)
                {
                    var element = hit.ModelHit as Element3D;
                    if (element?.DataContext is DataModel)
                    {
                        if (element.DataContext is DataModel model)
                        {
                            model.Highlight = true;
                            HighlightItems.Add(model);
                        }
                    }
                    else if (element?.DataContext == this)
                    {
                        if (hit.TriangleIndices is not null)
                        {
                            Material = PhongMaterials.Yellow;
                        }
                        else
                        {
                            var v = new Vector3Collection
                            {
                                hit.PointHit
                            };

                            if (PointsHitModel is not null)
                            {
                                PointsHitModel.Positions = v;
                            }

                            var idx = new IntCollection
                            {
                                0
                            };

                            PointsHitModel = new PointGeometry3D()
                            {
                                Positions = v,
                                Indices = idx
                            };
                        }
                    }
                }
            }
            else
            {
                var hit = hitTests[0];
                if (hit.ModelHit is Element3D elem)
                {
                    if (elem.DataContext is DataModel)
                    {
                        if (elem.DataContext is DataModel model)
                        {
                            model.Highlight = true;
                            HighlightItems.Add(model);
                        }
                    }
                    else if (elem.DataContext == this)
                    {
                        if (hit.TriangleIndices is not null)
                        {
                            Material = PhongMaterials.Yellow;
                        }
                        else
                        {
                            var v = new Vector3Collection
                            {
                                hit.PointHit
                            };

                            if (PointsHitModel is not null)
                            {
                                PointsHitModel.Positions = v;
                            }

                            var idx = new IntCollection
                            {
                                0
                            };

                            PointsHitModel = new PointGeometry3D()
                            {
                                Positions = v,
                                Indices = idx
                            };
                        }
                    }
                }
            }
        }
    }

    private double theta = 0;
    private double newModelZ = -5;

    [RelayCommand]
    private void AddModel()
    {
        var x = 10 * (float)Math.Sin(theta);
        var y = 10 * (float)Math.Cos(theta);
        theta += 0.3;
        newModelZ += 0.5;
        var z = (float)(newModelZ);
        Items.Add(new SphereModel(new Vector3(x, y + 20, z + 14), 1));
    }

    [RelayCommand]
    private void RemoveModel()
    {
        if (Items.Count > 0)
        {
            Items.RemoveAt(Items.Count - 1);
            newModelZ = newModelZ > -5 ? newModelZ - 0.5 : 0;
        }
    }

    [RelayCommand]
    private void ClearModel()
    {
        Items.Clear();
        HighlightItems.Clear();
    }

    private readonly DispatcherTimer timer;
    private int counter = 0;

    [ObservableProperty]
    private bool autoTesting = false;

    partial void OnAutoTestingChanged(bool value)
    {
        Enabled = !value;
    }

    [ObservableProperty]
    private bool enabled = true;

    private readonly Random rnd = new();

    [RelayCommand]
    private void AutoTest()
    {
        if (!timer.IsEnabled)
        {
            AutoTesting = true;
            timer.Start();
        }
        else
        {
            timer.Stop();
            AutoTesting = false;
            counter = 0;
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (counter > 499)
        {
            counter = -500;
        }

        if (counter < 0)
        {
            RemoveModel();
        }
        else
        {
            AddModel();
        }

        if (counter % 2 == 0)
        {
            int k = rnd.Next(0, Items.Count - 1);
            int radius = rnd.Next(1, 5);

            if (Items[k] is SphereModel sphere)
            {
                sphere.Radius = radius;
            }
        }

        ++counter;
    }

    [RelayCommand]
    private void MultiViewport()
    {
        var win = new MultiviewportWindow()
        {
            DataContext = this
        };

        win.Show();
    }

    protected override void OnDisposeUnmanaged()
    {
        timer.Stop();
        timer.Tick -= Timer_Tick;

        base.OnDisposeUnmanaged();
    }
}
