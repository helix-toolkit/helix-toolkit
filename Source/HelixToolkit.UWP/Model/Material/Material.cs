/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.ComponentModel;
using System.Runtime.Serialization;
using Windows.UI.Xaml;

namespace HelixToolkit.UWP
{
    using Model;

    [DataContract]
    public abstract class Material : DependencyObject
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
            return m == null ? null : m.Core;
        }
    }
}
