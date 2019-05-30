// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileLoadDemo
{
    using HelixToolkit.Wpf.SharpDX;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeometryModel3D selectedModel;

        public MainWindow()
        {
            InitializeComponent();
            var Main = new MainViewModel();
             Main.modelView = this.view;
            this.DataContext = Main;
            view.AddHandler(Element3D.MouseDown3DEvent, new RoutedEventHandler((s,e)=> 
            {
                var arg = e as MouseDown3DEventArgs;

                if(arg.HitTestResult == null)
                {
                    return;
                }
                if(selectedModel != null)
                {
                    selectedModel.PostEffects = null;
                    selectedModel = null;
                }
                selectedModel = arg.HitTestResult.ModelHit as GeometryModel3D;
                if(selectedModel != null)
                {
                    selectedModel.PostEffects = string.IsNullOrEmpty(selectedModel.PostEffects) ? $"highlight[color:#FFFF00]" : null;
                }
            }));
        }
    }
}