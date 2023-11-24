using CommunityToolkit.Mvvm.Input;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Win32;
using SharpDX;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RenderTechniqueImportExportDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    private const string OpenFileFilter = "Techniques file (*.techniques;|*.techniques";

    public MeshGeometry3D MeshModel { private set; get; }

    public Material MeshMaterial { get; } = PhongMaterials.Jade;

    public LineGeometry3D LineModel { private set; get; }

    public PointGeometry3D PointModel { private set; get; }

    public ObservableCollection<string> TechniqueList { get; } = new();

    public string SelectedTechnique { set; get; } = string.Empty;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        var builder = new MeshBuilder();
        builder.AddSphere(new Vector3().ToVector(), 2);
        builder.AddTorus(5, 1);
        MeshModel = builder.ToMesh().ToMeshGeometry3D();

        var lineBuilder = new LineBuilder();
        lineBuilder.AddGrid(BoxFaces.All, 10, 10, 10, 10);
        LineModel = lineBuilder.ToLineGeometry3D();

        var offset = new Vector3(-4, 0, 0);
        PointModel = new PointGeometry3D()
        {
            Positions = MeshModel.Positions is null ? null : new Vector3Collection(MeshModel.Positions.Select(x => x + offset))
        };
    }

    private void Export(string technique)
    {
        var path = MainViewModel.CreateFileDialog(OpenFileFilter, technique);

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (string.IsNullOrEmpty(technique))
        {
            EffectsManager?.ExportTechniquesAsBinary(path);
        }
        else
        {
            EffectsManager?.ExportTechniqueAsBinary(technique, path);
        }
    }

    [RelayCommand]
    private void Export()
    {
        Export(string.Empty);
    }

    [RelayCommand]
    private void Import()
    {
        var path = MainViewModel.OpenFileDialog(OpenFileFilter);
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        var manager = EffectsManager;
        EffectsManager = null;
        manager?.ImportTechniques(path, true);
        EffectsManager = manager;
        TechniqueList.Clear();

        if (EffectsManager is not null)
        {
            foreach (var tech in EffectsManager.RenderTechniques)
            {
                TechniqueList.Add(tech);
            }
        }
    }

    [RelayCommand]
    private void ExportSingleTechnique()
    {
        Export(SelectedTechnique);
    }

    private static string? OpenFileDialog(string filter)
    {
        var d = new OpenFileDialog();
        d.CustomPlaces.Clear();

        d.Filter = filter;
        d.InitialDirectory = Environment.CurrentDirectory;

        if (d.ShowDialog() != true)
        {
            return null;
        }

        return d.FileName;
    }

    private static string? CreateFileDialog(string filter, string fileName)
    {
        var d = new SaveFileDialog();
        d.CustomPlaces.Clear();

        d.Filter = filter;
        d.FileName = fileName;
        d.DefaultExt = "techniques";

        if (d.ShowDialog() != true)
        {
            return null;
        }

        return d.FileName;
    }
}
