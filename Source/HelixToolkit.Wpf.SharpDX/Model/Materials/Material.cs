// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Material.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using HelixToolkit.Wpf.SharpDX.Model;
    using System;
    using System.ComponentModel;
    using System.Windows;

    [Serializable]
    public abstract class Material : DependencyObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
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
                (d,e)=> { (d as Material).Core.Name = (string)e.NewValue; }));

        public string Name
        {
            get { return (string)this.GetValue(NameProperty); }
            set { this.SetValue(NameProperty, value); }
        }

        public override string ToString()
        {
            return Name;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.Property.Name));
        }

        protected abstract MaterialCore OnCreateCore();

        public static implicit operator MaterialCore(Material m)
        {
            return m == null ? null : m.Core;
        }
    }
}