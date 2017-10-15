// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Element3DCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a collection of Element3D.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    /// <summary>
    /// Provides a collection of Element3D.
    /// </summary>
    public class Element3DCollection : List<Element3D>
    {
        //internal void PreRenderSort()
        //{
        //    var comparer = new ElementComparer();
        //    this.Sort(comparer);
        //}
    }

    /// <summary>
    /// Provides an observable collection of Element3D.
    /// </summary>
    public class ObservableElement3DCollection : ObservableCollection<Element3D>
    {
        protected override void ClearItems()
        {
            CheckReentrancy();
            var items = Items.ToList();
            base.ClearItems();
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, -1));
        }
    }

    //public class ElementComparer : IComparer
    //{
    //    // Calls CaseInsensitiveComparer.Compare with the parameters reversed. 
    //    int IComparer.Compare(object x, object y)
    //    {
    //        return ((new CaseInsensitiveComparer()).Compare(y, x));
    //    }
    //}
}