// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TessellationDemo
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Transform3DGroup = System.Windows.Media.Media3D.Transform3DGroup;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using System;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
            Closed += (s, e) => {
                if (DataContext is IDisposable)
                {
                    (DataContext as IDisposable).Dispose();
                }
            };
        }
    }

}