﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Provides an observable collection of Element3D.
/// </summary>
public class ObservableElement3DCollection : ObservableCollection<Element3D>
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
