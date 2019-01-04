using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace Viewport3DXCodeBehindTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EffectsManager manager;
        private Viewport3DX viewport;
        private Models models = new Models();
        private ViewModel viewmodel = new ViewModel();
        private SceneNodeGroupModel3D sceneNodeGroup;

        public MainWindow()
        {
            InitializeComponent();
            manager = new DefaultEffectsManager();
            DataContext = viewmodel;
            buttonRemoveViewport.IsEnabled = false;
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            viewport.Items.Add(models.GetModelRandom());
        }
        private void Button_Click_Remove(object sender, RoutedEventArgs e)
        {
            var model = viewport.Items.Last();
            if(model is Light3D)
            {
                return;
            }
            else if(model is EnvironmentMap3D)
            {
                viewmodel.EnableEnvironmentButtons = true;
            }
            viewport.Items.RemoveAt(viewport.Items.Count - 1);
        }

        private void Button_Click_Initialize(object sender, RoutedEventArgs e)
        {
            viewport = new Viewport3DX();
            viewport.BackgroundColor = Colors.Black;
            viewport.ShowCoordinateSystem = true;
            viewport.ShowFrameRate = true;
            viewport.EffectsManager = manager;
            viewport.Items.Add(new DirectionalLight3D() { Direction = new System.Windows.Media.Media3D.Vector3D(-1, -1, -1) });
            viewport.Items.Add(new AmbientLight3D() { Color = Color.FromArgb(255, 50, 50, 50) });
            sceneNodeGroup = new SceneNodeGroupModel3D();
            viewport.Items.Add(sceneNodeGroup);
            viewport.MouseDown3D += Viewport_MouseDown3D;
            Grid.SetColumn(viewport, 0);
            mainGrid.Children.Add(viewport);
            buttonInit.IsEnabled = false;
            buttonRemoveViewport.IsEnabled = true;
            viewmodel.EnableButtons = true;
        }

        private void Viewport_MouseDown3D(object sender, RoutedEventArgs e)
        {
            if(e is MouseDown3DEventArgs args && args.HitTestResult != null)
            {
                var model = args.HitTestResult.ModelHit;
            }
        }

        private void buttonEnvironment_Click(object sender, RoutedEventArgs e)
        {
            var texture = BaseViewModel.LoadFileToMemory("Cubemap_Grandcanyon.dds");
            var environment = new EnvironmentMap3D() { Texture = texture };
            viewport.Items.Add(environment);
            viewmodel.EnableEnvironmentButtons = false;
        }

        private void buttonSceneNode_Click(object sender, RoutedEventArgs e)
        {
            sceneNodeGroup.AddNode(models.GetSceneNodeRandom());
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Remove(viewport);
            buttonInit.IsEnabled = true;
            buttonRemoveViewport.IsEnabled = false;
        }
    }

    public class ViewModel : BaseViewModel
    {
        private bool enableButtons = false;
        public bool EnableButtons
        {
            set
            {
                if(SetValue(ref enableButtons, value))
                {
                    OnPropertyChanged(nameof(EnableEnvironmentButtons));
                }
            }
            get { return enableButtons; }
        }

        private bool enableEnvironmentButtons = true;
        public bool EnableEnvironmentButtons
        {
            set
            {
                SetValue(ref enableEnvironmentButtons, value);
            }
            get { return enableEnvironmentButtons && enableButtons; }
        }
    }

    public class Models
    {
        private IList<Geometry3D> models { get; } = new List<Geometry3D>();
        private PhongMaterialCollection materials = new PhongMaterialCollection();
        private Random rnd = new Random();
        public Models()
        {
            var builder = new MeshBuilder();
            builder.AddSphere(Vector3.Zero, 1);
            models.Add(builder.ToMesh());

            builder = new MeshBuilder();
            builder.AddBox(Vector3.Zero, 1, 1, 1);
            models.Add(builder.ToMesh());

            builder = new MeshBuilder();
            builder.AddDodecahedron(Vector3.Zero, new Vector3(1, 0, 0), new Vector3(0, 1, 0), 1);
            models.Add(builder.ToMesh());
        }

        public MeshGeometryModel3D GetModelRandom()
        {
            var idx = rnd.Next(0, models.Count);
            MeshGeometryModel3D model = new MeshGeometryModel3D() { Geometry = models[idx], CullMode = SharpDX.Direct3D11.CullMode.Back};
            var scale = new System.Windows.Media.Media3D.ScaleTransform3D(rnd.NextDouble(1, 5), rnd.NextDouble(1, 5), rnd.NextDouble(1, 5));
            var translate =  new System.Windows.Media.Media3D.TranslateTransform3D(rnd.NextDouble(-20, 20), rnd.NextDouble(-20, 20), rnd.NextDouble(-20, 20));
            var group = new System.Windows.Media.Media3D.Transform3DGroup();
            group.Children.Add(scale);
            group.Children.Add(translate);
            model.Transform = group;
            var material = materials[rnd.Next(0, materials.Count - 1)];
            model.Material = material;
            if (material.DiffuseColor.Alpha < 1)
            {
                model.IsTransparent = true;
            }
            return model;
        }

        public MeshNode GetSceneNodeRandom()
        {
            var idx = rnd.Next(0, models.Count);
            MeshNode model = new MeshNode() { Geometry = models[idx], CullMode = SharpDX.Direct3D11.CullMode.Back };
            var scale = SharpDX.Matrix.Scaling((float)rnd.NextDouble(1, 5), (float)rnd.NextDouble(1, 5), (float)rnd.NextDouble(1, 5));
            var translate = SharpDX.Matrix.Translation((float)rnd.NextDouble(-20, 20), (float)rnd.NextDouble(-20, 20), (float)rnd.NextDouble(-20, 20));
            model.ModelMatrix = scale * translate;
            var material = materials[rnd.Next(0, materials.Count - 1)];
            model.Material = material;
            if (material.DiffuseColor.Alpha < 1)
            {
                model.IsTransparent = true;
            }
            return model;
        }
    }
}
