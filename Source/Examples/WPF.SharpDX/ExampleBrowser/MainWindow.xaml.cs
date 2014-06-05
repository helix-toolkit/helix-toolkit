// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
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