// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaterialControl.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MaterialControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EnvironmentMapDemo
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Data;
    using HelixToolkit.Mathematics;
    using HelixToolkit.Wpf.SharpDX;

    /// <summary>
    /// Interaction logic for MaterialControl.xaml
    /// </summary>
    public partial class MaterialControl : UserControl
    {
        public MaterialControl()
        {
            InitializeComponent();            
        }
    }

    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var c = (Color4)value;
            return c.ToColor();      
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var c = (System.Windows.Media.Color)value;
            return c.ToColor4(); 
        }
    }
}