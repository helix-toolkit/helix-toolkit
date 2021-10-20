#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Model;
namespace HelixToolkit.WinUI
#else
using System.ComponentModel;
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model;
#endif
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

        public PositionColorMaterial()
        {
        }

        public PositionColorMaterial(PositionMaterialCore core) : base(core) { }
#if !NETFX_CORE && !WINUI
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