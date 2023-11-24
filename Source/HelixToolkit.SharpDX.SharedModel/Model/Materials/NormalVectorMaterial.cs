using HelixToolkit.SharpDX.Model;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Render color by triangle normal
/// </summary>
public sealed class NormalVectorMaterial : Material
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NormalVectorMaterial"/> class.
    /// </summary>
    public NormalVectorMaterial()
    {

    }
    /// <summary>
    /// Initializes a new instance of the <see cref="NormalVectorMaterial"/> class.
    /// </summary>
    /// <param name="core">The core.</param>
    public NormalVectorMaterial(NormalMaterialCore core) : base(core)
    {

    }
    protected override MaterialCore OnCreateCore()
    {
        return NormalVectorMaterialCore.Core;
    }

#if WPF
    protected override Freezable CreateInstanceCore()
    {
        return new NormalVectorMaterial()
        {
            Name = Name
        };
    }
#endif
}
