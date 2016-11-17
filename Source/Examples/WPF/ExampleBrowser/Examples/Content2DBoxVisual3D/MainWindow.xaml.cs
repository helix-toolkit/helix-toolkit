// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Content2DBoxVisual3DDemo
{
    using System;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example("Content2DBoxVisual3DDemo", "Easyly show six faces on a cube")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

    private void TopClicked(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("You Clicked Top");
    }

    private void BottomClick(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("You Clicked Bottom");
    }

    private void FrontClicked(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("You Clicked Bottom");
    }

    private void BackClicked(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("You Clicked Back");
    }

    private void RightClicked(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("You Clicked Right");
    }

    private void LeftClicked(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("You Clicked Left");
    }
  }
}