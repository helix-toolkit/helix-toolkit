// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ExampleBrowser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Examples = this.GetExamples(this.GetType().Assembly).ToArray();
            var polygon = new List<SharpDX.Vector2>(){
                new SharpDX.Vector2(5.2f, 1f),
                new SharpDX.Vector2(2.8f, 11.4f),
                new SharpDX.Vector2(6.7f, 5.6f),
                new SharpDX.Vector2(5.2f, 5.2f),
                new SharpDX.Vector2(13.2f, 1f),
                new SharpDX.Vector2(6.6f, 8.8f),
                new SharpDX.Vector2(11.2f, 9.8f),
                new SharpDX.Vector2(12.2f, 6.2f),
                new SharpDX.Vector2(9.6f, 8.2f),
                new SharpDX.Vector2(12.1f, 3.6f),
                new SharpDX.Vector2(14.2f, 5.7f),
                new SharpDX.Vector2(12.1f, 13.9f),
                new SharpDX.Vector2(6.7f, 10.3f),
                new SharpDX.Vector2(5.2f, 12.8f),
                new SharpDX.Vector2(9.2f, 13.9f),
                new SharpDX.Vector2(12.2f, 18.1f),
                new SharpDX.Vector2(10.6f, 21.7f),
                new SharpDX.Vector2(8.7f, 18.7f),
                new SharpDX.Vector2(6.2f, 20.7f),
                new SharpDX.Vector2(8.7f, 22.2f),
                new SharpDX.Vector2(2.7f, 21.6f),
                new SharpDX.Vector2(5.3f, 18f),
                new SharpDX.Vector2(7.7f, 17.6f),
                new SharpDX.Vector2(3.7f, 15.4f),
                new SharpDX.Vector2(3.2f, 19.6f),
                new SharpDX.Vector2(.8f, 8.8f),
            };

            var triangulationIndices = HelixToolkit.Wpf.SharpDX.SweepLinePolygonTriangulation.Triangulate(polygon);

        }

        /// <summary>
        /// Gets the examples.
        /// </summary>
        /// <value>The examples.</value>
        public IList<Example> Examples { get; private set; }

        /// <summary>
        /// Creates a thumbnail of the specified window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="width">The width of the thumbnail.</param>
        /// <param name="path">The output path.</param>
        /// <param name="delay">The delay before capturing the window (in milliseconds).</param>
        private static void CreateThumbnail(Window window, int width, string path, double delay)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(delay) };
            timer.Tick += (s, a) =>
            {
                var bitmap = ScreenCapture.Capture(
                    (int)window.Left,
                    (int)window.Top,
                    (int)window.ActualWidth,
                    (int)window.ActualHeight);
                var newHeight = width * bitmap.Height / bitmap.Width;
                var resizedBitmap = BitmapTools.Resize(bitmap, width, newHeight);
                resizedBitmap.Save(path);
                timer.Stop();
            };
            timer.Start();
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the ListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ListBoxMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var lb = (ListBox)sender;
            var example = lb.SelectedItem as Example;
            if (example != null)
            {
                var window = example.Create();
                window.Show();

                if (example.Thumbnail == null)
                {
                    CreateThumbnail(window, 120, Path.Combine(@"..\..\Images\", example.ThumbnailFileName), 1000);
                }
            }
        }

        /// <summary>
        /// Gets the examples in the specified assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private IEnumerable<Example> GetExamples(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var ea = type.GetCustomAttributes(typeof(ExampleAttribute), false).FirstOrDefault() as ExampleAttribute;
                if (ea != null)
                {
                    yield return new Example(type, ea.Title, ea.Description);
                }
            }
        }
    }
}