namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

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
        //internal void PreRenderSort()
        //{
        //    var comparer = new ElementComparer();
        //    this.Sort(comparer);
        //}
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