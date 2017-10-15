// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Material.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows;

    public class MaterialPropertyChanged: EventArgs
    {
        public readonly string PropertyName;
        public MaterialPropertyChanged(string propertyName)
        {
            PropertyName = propertyName;
        }
    }

    [Serializable]
    public abstract class Material : DependencyObject
    {
        public delegate void OnPropertyChangedHandler(object sender, MaterialPropertyChanged e);
        public event OnPropertyChangedHandler OnMaterialPropertyChanged;

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Material), new UIPropertyMetadata(null));

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
            OnMaterialPropertyChanged?.Invoke(this, new MaterialPropertyChanged(e.Property.Name));
        }
    }

}