using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

#if WINUI
using HelixToolkit.SharpDX.Core;
namespace HelixToolkit.WinUI.Elements2D
#else
namespace HelixToolkit.UWP.Elements2D
#endif
{
    using Core2D;
    using Extensions;

    public abstract class Element2D : Element2DCore, IHitable2D
    {
    }
}
