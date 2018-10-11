using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Plane = SharpDX.Plane;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using Color4 = SharpDX.Color4;

namespace OctreeDemo
{
    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
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
        public Color Light1Color { get; set; }

        //public MeshGeometry3D Other { get; private set; }
        public Color AmbientLightColor { get; set; }

        public Color PointColor
        { get { return Colors.Green; } }

        public Color PointHitColor
        { get { return Colors.Red; } }
        public Color LineColor { set; get; }

        private PhongMaterial material;
        public PhongMaterial Material
        {
            private set
            {
                SetValue<PhongMaterial>(ref material, value, nameof(Material));
            }
            get
            {
                return material;
            }
        }
        public MeshGeometry3D DefaultModel { private set; get; }
        public PointGeometry3D PointsModel { private set; get; }

        private PointGeometry3D pointsHitModel;
        public PointGeometry3D PointsHitModel
        {
            set
            {
                SetValue(ref pointsHitModel, value, nameof(PointsHitModel));
            }
            get
            {
                return pointsHitModel;
            }
        }

        public LineGeometry3D LinesModel { private set; get; }
        public ObservableCollection<DataModel> Items { set; get; }
        public List<DataModel> LanderItems { private set; get; }

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

        public bool HitThrough { set; get; }

        private readonly IList<DataModel> HighlightItems = new List<DataModel>();

        private int sphereSize = 1;
        public int SphereSize
        {
            set
            {
                if (SetValue<int>(ref sphereSize, value, nameof(SphereSize)))
                {
                    if (HighlightItems.Count > 0)
                    {
                        foreach (SphereModel item in HighlightItems)
                        {
                            item.Radius = value;
                        }
                    }
                }
            }
            get
            {
                return sphereSize;
            }
        }

        private bool autoDeleteEmptyNode = true;
        public bool AutoDeleteEmptyNode
        {
            set
            {
                autoDeleteEmptyNode = value;
                OnPropertyChanged();
            }
            get { return autoDeleteEmptyNode; }
        }

        private bool octreeFrameVisible = false;
        public bool OctreeFrameVisible
        {
            set
            {
                octreeFrameVisible = value;
                OnPropertyChanged();
            }
            get { return octreeFrameVisible; }
        }

        public ICommand AddModelCommand { private set; get; }
        public ICommand RemoveModelCommand { private set; get; }
        public ICommand ClearModelCommand { private set; get; }
        public ICommand AutoTestCommand { private set; get; }

        public ICommand MultiViewportCommand { private set; get; }

        public MainViewModel()
        {            // titles
            this.Title = "DynamicTexture Demo";
            this.SubTitle = "WPF & SharpDX";
            EffectsManager = new DefaultEffectsManager();
            this.Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
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
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
            AddModelCommand = new RelayCommand(AddModel);
            RemoveModelCommand = new RelayCommand(RemoveModel);
            ClearModelCommand = new RelayCommand(ClearModel);
            AutoTestCommand = new RelayCommand(AutoTestAddRemove);
            MultiViewportCommand = new RelayCommand((o) => 
            {
                var win = new MultiviewportWin() { DataContext = this };
                win.Show();
            });
        }

        private void CreateDefaultModels()
        {
            Material = PhongMaterials.White;
            var b2 = new MeshBuilder(true, true, true);
            b2.AddSphere(new Vector3(15f, 0f, 0f), 4, 64, 64);
            b2.AddSphere(new Vector3(25f, 0f, 0f), 2, 32, 32);
            b2.AddTube(new Vector3[] { new Vector3(10f, 5f, 0f), new Vector3(10f, 7f, 0f) }, 2, 12, false, true, true);
            DefaultModel = b2.ToMeshGeometry3D();
            DefaultModel.OctreeParameter.RecordHitPathBoundingBoxes = true;

            PointsModel = new PointGeometry3D();
            var offset = new Vector3(1, 1, 1);
            
            PointsModel.Positions = new Vector3Collection(DefaultModel.Positions.Select(x=>x+offset));
            PointsModel.Indices = new IntCollection(Enumerable.Range(0, PointsModel.Positions.Count));
            PointsModel.OctreeParameter.RecordHitPathBoundingBoxes = true;
            for (int i = 0; i < 50; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    Items.Add(new SphereModel(new Vector3(i - 50, j - 25, i + j - 75), rnd.NextDouble(1,3)));
                }
            }

            var b3 = new LineBuilder();
            for (int i = 0; i < 10; ++i)
            {
                for(int j =0; j< 5; ++j)
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
            return list;
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

        public void OnMouseLeftButtonDownHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (var item in HighlightItems)
            {
                item.Highlight = false;
            }
            HighlightItems.Clear();
            Material = PhongMaterials.White;
            var viewport = sender as Viewport3DX;
            if (viewport == null) { return; }
            var point = e.GetPosition(viewport);
            var hitTests = viewport.FindHits(point);
            if (hitTests != null && hitTests.Count > 0)
            {
                if (HitThrough)
                {
                    foreach (var hit in hitTests)
                    {
                        if ((hit.ModelHit as Element3D).DataContext is DataModel)
                        {
                            var model = (hit.ModelHit as Element3D).DataContext as DataModel;
                            model.Highlight = true;
                            HighlightItems.Add(model);
                        }
                        else if ((hit.ModelHit as Element3D).DataContext == this)
                        {
                            if (hit.TriangleIndices != null)
                            {
                                Material = PhongMaterials.Yellow;
                            }
                            else
                            {
                                var v = new Vector3Collection();
                                v.Add(hit.PointHit);
                                PointsHitModel.Positions = v;
                                var idx = new IntCollection();
                                idx.Add(0);
                                PointsHitModel = new PointGeometry3D() { Positions = v, Indices = idx };
                            }
                        }
                    }
                }
                else
                {
                    var hit = hitTests[0];
                    if ((hit.ModelHit as Element3D).DataContext is DataModel)
                    {
                        var model = (hit.ModelHit as Element3D).DataContext as DataModel;
                        model.Highlight = true;
                        HighlightItems.Add(model);
                    }
                    else if ((hit.ModelHit as Element3D).DataContext == this)
                    {
                        if (hit.TriangleIndices != null)
                        {
                            Material = PhongMaterials.Yellow;
                        }
                        else
                        {
                            var v = new Vector3Collection();
                            v.Add(hit.PointHit);
                            PointsHitModel.Positions = v;
                            var idx = new IntCollection();
                            idx.Add(0);
                            PointsHitModel = new PointGeometry3D() { Positions = v, Indices = idx };
                        }
                    }
                }
            }
        }

        private double theta = 0;
        private double newModelZ = -5;
        private void AddModel(object o)
        {
            var x = 10 * (float)Math.Sin(theta);
            var y = 10 * (float)Math.Cos(theta);
            theta += 0.3;
            newModelZ += 0.5;
            var z = (float)(newModelZ);
            Items.Add(new SphereModel(new Vector3(x, y + 20, z + 14), 1));
        }

        private void RemoveModel(object o)
        {
            if (Items.Count > 0)
            {
                Items.RemoveAt(Items.Count - 1);
                newModelZ = newModelZ > -5 ? newModelZ - 0.5 : 0;
            }
        }

        private void ClearModel(object o)
        {
            Items.Clear();
            HighlightItems.Clear();
        }

        private DispatcherTimer timer;
        private int counter = 0;
        private bool autoTesting = false;
        public bool AutoTesting
        {
            set
            {
                if (SetValue<bool>(ref autoTesting, value, nameof(AutoTesting)))
                {
                    Enabled = !value;
                }
            }
            get
            {
                return autoTesting;
            }
        }

        private bool enabled = true;
        public bool Enabled
        {
            set
            {
                SetValue<bool>(ref enabled, value, nameof(Enabled));
            }
            get
            {
                return enabled;
            }
        }
        private Random rnd = new Random();
        private void AutoTestAddRemove(object o)
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (counter > 499)
            {
                counter = -500;
            }
            if (counter < 0)
            {
                RemoveModel(null);
            }
            else
            {
                AddModel(null);
            }
            if (counter % 2 == 0)
            {
                int k = rnd.Next(0, Items.Count - 1);
                int radius = rnd.Next(1, 5);
                (Items[k] as SphereModel).Radius = radius;
            }
            ++counter;
        }

        protected override void Dispose(bool disposing)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
            }

            base.Dispose(disposing);
        }
    }
}
