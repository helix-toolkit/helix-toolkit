#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI_NET5_0 
using Microsoft.UI.Xaml;

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

#if !NETFX_CORE && !NET5_0
        protected override Freezable CreateInstanceCore()
        {
            return new NormalVectorMaterial()
            {
                Name = Name
            };
        }
#endif
    }
}
