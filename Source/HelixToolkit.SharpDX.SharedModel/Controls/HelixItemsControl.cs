using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETFX_CORE
using  Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace TT.HelixToolkit.UWP
#elif WINUI_NET5_0
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;

namespace HelixToolkit.WinUI
#else
using System.Windows;
using System.Windows.Controls;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    namespace Controls
    {
        public class HelixItemsControl : ItemsControl
        {
            public HelixItemsControl()
            {
#if NETFX_CORE
                ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.None;
#elif WINUI_NET5_0
                ManipulationMode = Microsoft.UI.Xaml.Input.ManipulationModes.None;
#else
                Focusable = false;
                Visibility = Visibility.Collapsed;
#endif
                IsHitTestVisible = false;
                this.DefaultStyleKey = typeof(HelixItemsControl);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                return new Size();
            }

            protected override Size MeasureOverride(Size availableSize)
            {
                return new Size();
            }
        }
    }

}
