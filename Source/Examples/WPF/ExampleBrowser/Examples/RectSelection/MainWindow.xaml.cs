// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit examples">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RectSelection
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;
    using HelixToolkit.Wpf.Selections;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example("Rectangle Selection", "Test for rectangle selection.")]
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The select command property.
        /// </summary>
        private static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register(
                "SelectCommand",
                typeof(SelectionCommand),
                typeof(MainWindow),
                new PropertyMetadata());

        private static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
            "SelectionMode",
            typeof(SelectionHitMode),
            typeof(MainWindow),
            new PropertyMetadata(SelectionHitMode.Touch));

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.SelectCommand = new SelectByRectangleCommand(this.view1.Viewport, SelectionHitMode.Touch, this.OnModelsSelected);
            this.view1.InputBindings.Add(new MouseBinding(this.SelectCommand, new MouseGesture(MouseAction.LeftClick)));
            this.InitSelector();
            this.modeSelector.SelectedIndex = 0;

            this.InitModels();

            foreach (var item in this.view1.Viewport.Children)
            {
                var model = item as MeshElement3D;
                if (model != null)
                {
                    model.Material = Materials.Blue;
                    model.BackMaterial = Materials.Blue;
                }
            }
        }

        /// <summary>
        /// Gets or sets the select command.
        /// </summary>
        public SelectionCommand SelectCommand
        {
            get
            {
                return (SelectionCommand)this.GetValue(SelectCommandProperty);
            }

            set
            {
                this.SetValue(SelectCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        public SelectionHitMode SelectionMode
        {
            get
            {
                return (SelectionHitMode)this.GetValue(SelectionModeProperty);
            }

            set
            {
                this.SetValue(SelectionModeProperty, value);
            }
        }

        /// <summary>
        /// Gets the glass geometry.
        /// </summary>
        public MeshGeometry3D GlassGeometry
        {
            get
            {
                var builder = new MeshBuilder(true, true);
                var profile = new[] { new Point(0, 0.4), new Point(0.06, 0.36), new Point(0.1, 0.1), new Point(0.34, 0.1), new Point(0.4, 0.14), new Point(0.5, 0.5), new Point(0.7, 0.56), new Point(1, 0.46) };
                builder.AddRevolvedGeometry(profile, null, new Point3D(0, 0, 0), new Vector3D(0, 0, 1), 100);
                return builder.ToMesh(true);
            }
        }

        private void InitModels()
        {
            for (var i = 1; i < 5; i++)
            {
                var model = new SphereVisual3D
                                {
                                    Center = new Point3D(0, 3, 0),
                                    Radius = 0.1,
                                    Transform = new TranslateTransform3D(i * 0.5, 0, 0)
                                };

                this.view1.Viewport.Children.Add(model);
            }
        }
        
        private void InitSelector()
        {
            foreach (var name in Enum.GetNames(typeof(SelectionHitMode)))
            {
                this.modeSelector.Items.Add(name);
            }
        }

        /// <summary>
        /// The on models selected.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void OnModelsSelected(object sender, SelectionRoutedEventArgs args)
        {
            this.ResetModels();
            foreach (var model in args.SelectedModels)
            {
                var geoModel = model as GeometryModel3D;
                if (geoModel != null)
                {
                    geoModel.Material = Materials.Red;
                    geoModel.BackMaterial = Materials.Red;
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = this.modeSelector.SelectedItem as string;

            SelectionHitMode mode;
            if (item != null && Enum.TryParse(item, out mode))
            {
                this.SelectCommand.SelectionHitMode = mode;
            }
        }

        private void ResetModels()
        {
            this.view1.Viewport.Children.Traverse<GeometryModel3D>((x, transform) =>
                {
                    x.Material = Materials.Blue;
                    x.BackMaterial = Materials.Blue;
                });
        }
    }
}
