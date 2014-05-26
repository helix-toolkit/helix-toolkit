// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit examples">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DataTemplateDemo
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Creating Visual3Ds by a template.")]
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.Elements = new List<Element>
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
        public IList<Element> Elements { get; set; }
    }
}