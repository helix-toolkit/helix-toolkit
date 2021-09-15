using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINUI
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
{
    using System.Collections.ObjectModel;
    public sealed class ManipulationBindingCollection : ObservableCollection<ManipulationBinding>
    {
    }
}
