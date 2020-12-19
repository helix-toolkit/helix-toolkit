// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MorphTargetAnimationDemo
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mvm;

        public MainWindow()
        {
            InitializeComponent();
            mvm = new MainViewModel();
            this.DataContext = mvm;
            Closed += (s, e) => {
                if (DataContext is IDisposable)
                {
                    (DataContext as IDisposable).Dispose();
                }
            };

            System.Windows.Media.CompositionTarget.Rendering += OnRender;
        }

        private void OnRender(object sender, EventArgs e)
        {
            //Delete if we dont use this
        }
    }
}