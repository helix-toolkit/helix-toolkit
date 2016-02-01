// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataTemplateDemo
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;
using System;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Creating Visual3Ds by a template.")]
    public partial class MainWindow : Window
    {
        private Random rand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.rand = new Random();
            this.InitializeComponent();
            this.Elements = new ObservableCollection<Element>
                             {
                                 new Element
                                     {
                                         Position = new Point3D(0, 0, 0), 
                                         Material = Materials.Red, 
                                         Radius = 1
                                     }, 
                                 new Element
                                     {
                                         Position = new Point3D(-0.757, 0.586, 0), 
                                         Material = Materials.White, 
                                         Radius = 0.6
                                     }, 
                                 new Element
                                     {
                                         Position = new Point3D(0.757, 0.586, 0), 
                                         Material = Materials.White, 
                                         Radius = 0.6
                                     }
                             };

            this.Elements2 = new List<Element>
                             {
                                 new Element
                                     {
                                         Position = new Point3D(0, 0, 0), 
                                         Material = Materials.Red, 
                                         Radius = 1
                                     }, 
                                 new Element
                                     {
                                         Position = new Point3D(-0.757, 0.586, 0), 
                                         Material = Materials.White, 
                                         Radius = 0.6
                                     }, 
                                 new Element
                                     {
                                         Position = new Point3D(0.757, 0.586, 0), 
                                         Material = Materials.White, 
                                         Radius = 0.6
                                     }
                             };
            this.DataContext = this;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public ObservableCollection<Element> Elements { get; set; }

        public IList<Element> Elements2 { get; set; }

        

        private void AddElemButton_Click(object sender, RoutedEventArgs e)
        {
            this.Elements.Add(new Element()
            {

                Position = new Point3D(this.rand.NextDouble(), this.rand.NextDouble(), 0), 
                Material = Materials.Green, 
                Radius = 1

            });
        }

        private void RemoveElemButton_Click(object sender, RoutedEventArgs e)
        {
            if ((this.Elements != null) && (this.Elements.Count > 0))
                this.Elements.RemoveAt(this.Elements.Count - 1);
        }
    }
}
