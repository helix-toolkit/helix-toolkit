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
    using System.Windows.Media;

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
            this.FixedElementsPositonsBinding = new List<Element>
                            {
                                new Element
                                     {
                                         Positions = new Point3DCollection
                                         {
                                             new Point3D(-1, 1, 1.85),
                                             new Point3D(-1, -1, 1.85),
                                             new Point3D(1, -1, 1.85),
                                             new Point3D(1, 1, 1.85)
                                         }
                                     },
                            };
            this.DataContext = this;
            this.AddElementCommand = new DelegateCommand(() =>
            {
                if (this.ObservableElements.Count % 3 == 1)
                {
                    var modelBuilder = new MeshBuilder();
                    modelBuilder.AddCylinder(new Point3D(0, 0, 0), new Point3D(0, 1, 0), 0.75, 15);

                    ModelElement model = new ModelElement1();
                    if (this.ObservableElements.Count % 2 == 0)
                        model = new ModelElement2();

                    model.IsVisible = true;
                    model.Model = new GeometryModel3D
                    {
                        Material = new DiffuseMaterial(System.Windows.Media.Brushes.Orange),
                        BackMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Orange),
                        Geometry = modelBuilder.ToMesh()
                    };
                    model.Position = new Point3D(0, -3, this.ObservableElements.Count);

                    this.ObservableElements.Add(model);
                }
                else if (this.ObservableElements.Count % 2 == 0)
                {
                    this.ObservableElements.Add(new SphereElement
                    {
                        Position = new Point3D(-2, -3, this.ObservableElements.Count),
                        Material = Materials.Green,
                        Radius = 0.4
                    });
                }
                else
                {
                    this.ObservableElements.Add(new CubeElement
                    {
                        Position = new Point3D(2, -3, this.ObservableElements.Count)
                    });
                }
            });
            this.DeleteElementCommand = new DelegateCommand(() =>
            {
                this.ObservableElements.RemoveAt(this.ObservableElements.Count - 1);
            },
            () => this.ObservableElements.Count > 0);
            this.AddElementsCommand = new DelegateCommand(() =>
            {
                for (int a = 0; a < 250; a++)
                    AddElementCommand.Execute(null);
            });
            this.AddUIElementCommand = new DelegateCommand(() =>
            {
                ModelElement model = new ModelElement3
                {
                    IsVisible = true,
                    Color = Colors.Pink
                };
                model.Position = new Point3D(0, -3, this.ObservableElements.Count);

                this.ObservableElements.Add(model);
            });
        }

        public DelegateCommand AddElementCommand { get; private set; }
        public DelegateCommand AddElementsCommand { get; private set; }
        public DelegateCommand AddUIElementCommand { get; private set; }

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

        /// <summary>
        /// Gets or sets the fixed elements.
        /// </summary>
        /// <value>The fixed elements.</value>
        public IList<Element> FixedElementsPositonsBinding { get; set; }
    }
}
