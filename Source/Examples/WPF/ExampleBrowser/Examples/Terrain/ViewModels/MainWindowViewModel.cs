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
        { "HGT (SRTM3 ~= 92.209 * 111.224 km)", "Examples/Terrain/Models/N34W119.hgt.zip" }, // SRTM3 file of part of North America
        { "HGT (SRTM3 ~= 81.343 * 111.224 km)", "Examples/Terrain/Models/N43E042.hgt.zip" },  // SRTM3 file of part of Eurasia (Elbrus)
        { "HGT (SRTM3 ~= 81.343 * 111.224 km (HGT-MODEL))", "Examples/Terrain/Models/N43E042.hgt.zip" }, // example for model load
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
                    Texture = new MapTexture("c:\\Users\\alex-valchuk\\AppData\\Roaming\\Aerologos\\LOGOS\\bmps\\12-061.bmp")
                    {
                        Top = 100,
                        Bottom = -100,
                        Left = -100,
                        Right = 100
                    },
                    /*Texture = new SlopeTexture(8)
                    {
                        Brush = GradientBrushes.GreenGrayWhite
                    }*/
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
