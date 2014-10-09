// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RectSelection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    [Example("Rectangle Selection", "Demonstrates rectangle selection.")]
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
            var vm = new MainWindowViewModel(this.view1.Viewport);
            this.DataContext = vm;
            this.view1.InputBindings.Add(new MouseBinding(vm.RectangleSelectionCommand, new MouseGesture(MouseAction.LeftClick)));
            this.view1.InputBindings.Add(new MouseBinding(vm.PointSelectionCommand, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control)));
        }
    }

    public class MainWindowViewModel
    {
        private IList<Model3D> selectedModels;

        public MainWindowViewModel(Viewport3D viewport)
        {
            this.RectangleSelectionCommand = new RectangleSelectionCommand(viewport, this.HandleSelectionEvent);
            this.PointSelectionCommand = new PointSelectionCommand(viewport, this.HandleSelectionEvent);
        }

        public RectangleSelectionCommand RectangleSelectionCommand { get; private set; }

        public PointSelectionCommand PointSelectionCommand { get; private set; }

        public SelectionHitMode SelectionMode
        {
            get
            {
                return this.RectangleSelectionCommand.SelectionHitMode;
            }

            set
            {
                this.RectangleSelectionCommand.SelectionHitMode = value;
            }
        }

        public IEnumerable<SelectionHitMode> SelectionModes
        {
            get
            {
                return Enum.GetValues(typeof(SelectionHitMode)).Cast<SelectionHitMode>();
            }
        }

        private void HandleSelectionEvent(object sender, ModelsSelectedEventArgs args)
        {
            this.ChangeMaterial(this.selectedModels, Materials.Blue);
            this.selectedModels = args.SelectedModels;
            var rectangleSelectionArgs = args as ModelsSelectedByRectangleEventArgs;
            if (rectangleSelectionArgs != null)
            {
                this.ChangeMaterial(
                    this.selectedModels,
                    rectangleSelectionArgs.Rectangle.Size != default(Size) ? Materials.Red : Materials.Green);
            }
            else
            {
                this.ChangeMaterial(this.selectedModels, Materials.Orange);
            }
        }

        private void ChangeMaterial(IEnumerable<Model3D> models, Material material)
        {
            if (models == null)
            {
                return;
            }

            foreach (var model in models)
            {
                var geometryModel = model as GeometryModel3D;
                if (geometryModel != null)
                {
                    geometryModel.Material = geometryModel.BackMaterial = material;
                }
            }
        }
    }
}