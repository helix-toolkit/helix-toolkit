// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using HelixToolkit.Wpf.SharpDX;

namespace CustomShaderDemo
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();                                  
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach(var item in view1.Items)
            {
                if(item is GeometryModel3D)
                {
                    var geom = item as GeometryModel3D;
                    geom.SetValue(AttachedProperties.ShowSelectedProperty, !(bool)geom.GetValue(AttachedProperties.ShowSelectedProperty));
                }
            }
        }
    }

}