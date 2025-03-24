using System.Collections.Generic;
using System.Linq;

using HelixToolkit.Wpf;
using SRTM;

namespace ExampleBrowser.Examples.Terrain.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private Dictionary<string, string> _sources { get; set; } = new Dictionary<string, string>
    {
        { "BT (Binary Terrain)", "Examples/Terrain/Models/crater_0513.btz" },
        { "HGT (SRTM1 ~= 108 * 108 km)", "Examples/Terrain/Models/N43W123.hgt.zip" }, //SRTM1 is for North America
        { "HGT (SRTM3 ~= 108 * 108 km)", "Examples/Terrain/Models/N43E042.hgt.zip" },  //SRTM3 is for the rest of the world
        { "HGT (SRTM3 ~= 108 * 108 km (HGT-MODEL))", "Examples/Terrain/Models/N43E042.hgt.zip" }, // example for model load
    };

    public MainWindowViewModel()
    {
        SourceKeys = _sources.Keys.ToArray();
        SelectedSourceKey = SourceKeys.First();
        SelectedModel = null;
    }

    public string[] SourceKeys { get; set; }

    private string _selectedSourceKey = "";
    public string SelectedSourceKey
    {
        get => _selectedSourceKey;
        set
        {
            SetProperty(ref _selectedSourceKey, value);

            SelectedSource = "";
            SelectedModel = null;

            var source = _sources[value];

            if (value.Contains("HGT-MODEL"))
            {
                var model = new HgtTerrainModel
                {
                    Texture = new SlopeTexture(8)
                    {
                        Brush = GradientBrushes.GreenGrayWhite
                    }
                };
                var cell = new SRTMDataCell(source);
                model.Load(cell);
                SelectedModel = model;
            }
            else
                SelectedSource = source;
        }
    }

    private string _selectedSource = "";
    public string SelectedSource
    {
        get => _selectedSource;
        set => SetProperty(ref _selectedSource, value);
    }

    private ITerrainModel? _selectedModel;
    public ITerrainModel? SelectedModel
    {
        get => _selectedModel;
        set => SetProperty(ref _selectedModel, value);
    }
}
