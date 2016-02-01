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
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    using PropertyTools.Wpf;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Creating Visual3Ds by a template.")]
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.ObservableElements = new ObservableCollection<Element>();
            this.FixedElements = new List<Element>
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
            this.AddElementCommand = new DelegateCommand(() =>
            {
                this.ObservableElements.Add(new Element
                {
                    Position = new Point3D(0, -3, this.ObservableElements.Count),
                    Material = Materials.Green,
                    Radius = 0.4
                });
            });
            this.DeleteElementCommand = new DelegateCommand(() =>
            {
                this.ObservableElements.RemoveAt(this.ObservableElements.Count - 1);
            },
            () => this.ObservableElements.Count > 0);
        }

        public DelegateCommand AddElementCommand { get; private set; }

        public DelegateCommand DeleteElementCommand { get; private set; }

        /// <summary>
        /// Gets or sets the observable elements.
        /// </summary>
        public ObservableCollection<Element> ObservableElements { get; set; }

        /// <summary>
        /// Gets or sets the fixed elements.
        /// </summary>
        /// <value>The fixed elements.</value>
        public IList<Element> FixedElements { get; set; }
    }
}
