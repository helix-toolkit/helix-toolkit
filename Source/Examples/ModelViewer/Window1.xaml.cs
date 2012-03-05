// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window1.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using System.IO;
using System.Windows.Media.Media3D;

namespace StudioDemo
{
    using System.Diagnostics;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            var vm = new MainViewModel(new FileDialogService(), view1);
            DataContext = vm;
          //  FindModels(@"Models");
          //  KeyDown += new KeyEventHandler(Window1_KeyDown);
        }

/*        private Visual3D selectedObject;

        void Window1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (selectedObject != null)
                {
                    Delete(selectedObject);
                    selectedObject = null;
                }

            }
            if (e.Key == Key.I)
            {
                if (selectedObject != null)
                    selectedObject.Transform = Transform3D.Identity;
            }
        }

        private void Delete(Visual3D selectedObject)
        {
            var dp = VisualTreeHelper.GetParent(selectedObject);
            var m = dp as ModelVisual3D;
            if (m != null)
            {
                m.Children.Remove(selectedObject);
            }
        }

        private void FindModels(string p)
        {
            var files = System.IO.Directory.GetFiles(p, "*.obj");
            foreach (var file in files)
                list1.Items.Add(new ListBoxItem()
                {
                    Content = System.IO.Path.GetFileNameWithoutExtension(file),
                    Tag = System.IO.Path.GetFullPath(file)
                });
            foreach (var dir in Directory.GetDirectories(p))
                FindModels(dir);
        }

        private void list1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var model = view1.Children[2] as FileModelVisual3D;
            if (model != null)
                model.Source = (list1.SelectedItem as ListBoxItem).Tag as string;

            view1.CameraController.ResetCamera();
            if (model != null)
            {
                //if (model.Content != null)
                //    view1.LookAt(model.Content.Bounds.Location, 100, 500);

                var bounds = Visual3DHelper.FindBounds(model.Children);
                view1.LookAt(bounds.Location);

            }
            view1.ZoomExtents(400);
        }

        private void view1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedObject = view1.FindNearestVisual(e.GetPosition(view1));
            if (selectedObject != null)
            {
                var visual = selectedObject as ModelVisual3D;

                Info.Text = visual.Transform.Value.ToString("N4", 8);
            }
            else
            {
                Info.Text = "";
            }
        }*/

        private void ModelLoaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomExtents();
        }
    }
}