using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Model;

public abstract class ObservableObject : INotifyPropertyChanged
{
    private bool disablePropertyChangedEvent = false;
    public bool DisablePropertyChangedEvent
    {
        set
        {
            if (disablePropertyChangedEvent == value)
            {
                return;
            }
            disablePropertyChangedEvent = value;
            RaisePropertyChanged();
        }
        get
        {
            return disablePropertyChangedEvent;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void RaisePropertyChanged([CallerMemberName] string propertyName = StringHelper.EmptyStr)
    {
        if (!DisablePropertyChangedEvent)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        if (!DisablePropertyChangedEvent)
            PropertyChanged?.Invoke(this, args);
    }

    protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = StringHelper.EmptyStr)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
        {
            return false;
        }

        backingField = value;
        this.RaisePropertyChanged(propertyName);
        return true;
    }

    protected bool Set<T>(ref T backingField, T value, bool raisePropertyChanged, [CallerMemberName] string propertyName = StringHelper.EmptyStr)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
        {
            return false;
        }

        backingField = value;
        if (raisePropertyChanged)
        {
            this.RaisePropertyChanged(propertyName);
        }
        return true;
    }
}
