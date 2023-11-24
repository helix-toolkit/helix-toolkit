using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Linq;
using System.Text;
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

namespace Viewport3DXCodeBehindTester;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly EffectsManager manager;
    private Viewport3DX? viewport;
    private readonly Models models = new();
    private readonly ViewModel viewmodel = new();
    private SceneNodeGroupModel3D? sceneNodeGroup;

    public MainWindow()
    {
        InitializeComponent();

        Closed += (s, e) => (DataContext as IDisposable)?.Dispose();

        manager = new DefaultEffectsManager();
        DataContext = viewmodel;
        buttonRemoveViewport.IsEnabled = false;
    }

    private void Button_Click_Add(object? sender, RoutedEventArgs e)
    {
        viewport?.Items.Add(models.GetModelRandom());
    }

    private void Button_Click_Remove(object? sender, RoutedEventArgs e)
    {
        var model = viewport?.Items.Last();
        if (model is Light3D)
        {
            return;
        }
        else if (model is EnvironmentMap3D)
        {
            viewmodel.EnableEnvironmentButtons = true;
        }
        viewport?.Items.RemoveAt(viewport.Items.Count - 1);
    }

    private void Button_Click_Initialize(object? sender, RoutedEventArgs e)
    {
        viewport = new Viewport3DX
        {
            BackgroundColor = Colors.Black,
            ShowCoordinateSystem = true,
            ShowFrameRate = true,
            EffectsManager = manager
        };
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

    private void Viewport_MouseDown3D(object? sender, RoutedEventArgs e)
    {
        if (e is MouseDown3DEventArgs args && args.HitTestResult != null)
        {
            var model = args.HitTestResult.ModelHit;
        }
    }

    private void buttonEnvironment_Click(object? sender, RoutedEventArgs e)
    {
        var texture = TextureModel.Create("Cubemap_Grandcanyon.dds");
        var environment = new EnvironmentMap3D() { Texture = texture };
        viewport?.Items.Add(environment);
        viewmodel.EnableEnvironmentButtons = false;
    }

    private void buttonSceneNode_Click(object? sender, RoutedEventArgs e)
    {
        sceneNodeGroup?.AddNode(models.GetSceneNodeRandom());
    }

    private void ButtonRemove_Click(object? sender, RoutedEventArgs e)
    {
        mainGrid.Children.Remove(viewport);
        buttonInit.IsEnabled = true;
        buttonRemoveViewport.IsEnabled = false;
    }
}