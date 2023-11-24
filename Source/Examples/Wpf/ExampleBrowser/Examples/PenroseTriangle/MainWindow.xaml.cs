using HelixToolkit.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PenroseTriangle;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("PenroseTriangle", "Shows a Penrose triangle in 3D.")]
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the key down event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
    private void WindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.E)
        {
            var d = new SaveFileDialog
            {
                Title = "Export model",
                Filter = Exporters.Filter,
                DefaultExt = Exporters.DefaultExtension
            };

            if (d.ShowDialog() == true)
            {
                view.Export(d.FileName);
            }
        }
    }
}
