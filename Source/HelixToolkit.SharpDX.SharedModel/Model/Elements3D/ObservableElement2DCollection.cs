using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Elements2D;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class ObservableElement2DCollection : ObservableCollection<Element2D>
{
    protected override void ClearItems()
    {
        CheckReentrancy();
        var items = Items.ToArray();
        base.ClearItems();
        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, -1));
    }
}
