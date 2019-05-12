// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoreWpfTest
{
    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.SharpDX.Core.Model.Scene;
    using System.Windows;
    using FileLoadDemo;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            view.AddHandler(Element3D.MouseDown3DEvent, new RoutedEventHandler((s,e)=> 
            {
                var arg = e as MouseDown3DEventArgs;

                if(arg.HitTestResult == null)
                {
                    return;
                }
                if(arg.HitTestResult.ModelHit is SceneNode node && node.Tag is AttachedNodeViewModel vm)
                {
                    vm.Selected = !vm.Selected;
                }
            }));
        }
    }
}