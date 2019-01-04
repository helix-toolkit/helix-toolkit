/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
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
    namespace Model
    {
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

            public event PropertyChangedEventHandler PropertyChanged;
            protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
            {
                if(!DisablePropertyChangedEvent)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
            {
                if (EqualityComparer<T>.Default.Equals(backingField, value))
                {
                    return false;
                }

                backingField = value;
                this.RaisePropertyChanged(propertyName);
                return true;
            }

            protected bool Set<T>(ref T backingField, T value, bool raisePropertyChanged, [CallerMemberName] string propertyName = "")
            {
                if (EqualityComparer<T>.Default.Equals(backingField, value))
                {
                    return false;
                }

                backingField = value;
                if (raisePropertyChanged)
                { this.RaisePropertyChanged(propertyName); }
                return true;
            }
        }
    }

}
