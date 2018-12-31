using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using HelixToolkit.Wpf.SharpDX.Core;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace GroupElementTester
{
    public class MainViewModel : BaseViewModel
    {
        private readonly Random rnd = new Random();
        public LineGeometry3D AxisModel { get; private set; }
        public BillboardText3D AxisLabel { private set; get; }

        public MeshGeometry3D SphereModel { private set; get; }

        public MeshGeometry3D BoxModel { private set; get; }

        public MeshGeometry3D ConeModel { private set; get; }

        public PhongMaterial RedMaterial { get; } = PhongMaterials.Red;
        public PhongMaterial BlueMaterial { get; } = PhongMaterials.Blue;
        public PhongMaterial GreenMaterial { get; } = PhongMaterials.Green;

        public Transform3D GroupModel3DTransform { private set; get; } = new Media3D.TranslateTransform3D(5, 0, 0);
        public Transform3D ItemsModel3DTransform { private set; get; } = new Media3D.TranslateTransform3D(0, 0, 5);

        public Transform3D Transform1 { get; } = new Media3D.TranslateTransform3D(0, 0, 0);

        public Transform3D Transform2 { get; } = new Media3D.TranslateTransform3D(-2, 0, 0);

        public Transform3D Transform3 { get; } = new Media3D.TranslateTransform3D(-4, 0, 0);

        public Transform3D Transform4 { get; } = new Media3D.TranslateTransform3D(-6, 0, 0);

        public ObservableElement3DCollection GroupModelSource { private set; get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection TransparentGroupModelSource { private set; get; } = new ObservableElement3DCollection();
        public ObservableCollection<MeshDataModel> ItemsSource { private set; get; } = new ObservableCollection<MeshDataModel>();

        private PhongMaterialCollection materialCollection = new PhongMaterialCollection();

        public ICommand AddGroupModelCommand { get; private set; }

        public ICommand RemoveGroupModelCommand { private set; get; }

        public ICommand AddTransparentGroupModelCommand { get; private set; }

        public ICommand RemoveTransparentGroupModelCommand { private set; get; }

        public ICommand ClearGroupModelCommand { private set; get; }

        public ICommand AnimateGroupModelCommand { private set; get; }

        public ICommand AddItemsModelCommand { get; private set; }

        public ICommand RemoveItemsModelCommand { private set; get; }
        public ICommand ClearItemsModelCommand { private set; get; }

        public ICommand AnimateItemsModelCommand { private set; get; }

        public ICommand ReplaceGroupSourceCommand { private set; get; }
        public ICommand ReplaceItemsModelSourceCommand { private set; get; }
        public MainViewModel()
        {
            //    RenderTechniquesManager = new DefaultRenderTechniquesManager();           
            EffectsManager = new DefaultEffectsManager();
            // ----------------------------------------------
            // titles
            this.Title = "GroupElement Test";
            this.SubTitle = "WPF & SharpDX";

            // ----------------------------------------------
            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(10, 2, 10), LookDirection = new Vector3D(-10, -2, -10), UpDirection = new Vector3D(0, 1, 0) };

            var lineBuilder = new LineBuilder();
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(10, 0, 0));
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 10, 0));
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 0, 10));
            AxisModel = lineBuilder.ToLineGeometry3D();
            AxisModel.Colors = new Color4Collection(AxisModel.Positions.Count);
            AxisModel.Colors.Add(Colors.Red.ToColor4());
            AxisModel.Colors.Add(Colors.Red.ToColor4());
            AxisModel.Colors.Add(Colors.Green.ToColor4());
            AxisModel.Colors.Add(Colors.Green.ToColor4());
            AxisModel.Colors.Add(Colors.Blue.ToColor4());
            AxisModel.Colors.Add(Colors.Blue.ToColor4());

            AxisLabel = new BillboardText3D();
            AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(11, 0, 0), Text = "X", Foreground = Colors.Red.ToColor4() });
            AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 11, 0), Text = "Y", Foreground = Colors.Green.ToColor4() });
            AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 0, 11), Text = "Z", Foreground = Colors.Blue.ToColor4() });

            var meshBuilder = new MeshBuilder(true);
            meshBuilder.AddSphere(new Vector3(0, 0, 0), 0.5);
            SphereModel = meshBuilder.ToMesh();
            meshBuilder = new MeshBuilder(true);
            meshBuilder.AddBox(Vector3.Zero, 0.5, 0.5, 0.5);
            BoxModel = meshBuilder.ToMesh();
            meshBuilder = new MeshBuilder(true);
            meshBuilder.AddCone(Vector3.Zero, new Vector3(0, 2, 0), 1, true, 24);
            ConeModel = meshBuilder.ToMesh();

            AddGroupModelCommand = new RelayCommand(AddGroupModel);
            RemoveGroupModelCommand = new RelayCommand(RemoveGroupModel);
            AddTransparentGroupModelCommand = new RelayCommand(AddTransparentMesh);
            RemoveTransparentGroupModelCommand = new RelayCommand(RemoveTransparentModel);
            ClearGroupModelCommand = new RelayCommand((o) => { GroupModelSource.Clear(); });
            AnimateGroupModelCommand = new RelayCommand(AnimateGroupModel);
            AddItemsModelCommand = new RelayCommand(AddItemsModel);
            RemoveItemsModelCommand = new RelayCommand(RemoveItemsModel);
            ClearItemsModelCommand = new RelayCommand((o) => { ItemsSource.Clear(); });
            AnimateItemsModelCommand = new RelayCommand(AnimateItemsModel);
            ReplaceGroupSourceCommand = new RelayCommand((o) => {
                GroupModelSource = new ObservableElement3DCollection();
                OnPropertyChanged(nameof(GroupModelSource));
            });
            ReplaceItemsModelSourceCommand = new RelayCommand((o) => 
            {
                ItemsSource = new ObservableCollection<MeshDataModel>();
                OnPropertyChanged(nameof(ItemsSource));
            });
        }

        private void AddGroupModel(object o)
        {
            var model = new MeshGeometryModel3D();
            model.Geometry = SphereModel;
            model.Material = BlueMaterial;
            model.Transform = new Media3D.TranslateTransform3D(0, (GroupModelSource.Count + 1) * 2, 0);
            GroupModelSource.Add(model);
        }

        private void RemoveGroupModel(object o)
        {
            if (GroupModelSource.Count > 0)
            { GroupModelSource.RemoveAt(GroupModelSource.Count - 1); }
        }

        private void AddItemsModel(object o)
        {
            var model = new MeshDataModel();
            model.Geometry = SphereModel;
            model.Material = GreenMaterial;
            model.Transform = new Media3D.TranslateTransform3D(0, - (ItemsSource.Count) * 2, 0);
            ItemsSource.Add(model);
        }

        private void RemoveItemsModel(object o)
        {
            if (ItemsSource.Count > 0)
            { ItemsSource.RemoveAt(ItemsSource.Count - 1); }
        }

        private void AnimateGroupModel(object o)
        {
            GroupModel3DTransform = CreateAnimatedTransform1(new Media3D.Transform3DGroup(), new Vector3D(5, 0, 0), new Vector3D(0, 1, 0));
            OnPropertyChanged(nameof(GroupModel3DTransform));
        }

        private void AnimateItemsModel(object o)
        {
            ItemsModel3DTransform = CreateAnimatedTransform1(new Media3D.Transform3DGroup(), new Vector3D(0, 0, 5), new Vector3D(0, 0, 1));
            OnPropertyChanged(nameof(ItemsModel3DTransform));
        }

        private static Media3D.Transform3D CreateAnimatedTransform1(Media3D.Transform3DGroup transformGroup,
            Media3D.Vector3D center, Media3D.Vector3D axis, double speed = 4)
        {
            var rotateAnimation1 = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                By = new Media3D.AxisAngleRotation3D(axis, 240),
                Duration = TimeSpan.FromSeconds(speed / 4),
                IsCumulative = true,
            };

            var rotateTransform1 = new Media3D.RotateTransform3D();
            rotateTransform1.CenterX = 0;
            rotateTransform1.CenterY = 0;
            rotateTransform1.CenterZ = 0;
            rotateTransform1.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation1);

            transformGroup.Children.Add(rotateTransform1);
            transformGroup.Children.Add(new TranslateTransform3D(center));

            return transformGroup;
        }

        private void AddTransparentMesh(object o)
        {
            var model = new MeshGeometryModel3D();
            int val = rnd.Next(0, 2);
            switch (val)
            {
                case 0:
                    model.Geometry = SphereModel;
                    break;
                case 1:
                    model.Geometry = BoxModel;
                    break;
                case 2:
                    model.Geometry = ConeModel;
                    break;
            }
            val = rnd.Next(0, materialCollection.Count - 1);
            var material = materialCollection[val];
            var diffuse = material.DiffuseColor;
            diffuse.Alpha = (float)rnd.Next(20, 60)/100f;
            material.DiffuseColor = diffuse;
            model.Material = material;
            model.Transform = new Media3D.TranslateTransform3D((float)rnd.Next(10, 100)/10, (float)rnd.Next(10, 100) / 10, (float)rnd.Next(10, 100) / 10);
            model.IsTransparent = true;
            TransparentGroupModelSource.Add(model);
        }

        private void RemoveTransparentModel(object o)
        {
            if (TransparentGroupModelSource.Count > 0)
            { TransparentGroupModelSource.RemoveAt(TransparentGroupModelSource.Count - 1); }
        }
    }

    public class MeshDataModel
    {
        public Geometry3D Geometry { set; get; }
        public Material Material { set; get; }
        public Transform3D Transform { set; get; }
    }
}
