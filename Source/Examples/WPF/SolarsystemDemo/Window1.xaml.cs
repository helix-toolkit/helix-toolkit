// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window1.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace SolarsystemDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        SolarSystem3D SolarSystem;

        public Window1()
        {
            InitializeComponent();

            view1.Camera.Position = new Point3D(0, 400, 500);
            view1.Camera.LookDirection = new Vector3D(0, -400, -500);
            SolarSystem = view1.Children[2] as SolarSystem3D;
            DataContext = SolarSystem;

            Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            SolarSystem.InitModel();
            SolarSystem.UpdateModel();
        }

    }
}