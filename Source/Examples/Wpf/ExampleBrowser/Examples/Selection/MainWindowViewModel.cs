using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Selection;

public sealed partial class MainWindowViewModel : ObservableObject
{
    private IList<Model3D>? selectedModels;
    private IList<Visual3D>? selectedVisuals;

    public MainWindowViewModel(Viewport3D viewport)
    {
        this.PointSelectionCommand = new PointSelectionCommand(viewport, this.HandleSelectionModelsEvent, this.HandleSelectionVisualsEvent);
        this.RectangleSelectionCommand = new RectangleSelectionCommand(viewport, this.HandleSelectionModelsEvent, this.HandleSelectionVisualsEvent);
        this.CombinedSelectionCommand = new CombinedSelectionCommand(viewport, this.HandleSelectionModelsEvent, this.HandleSelectionVisualsEvent);

        this.CombinedSelectionCommand.FillRectangleBrush = new SolidColorBrush(Colors.Green) { Opacity = 0.5 };
    }

    [ObservableProperty]
    private bool isPointSelectionCommand = true;

    [ObservableProperty]
    private bool isRectangleSelectionCommand;

    [ObservableProperty]
    private bool isCombinedSelectionCommand;

    public bool AllowAutoSetSelectionHitMode
    {
        get
        {
            return CombinedSelectionCommand.AllowAutoSetSelectionHitMode;
        }
        set
        {
            CombinedSelectionCommand.AllowAutoSetSelectionHitMode = value;
            OnPropertyChanged();
        }
    }

    public PointSelectionCommand PointSelectionCommand { get; private set; }

    public RectangleSelectionCommand RectangleSelectionCommand { get; private set; }

    public CombinedSelectionCommand CombinedSelectionCommand { get; private set; }

    public SelectionHitMode SelectionMode
    {
        get
        {
            return this.RectangleSelectionCommand.SelectionHitMode;
        }

        set
        {
            this.RectangleSelectionCommand.SelectionHitMode = value;
            OnPropertyChanged();
        }
    }

    public SelectionHitMode CombinedSelectionMode
    {
        get
        {
            return this.CombinedSelectionCommand.SelectionHitMode;
        }

        set
        {
            this.CombinedSelectionCommand.SelectionHitMode = value;
            OnPropertyChanged();
        }
    }

    public IEnumerable<SelectionHitMode> SelectionModes
    {
        get
        {
            return Enum.GetValues(typeof(SelectionHitMode)).Cast<SelectionHitMode>();
        }
    }

    public string SelectedVisuals
    {
        get
        {
            return selectedVisuals is null ? "" : string.Join("; ", selectedVisuals.Select(x => x.GetType().Name));
        }
    }

    private void HandleSelectionVisualsEvent(object? sender, VisualsSelectedEventArgs args)
    {
        this.selectedVisuals = args.SelectedVisuals.Where(t => t is not null).ToList()!;
        OnPropertyChanged(nameof(SelectedVisuals));

        CombinedSelectionMode = CombinedSelectionCommand.SelectionHitMode;
    }

    private void HandleSelectionModelsEvent(object? sender, ModelsSelectedEventArgs args)
    {
        this.ChangeMaterial(this.selectedModels, Materials.Blue);
        this.selectedModels = args.SelectedModels.Where(t => t is not null).ToList()!;

        if (args is ModelsSelectedByRectangleEventArgs rectangleSelectionArgs)
        {
            this.ChangeMaterial(
                this.selectedModels,
                rectangleSelectionArgs.Rectangle.Size != default ? Materials.Red : Materials.Green);
        }
        else
        {
            this.ChangeMaterial(this.selectedModels, Materials.Orange);
        }
    }

    private void ChangeMaterial(IEnumerable<Model3D>? models, Material material)
    {
        if (models is null)
        {
            return;
        }

        foreach (var model in models)
        {
            if (model is GeometryModel3D geometryModel)
            {
                geometryModel.Material = geometryModel.BackMaterial = material;
            }
        }
    }
}
