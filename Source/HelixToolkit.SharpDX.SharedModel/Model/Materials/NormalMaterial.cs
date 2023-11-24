using HelixToolkit.SharpDX.Model;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Render color by triangle normal
/// </summary>
public sealed class NormalMaterial : Material
{
    public NormalMaterial()
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="NormalMaterial"/> class.
    /// </summary>
    /// <param name="core">The core.</param>
    public NormalMaterial(NormalMaterialCore core) : base(core)
    {

    }

    /// <summary>
    /// Called when [create core].
    /// </summary>
    /// <returns></returns>
    protected override MaterialCore OnCreateCore()
    {
        return NormalMaterialCore.Core;
    }

#if WPF
    protected override Freezable CreateInstanceCore()
    {
        return new NormalMaterial()
        {
            Name = Name
        };
    }
#endif
}
