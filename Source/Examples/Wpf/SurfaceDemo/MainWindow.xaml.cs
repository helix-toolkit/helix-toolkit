using HelixToolkit.Wpf;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Resources;

namespace SurfaceDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ViewModel _viewModel = new();

    // todo: auto-enumerate resources??
    private readonly string[] models = {
                                               "Apple", "Boys surface", "Breather", "Conchoid", "Dini", "Enneper",
                                               "Klein bagel 2", "Klein bagel", "Klein bottle", "Mobius strip",
                                               "Plückers conoid", "Pseudocatenoid","Seashell surface", "Spring", "Steiner", "Toupie",
                                               "Tranguloid trefoil",
                                               "Trefoil", "Twisted torus", "Verrill", "Whitney umbrella"
                                           };

    private readonly ClonedVisual3D surface2;
    private bool _isInvalidated = true;
    private bool _isUpdating;
    private UIElement currentView;
    private readonly object updateLock = "abc";
    private Viewport3D? v1;
    private Viewport3D? v2;

    public MainWindow()
    {
        InitializeComponent();

        CompositionTarget.Rendering += this.OnCompositionTargetRendering;
        DataContext = _viewModel;

        _viewModel.PropertyChanged += ModelChanged;

        foreach (string m in models)
        {
            var uri = new Uri(String.Format("pack://application:,,,/Expressions/{0}.txt", m));
            AddToMenu(m, uri);
        }
        SearchForSurfaces("Expressions");

        view2.LeftViewport?.Children.Add(new DefaultLights());
        view2.RightViewport?.Children.Add(new DefaultLights());

        view3.LeftViewport?.Children.Add(new DefaultLights());
        view3.RightViewport?.Children.Add(new DefaultLights());

        view4.LeftViewport?.Children.Add(new DefaultLights());
        view4.RightViewport?.Children.Add(new DefaultLights());

        surface2 = new ClonedVisual3D();

        patternBrush = CreateDrawingBrush(0.02, 0.05);

        _viewModel.Brush = GradientBrushes.Hue;

        // normal mode
        v1 = view1.Viewport;
        v2 = null;
        currentView = view1;

        Loaded += Window1_Loaded;
    }

    private readonly Brush patternBrush;

    private void Window1_Loaded(object? sender, RoutedEventArgs? e)
    {
        SurfaceFile_Click(surfacesMenu.Items[0], null);
    }

    private void OnCompositionTargetRendering(object? sender, EventArgs? e)
    {
        if (_isInvalidated)
        {
            // sync:
            // _isInvalidated = false;
            // UpdateModel(source1.Text);

            // async:
            BeginUpdateModel();
        }
    }

    private void ModelChanged(object? sender, PropertyChangedEventArgs? e)
    {
        if (e?.PropertyName == "StereoBase")
        {
            return;
        }
        Invalidate();
    }

    private void SearchForSurfaces(string dir)
    {
        if (!Directory.Exists(dir))
            return;
        string[] files = Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories);

        if (files.Length > 0)
            surfacesMenu.Items.Add(new Separator());

        foreach (string file in files)
        {
            AddToMenu(Path.GetFileNameWithoutExtension(file), Path.GetFullPath(file));
        }
    }

    private void AddToMenu(string h, object f)
    {
        var mi = new MenuItem { Header = h, Tag = f };
        mi.Click += SurfaceFile_Click;
        surfacesMenu.Items.Add(mi);
    }

    private void SurfaceFile_Click(object? sender, RoutedEventArgs? e)
    {
        if (sender is MenuItem menu)
        {
            if (menu.Tag is Uri uri)
            {
                Load(uri);
            }
            else if (menu.Tag is string p)
            {
                Load(p);
            }
        }
    }

    private void Load(Uri uri)
    {
        try
        {
            StreamResourceInfo sri = Application.GetResourceStream(uri);
            Load(sri.Stream);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Cannot read model\n" + ex.Message);
        }
    }

    private void Load(string p)
    {
        var s = new FileStream(p, FileMode.Open);
        Load(s);
        s.Close();
        _viewModel.ModelTitle = Path.GetFileNameWithoutExtension(p);
    }

    private void Load(Stream s)
    {
        var r = new StreamReader(s);
        source1.Text = r.ReadToEnd();
        r.Close();

        Invalidate();

        // todo: binding didn't work
        view1.Title = _viewModel.ModelTitle;
        if (!String.IsNullOrEmpty(_viewModel.ModelTitle))
            Title = "Parametric surfaces - " + _viewModel.ModelTitle;
        view1.CameraController?.ResetCamera();
        view2.UpdateCameras();
    }

    private void Source1_TextChanged(object? sender, TextChangedEventArgs? e)
    {
        Invalidate();
    }

    private void Invalidate()
    {
        lock (updateLock)
        {
            _isInvalidated = true;
        }
    }

    private void Exit_Click(object? sender, RoutedEventArgs? e)
    {
        Close();
    }

    private void BeginUpdateModel()
    {
        lock (updateLock)
        {
            if (!_isUpdating)
            {
                _isInvalidated = false;
                _isUpdating = true;
                string src = source1.Text;
                Dispatcher.Invoke(new Action<string>(UpdateModel), src);
            }
        }
    }

    private void UpdateModel(string src)
    {
        UpdateSurface(surface1, src);
        if (_viewModel.ViewMode != ViewMode.Normal)
        {
            surface2.Source = null;
            surface2.Source = surface1;
        }

        errorList.ItemsSource = surface1.Errors;
        errorList.Visibility = surface1.HasErrors() ? Visibility.Visible : Visibility.Collapsed;

        _isUpdating = false;
    }

    //Brush currentBrush = CreateDrawingBrush(0.06,0.03);

    private void UpdateSurface(DynamicCodeSurface3D surface1, string src)
    {
        surface1.Source = null;
        surface1.MeshSizeU = _viewModel.MeshSizeU;
        surface1.MeshSizeV = _viewModel.MeshSizeV;
        surface1.ParameterW = _viewModel.ParameterW;

        // now the surface should be updated
        surface1.Source = src;

        source1.BorderBrush = surface1.HasErrors() ? Brushes.LightCoral : Brushes.Transparent;
    }

    private static Brush CreateDrawingBrush(double dx, double dy)
    {
        var db = new DrawingBrush
        {
            TileMode = TileMode.Tile,
            ViewportUnits = BrushMappingMode.Absolute,
            Viewport = new Rect(0, 0, dx, dy),
            Viewbox = new Rect(0, 0, 1, 1),
            ViewboxUnits = BrushMappingMode.Absolute
        };
        var dg = new DrawingGroup();
        dg.Children.Add(new GeometryDrawing { Geometry = new RectangleGeometry(new Rect(0, 0, 1, 1)), Brush = Brushes.White });
        dg.Children.Add(new GeometryDrawing
        {
            Geometry = new RectangleGeometry(new Rect(0.25, 0.25, 0.5, 0.5)),
            Brush = Brushes.Black
        });

        db.Drawing = dg;
        return db;
    }
    static Brush CreateImageBrush(string uri)
    {
        var img = new BitmapImage(new Uri(uri, UriKind.Relative));
        return new ImageBrush(img);
    }

    static Brush CreateVideoBrush(string uri)
    {
        var myMediaElement = new MediaElement
        {
            Source = new Uri(uri, UriKind.Relative),
            IsMuted = true
        };

        //            myMediaElement.Play();

        var myVisualBrush = new VisualBrush
        {
            Visual = myMediaElement
        };

        return myVisualBrush;
    }

    private void Stereo_Click(object? sender, RoutedEventArgs? e)
    {
        currentView.Visibility = Visibility.Collapsed;
        switch (_viewModel.ViewMode)
        {
            case ViewMode.Normal:
                currentView = view1;
                break;
            case ViewMode.Stereo:
                currentView = view2;
                break;
            case ViewMode.Anaglyph:
                currentView = view3;
                break;
            case ViewMode.Interlaced:
                currentView = view4;
                break;
        }
        currentView.Visibility = Visibility.Visible;

        v1?.Children.Remove(surface1);
        v2?.Children.Remove(surface2);

        switch (_viewModel.ViewMode)
        {
            case ViewMode.Normal:
                v1 = view1.Viewport;
                v2 = null;
                break;
            case ViewMode.Stereo:
                v1 = view2.LeftViewport;
                v2 = view2.RightViewport;
                break;
            case ViewMode.Anaglyph:
                v1 = view3.LeftViewport;
                v2 = view3.RightViewport;
                break;
            case ViewMode.Interlaced:
                v1 = view4.LeftViewport;
                v2 = view4.RightViewport;
                break;
        }

        v1?.Children.Add(surface1);
        v2?.Children.Add(surface2);
    }

    private void ViewSource_Click(object? sender, RoutedEventArgs? e)
    {
        // todo: move to viewmodel
        if (ViewSource.IsChecked)
        {
            SourcePanel.Visibility = Visibility.Visible;
            Grid.SetColumn(view1, 1);
            Grid.SetColumn(view2, 1);
            Grid.SetColumn(view3, 1);
            Grid.SetColumn(view4, 1);
            Grid.SetColumnSpan(view1, 1);
            Grid.SetColumnSpan(view2, 1);
            Grid.SetColumnSpan(view3, 1);
            Grid.SetColumnSpan(view4, 1);
        }
        else
        {
            SourcePanel.Visibility = Visibility.Collapsed;
            Grid.SetColumn(view1, 0);
            Grid.SetColumn(view2, 0);
            Grid.SetColumn(view3, 0);
            Grid.SetColumn(view4, 0);
            Grid.SetColumnSpan(view1, 2);
            Grid.SetColumnSpan(view2, 2);
            Grid.SetColumnSpan(view3, 2);
            Grid.SetColumnSpan(view4, 2);
        }
    }

    private void FullScreen_Click(object? sender, RoutedEventArgs? e)
    {
        // todo: move to viewmodel
        WindowState = WindowState.Normal;
        WindowStyle = Fullscreen.IsChecked ? WindowStyle.None : WindowStyle.ThreeDBorderWindow;
        WindowState = Fullscreen.IsChecked ? WindowState.Maximized : WindowState.Normal;
        ResizeMode = Fullscreen.IsChecked ? ResizeMode.NoResize : ResizeMode.CanResize;

        // mainMenu.Visibility = Fullscreen.IsChecked ? Visibility.Collapsed : Visibility.Visible;
    }

    private void Export_Click(object? sender, RoutedEventArgs? e)
    {
        var d = new SaveFileDialog
        {
            Filter = Exporters.Filter,
            DefaultExt = ".png"
        };
        if (d.ShowDialog() == true)
        {
            string ext = Path.GetExtension(d.FileName).ToLower();
            if (ext == ".xml")
            {
                view1.Export(d.FileName);
                string left = Path.ChangeExtension(d.FileName, "left.xml");
                string right = Path.ChangeExtension(d.FileName, "right.xml");
                view2.ExportKerkythea(left, right);
            }
            else
                view1.Export(d.FileName);

            string? path = Path.GetDirectoryName(d.FileName);

            if (path is not null)
            {
                Process.Start(path);
            }
        }
    }

    private void ExportStereo_Click(object? sender, RoutedEventArgs? e)
    {
        var d = new SaveFileDialog
        {
            Title = "Export stereo image(s)",
            Filter = Exporters.Filter,
            DefaultExt = "Left/right bitmap files (*.png;*.jpg)|*.png;*.jpg|MPO files (*.mpo)|*.mpo"
        };

        if (true == d.ShowDialog())
        {
            view1.ExportStereo(d.FileName, _viewModel.StereoBase);
            var directory = Path.GetDirectoryName(d.FileName);
            if (directory != null)
            {
                Process.Start(directory);
            }
        }
    }

    private void ResetCamera_Click(object? sender, RoutedEventArgs? e)
    {
        view1.CameraController?.ResetCamera();
    }

    private void BrushHue_Click(object? sender, RoutedEventArgs? e)
    {
        _viewModel.Brush = GradientBrushes.Hue;
    }
    private void BrushHueStripes_Click(object? sender, RoutedEventArgs? e)
    {
        _viewModel.Brush = GradientBrushes.HueStripes;
    }

    private void BrushRainbow_Click(object? sender, RoutedEventArgs? e)
    {
        _viewModel.Brush = GradientBrushes.Rainbow;
    }

    private void BrushRainbowStripes_Click(object? sender, RoutedEventArgs? e)
    {
        _viewModel.Brush = GradientBrushes.RainbowStripes;
    }

    private void BrushPattern_Click(object? sender, RoutedEventArgs? e)
    {
        _viewModel.Brush = patternBrush;
    }

    private void BrushImage_Click(object? sender, RoutedEventArgs? e)
    {
        var d = new OpenFileDialog()
        {
            Filter = "Image files (*.jpg;*.png)|*.jpg;*.png|All files(*.*)|*.*"
        };

        if (d.ShowDialog() == true)
        {
            _viewModel.Brush = CreateImageBrush(d.FileName);
        }
    }
}
