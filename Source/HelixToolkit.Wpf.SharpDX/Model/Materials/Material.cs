namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows;

    [Serializable]
    public abstract class Material : DependencyObject
    {
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
    }

}