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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            Closed += (s, e) => {
                if (DataContext is IDisposable)
                {
                    (DataContext as IDisposable).Dispose();
                }
            };
        }

        private void BlendValue0_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (DataContext as MainViewModel).SliderChanged(0, (float)e.NewValue);
        }

        private void BlendValue1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (DataContext as MainViewModel).SliderChanged(1, (float)e.NewValue);
        }

        private void BlendValue2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (DataContext as MainViewModel).SliderChanged(2, (float)e.NewValue);
        }

        private void BlendValue3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (DataContext as MainViewModel).SliderChanged(3, (float)e.NewValue);
        }
    }
}