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
using System.Windows.Input;
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

        public bool HitThrough {set; get;}

        public ICommand AddModelCommand { private set; get; }
        public ICommand RemoveModelCommand { private set; get; }

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
            this.PropertyChanged += MainViewModel_PropertyChanged;       

            LineColor = Color.Blue;
            GroupLineColor = Color.Red;
            Items = new ObservableCollection<DataModel>();
            CreateDefaultModels();

            AddModelCommand = new RelayCommand(AddModel);
            RemoveModelCommand = new RelayCommand(RemoveModel);
        }

        private void CreateDefaultModels()
        {
            var b2 = new MeshBuilder(true, true, true);
            b2.AddSphere(new Vector3(0f, 0f, 0f), 4, 64, 64);
            b2.AddSphere(new Vector3(5f, 0f, 0f), 2, 32, 32);
            b2.AddTube(new Vector3[] { new Vector3(0f, 5f, 0f), new Vector3(0f, 7f, 0f) }, 2, 12, false, true, true);
            var model = b2.ToMeshGeometry3D();

            model.UpdateOctree();
            OctreeModel = model.Octree.CreateOctreeLineModel();


            Items.Add(new DataModel() { Model = model });
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    var builder = new MeshBuilder(true, false, false);
                    builder.AddSphere(new Vector3(10f + i + (float)Math.Pow((float)j / 2, 2), 10f + (float)Math.Pow((float)i / 2, 2), 5f + (float)Math.Pow(j, ((float)i / 5))), 1, 12, 12);
                    model = builder.ToMeshGeometry3D();
                    model.UpdateOctree();
                    Items.Add(new DataModel() { Model = model });
                }
            }

            //for (int i = 0; i < 10; ++i)
            //{
            //    var builder = new MeshBuilder(true, false, false);
            //    builder.AddSphere(new Vector3(i * 2, 0, 0), 1);
            //    var model = builder.ToMeshGeometry3D();
            //    model.UpdateOctree();
            //    Items.Add(new DataModel() { Model = model });
            //}
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

        public void OnMouseLeftButtonDownHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewport = sender as Viewport3DX;
            if (viewport == null) { return; }
            var point = e.GetPosition(viewport);
            var hitTests = viewport.FindHits(point);
            if (hitTests != null && hitTests.Count > 0)
            {
                if (HitThrough)
                {
                    foreach(var hit in hitTests)
                    {
                        if (hit.ModelHit.DataContext is DataModel)
                        {
                            var model = hit.ModelHit.DataContext as DataModel;
                            model.Highlight = !model.Highlight;
                        }
                    }
                }
                else
                {
                    var hit = hitTests[0];
                    if (hit.ModelHit.DataContext is DataModel)
                    {
                        var model = hit.ModelHit.DataContext as DataModel;
                        model.Highlight = !model.Highlight;
                    }
                }
            }

        }

        private double theta = 0;
        private double newModelZ = -5;
        private int counter = 0;
        private void AddModel(object o)
        {
            var x = 10*(float)Math.Sin(theta);
            var y = 10*(float)Math.Cos(theta);
            theta += 0.3;
            newModelZ += counter*0.05;
            var z = (float)(newModelZ);
            var builder = new MeshBuilder(true, false, false);
            builder.AddSphere(new Vector3(x -14, y - 14, z - 14), 1);
            var model = builder.ToMeshGeometry3D();
            model.UpdateOctree();
            Items.Add(new DataModel() { Model = model });
            ++counter;
        }

        private void RemoveModel(object o)
        {
            if (Items.Count > 0)
            {
                Items.RemoveAt(Items.Count - 1);
            }
        }
    }
}
