using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        var allExamples = this.GetExamples(this.GetType().Assembly).OrderBy(e => e.Title).ToArray();
        this.Examples = new ObservableCollection<Example>(allExamples);
    }

    /// <summary>
    /// Gets the examples.
    /// </summary>
    /// <value>The examples.</value>
    public ObservableCollection<Example> Examples { get; }

    /// <summary>
    /// Creates a thumbnail of the specified window.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <param name="width">The width of the thumbnail.</param>
    /// <param name="path">The output path.</param>
    private static void CreateThumbnail(Window window, int width, string path)
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

        window.KeyDown += (s, args) =>
        {
            if (args.Key == Key.F12)
            {
                CreateThumbnail(window, 120, Path.Combine(@"..\..\..\Images\", example.ThumbnailFileName));
                MessageBox.Show(window, "Demo image updated. Now add `" + example.ThumbnailFileName + "` as a resource in the Images folder in the ExampleBrowser project.");
                e.Handled = true;
            }
        };
    }

    /// <summary>
    /// Gets the examples in the specified assembly.
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="textSearch"></param>
    /// <returns></returns>
    private IEnumerable<Example> GetExamples(Assembly assembly, string? textSearch= null)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttributes(typeof(ExampleAttribute), false).FirstOrDefault() is ExampleAttribute ea)
            {
                if (!string.IsNullOrEmpty(textSearch))
                {
                    string lowerTextSearch = textSearch!.ToLower();
                    if (!string.IsNullOrEmpty(ea.Title) && ea.Title.ToLower().Contains(lowerTextSearch)
                        || !string.IsNullOrEmpty(ea.Description) && ea.Description.ToLower().Contains(lowerTextSearch))
                        yield return new Example(type, ea.Title, ea.Description);
                }
                else
                {
                    yield return new Example(type, ea.Title, ea.Description);
                }

            }
        }
    }
    private void ButtonSearch_Click(object sender, RoutedEventArgs e)
    {
        this.Examples.Clear();
        var examples = this.GetExamples(this.GetType().Assembly, searchTxt.Text).OrderBy(e => e.Title).ToArray();
        foreach (var example in examples)
        {
            this.Examples.Add(example);
        }
    }
}
