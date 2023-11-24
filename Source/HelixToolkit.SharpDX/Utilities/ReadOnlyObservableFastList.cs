using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Utilities;

public sealed class ReadOnlyObservableFastList<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged, IEnumerable<T>, IEnumerable
{
    private readonly ObservableFastList<T> list;
    public ReadOnlyObservableFastList(ObservableFastList<T> list) : base(list)
    {
        this.list = list;
        list.CollectionChanged += List_CollectionChanged;
    }

    private void List_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
        if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
        {
            OnPropertyChanged(nameof(Count));
        }
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string info = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
    }
}
