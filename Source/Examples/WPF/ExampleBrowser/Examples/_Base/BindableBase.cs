using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExampleBrowser.Examples;

public class BindableBase : INotifyPropertyChanged
{
    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? property_name = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        RaisePropertyChanged(property_name);

        return true;
    }

    protected virtual bool SetPropertyAnyway<T>(ref T storage, T value, [CallerMemberName] string? property_name = null)
    {
        storage = value;
        RaisePropertyChanged(property_name);

        return true;
    }

    protected virtual void RaisePropertyChanged(string? property_name)
    {
        if (PropertyChanged != null)
        { PropertyChanged(this, new PropertyChangedEventArgs(property_name)); }
    }
    /// <summary>
    /// 
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
}
