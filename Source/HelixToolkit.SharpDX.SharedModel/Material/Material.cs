/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.Serialization;

#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.ComponentModel;
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;

    [DataContract]
#if NETFX_CORE
    public abstract class Material : DependencyObject
#else
    public abstract class Material : Freezable
#endif
    {
        private MaterialCore core;

        public MaterialCore Core
        {
            get
            {
                if (core == null)
                {
                    core = OnCreateCore();
                }
                return core;
            }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Material), new PropertyMetadata(null,
                (d, e) => { (d as Material).Core.Name = (string)e.NewValue; }));

        public string Name
        {
            get { return (string)this.GetValue(NameProperty); }
            set { this.SetValue(NameProperty, value); }
        }

        public override string ToString()
        {
            return Name;
        }

        protected abstract MaterialCore OnCreateCore();

        public static implicit operator MaterialCore(Material m)
        {
            return m?.Core;
        }
    }
}
