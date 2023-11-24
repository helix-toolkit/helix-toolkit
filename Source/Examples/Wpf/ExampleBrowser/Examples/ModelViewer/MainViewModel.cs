using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace ModelViewer;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public sealed partial class MainViewModel : ObservableObject
{
    private const string OpenFileFilter = "3D model files (*.3ds;*.obj;*.off;*.lwo;*.stl;*.ply;)|*.3ds;*.obj;*.objz;*.off;*.lwo;*.stl;*.ply;";

    private const string TitleFormatString = "3D model viewer - {0}";

    private readonly IFileDialogService fileDialogService;

    private readonly IHelixViewport3D viewport;

    private readonly Dispatcher dispatcher;

    [ObservableProperty]
    private string? currentModelPath;

    [ObservableProperty]
    private string? applicationTitle;

    [ObservableProperty]
    private double expansion;

    [ObservableProperty]
    private Model3D? currentModel;

    public MainViewModel(IFileDialogService fds, HelixViewport3D viewport)
    {
        Guard.IsNotNull(viewport);

        this.dispatcher = Dispatcher.CurrentDispatcher;
        this.Expansion = 1;
        this.fileDialogService = fds;
        this.viewport = viewport;
        this.ApplicationTitle = "3D Model viewer";
        this.Elements = new List<VisualViewModel>();
        foreach (var c in viewport.Children)
        {
            this.Elements.Add(new VisualViewModel(c));
        }

        // defaults: just to make sure it works
        var modelImporter = new ModelImporter();
        //this.CurrentModel = modelImporter.Load("pack://application:,,,/ExampleBrowser;component/Resources/simple3dModel.obj");
        //this.CurrentModel = modelImporter.Load("pack://application:,,,/ExampleBrowser;component/Resources/simple3dModel.objz");
        //this.CurrentModel = modelImporter.Load("pack://application:,,,/ExampleBrowser;component/Resources/simple3dModel.off");
        //this.CurrentModel = modelImporter.Load("pack://application:,,,/ExampleBrowser;component/Resources/simple3dModel.ply");
        this.CurrentModel = modelImporter.Load("pack://application:,,,/ExampleBrowser;component/Resources/simple3dModel.stl");
    }

    public List<VisualViewModel> Elements { get; set; }

    [RelayCommand]
    private static void FileExit()
    {
        Application.Current.Shutdown();
    }

    [RelayCommand]
    private void FileExport()
    {
        var path = this.fileDialogService.SaveFileDialog(null, null, Exporters.Filter, ".png");
        if (path == null)
        {
            return;
        }

        this.viewport.Export(path);
    }

    [RelayCommand]
    private void CopyXaml()
    {
        if (this.CurrentModel is null)
            return;
        var rd = XamlExporter.WrapInResourceDictionary(this.CurrentModel);
        Clipboard.SetText(XamlHelper.GetXaml(rd));
    }

    [RelayCommand]
    private void ViewZoomExtents()
    {
        this.viewport.ZoomExtents(500);
    }

    [RelayCommand]
    private async Task FileOpen()
    {
        this.CurrentModelPath = this.fileDialogService.OpenFileDialog("models", null, OpenFileFilter, ".3ds");

        if (this.CurrentModelPath is null)
        {
            return;
        }

        this.CurrentModel = await this.LoadAsync(this.CurrentModelPath, true);
        this.ApplicationTitle = string.Format(TitleFormatString, this.CurrentModelPath);
        this.viewport.ZoomExtents(0);
    }

    private async Task<Model3DGroup?> LoadAsync(string model3DPath, bool freeze)
    {
        return await Task.Factory.StartNew(() =>
        {
            var mi = new ModelImporter();

            if (freeze)
            {
                // Alt 1. - freeze the model 
                return mi.Load(model3DPath, null, true);
            }

            // Alt. 2 - create the model on the UI dispatcher
            return mi.Load(model3DPath, this.dispatcher);
        });
    }
}
