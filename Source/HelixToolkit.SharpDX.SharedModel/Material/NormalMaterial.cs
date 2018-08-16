#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.ComponentModel;
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    /// <summary>
    /// Render color by triangle normal
    /// </summary>
    public sealed class NormalMaterial : Material
    {
        protected override MaterialCore OnCreateCore()
        {
            return NormalMaterialCore.Core;
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return new NormalMaterial()
            {
                Name = Name
            };
        }
#endif
    }
}
