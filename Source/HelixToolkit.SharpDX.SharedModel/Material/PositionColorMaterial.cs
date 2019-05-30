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
    /// Render color by mesh vertex position
    /// </summary>
    public sealed class PositionColorMaterial : Material
    {
        protected override MaterialCore OnCreateCore()
        {
            return PositionMaterialCore.Core;
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return new PositionColorMaterial()
            {
                Name = Name
            };
        }
#endif
    }
}