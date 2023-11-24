using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

#if WINUI
using HelixToolkit.WinUI.SharpDX.Elements2D;
#else
using HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
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
