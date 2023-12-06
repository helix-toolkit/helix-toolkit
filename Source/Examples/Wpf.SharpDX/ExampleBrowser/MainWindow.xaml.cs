using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ExampleBrowser;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();

        this.DataContext = this;
        this.Examples = this.GetExamples(this.GetType().Assembly).OrderBy(e => e.Title).ToArray();
    }

    /// <summary>
    /// Gets the examples.
    /// </summary>
    /// <value>The examples.</value>
    public IList<Example> Examples { get; }

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
            Point point = window.PointToScreen(new Point());
            using var bitmap = ScreenCapture.Capture(
                (int)(point.X + 0.5),
                (int)(point.Y + 0.5),
                (int)(window.ActualWidth + 0.5),
                (int)(window.ActualHeight + 0.5));
            var newHeight = width * bitmap.Height / bitmap.Width;
            using var resizedBitmap = BitmapTools.Resize(bitmap, width, newHeight);
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
    private void ListBoxMouseDoubleClick(object? sender, MouseButtonEventArgs e)
    {
        var lb = sender as ListBox;

        if (lb?.SelectedItem is not Example example)
        {
            return;
        }

        var window = example.Create();

        if (window is null)
        {
            return;
        }

        window.Show();

        if (example.Thumbnail is null)
        {
            CreateThumbnail(window, 120, Path.Combine(@"..\..\..\Images\", example.ThumbnailFileName), 1000);
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
            if (type.GetCustomAttributes(typeof(ExampleAttribute), false).FirstOrDefault() is ExampleAttribute ea)
            {
                yield return new Example(type, ea.Title, ea.Description);
            }
        }
    }
}
