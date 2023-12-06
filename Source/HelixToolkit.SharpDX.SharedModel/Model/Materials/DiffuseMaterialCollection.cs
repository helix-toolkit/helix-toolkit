using System.Collections.ObjectModel;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public class DiffuseMaterialCollection : ObservableCollection<DiffuseMaterial>
{
    public DiffuseMaterialCollection()
    {
        Add(DiffuseMaterials.Black);
        Add(DiffuseMaterials.BlackPlastic);
        Add(DiffuseMaterials.BlackRubber);
        Add(DiffuseMaterials.Blue);
        Add(DiffuseMaterials.LightBlue);
        Add(DiffuseMaterials.SkyBlue);
        Add(DiffuseMaterials.Brass);
        Add(DiffuseMaterials.Bronze);
        Add(DiffuseMaterials.Chrome);
        Add(DiffuseMaterials.Copper);
        Add(DiffuseMaterials.DefaultVRML);
        Add(DiffuseMaterials.Emerald);
        Add(DiffuseMaterials.Glass);
        Add(DiffuseMaterials.Gold);
        Add(DiffuseMaterials.Green);
        Add(DiffuseMaterials.LightGreen);
        Add(DiffuseMaterials.Indigo);
        Add(DiffuseMaterials.Jade);
        Add(DiffuseMaterials.Gray);
        Add(DiffuseMaterials.LightGray);
        Add(DiffuseMaterials.MediumGray);
        Add(DiffuseMaterials.Obsidian);
        Add(DiffuseMaterials.Orange);
        Add(DiffuseMaterials.Pearl);
        Add(DiffuseMaterials.Pewter);
        Add(DiffuseMaterials.PolishedBronze);
        Add(DiffuseMaterials.PolishedCopper);
        Add(DiffuseMaterials.PolishedGold);
        Add(DiffuseMaterials.PolishedSilver);
        Add(DiffuseMaterials.Red);
        Add(DiffuseMaterials.Ruby);
        Add(DiffuseMaterials.Silver);
        Add(DiffuseMaterials.Turquoise);
        Add(DiffuseMaterials.Violet);
        Add(DiffuseMaterials.White);
        Add(DiffuseMaterials.Yellow);
    }
}
