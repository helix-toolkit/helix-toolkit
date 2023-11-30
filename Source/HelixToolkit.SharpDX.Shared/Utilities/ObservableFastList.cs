/*
The MIT License (MIT)
Copyright (c) 2023 Helix Toolkit contributors
*/
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
    {
        public sealed class ObservableFastList<T> : INotifyCollectionChanged, INotifyPropertyChanged, IList<T>
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;
            public event PropertyChangedEventHandler PropertyChanged;

            private readonly FastList<T> list;

            public int Count => list.Count;

            public bool IsReadOnly => false;

            public T this[int index] { get => list.Items[index]; set => list.Items[index] = value; }

            public ObservableFastList()
            {
                list = new FastList<T>();
            }

            public ObservableFastList(IEnumerable<T> collection)
            {
                list = new FastList<T>(collection);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            public ObservableFastList(int capacity)
            {
                list = new FastList<T>(capacity);
            }

            public void Add(T item)
            {
                list.Add(item);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                OnPropertyChanged(nameof(Count));
            }

            public bool Remove(T item)
            {
                if (list.Remove(item))
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                    OnPropertyChanged(nameof(Count));
                    return true;
                }
                return false;
            }

            public void RemoveAt(int index)
            {
                var item = list[index];
                list.RemoveAt(index);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                OnPropertyChanged(nameof(Count));
            }

            public void Move(int from, int to)
            {
                var itemFrom = list[from];
                list.RemoveAt(from);
                list.Insert(to, itemFrom);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list[to], to, from));
            }

            public void Insert(int index, T item)
            {
                list.Insert(index, item);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                OnPropertyChanged(nameof(Count));
            }

            public void Clear()
            {
                list.Clear();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnPropertyChanged(nameof(Count));
            }

            public int IndexOf(T item) => list.IndexOf(item);

            public bool Contains(T item) => list.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

            public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

            private void OnPropertyChanged([CallerMemberName] string info = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }
        }

        public sealed class ReadOnlyObservableFastList<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged, IEnumerable<T>, IEnumerable
        {
            private readonly ObservableFastList<T> list;
            public ReadOnlyObservableFastList(ObservableFastList<T> list) : base(list)
            {
                this.list = list;
                list.CollectionChanged += List_CollectionChanged;
            }

            private void List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                CollectionChanged?.Invoke(this, e);
                if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    OnPropertyChanged(nameof(Count));
                }
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string info = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}