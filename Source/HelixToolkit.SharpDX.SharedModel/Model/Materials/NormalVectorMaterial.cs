using HelixToolkit.SharpDX.Model;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
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

#if false
#elif WINUI
#elif WPF
    protected override Freezable CreateInstanceCore()
    {
        return new NormalVectorMaterial()
        {
            Name = Name
        };
    }
#else
#error Unknown framework
#endif
}
