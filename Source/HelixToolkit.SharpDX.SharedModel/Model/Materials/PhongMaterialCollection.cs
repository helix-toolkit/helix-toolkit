using System.Collections.ObjectModel;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class PhongMaterialCollection : ObservableCollection<PhongMaterial>
{
    public PhongMaterialCollection()
    {
        Add(PhongMaterials.Black);
        Add(PhongMaterials.BlackPlastic);
        Add(PhongMaterials.BlackRubber);
        Add(PhongMaterials.Blue);
        Add(PhongMaterials.Brass);
        Add(PhongMaterials.Bronze);
        Add(PhongMaterials.Chrome);
        Add(PhongMaterials.Copper);
        Add(PhongMaterials.DefaultVRML);
        Add(PhongMaterials.Emerald);
        Add(PhongMaterials.Glass);
        Add(PhongMaterials.Gold);
        Add(PhongMaterials.Green);
        Add(PhongMaterials.Indigo);
        Add(PhongMaterials.Jade);
        Add(PhongMaterials.Gray);
        Add(PhongMaterials.LightGray);
        Add(PhongMaterials.MediumGray);
        Add(PhongMaterials.Obsidian);
        Add(PhongMaterials.Orange);
        Add(PhongMaterials.Pearl);
        Add(PhongMaterials.Pewter);
        Add(PhongMaterials.PolishedBronze);
        Add(PhongMaterials.PolishedCopper);
        Add(PhongMaterials.PolishedGold);
        Add(PhongMaterials.PolishedSilver);
        Add(PhongMaterials.Red);
        Add(PhongMaterials.Ruby);
        Add(PhongMaterials.Silver);
        Add(PhongMaterials.Turquoise);
        Add(PhongMaterials.Violet);
        Add(PhongMaterials.White);
        Add(PhongMaterials.Yellow);
    }
}
