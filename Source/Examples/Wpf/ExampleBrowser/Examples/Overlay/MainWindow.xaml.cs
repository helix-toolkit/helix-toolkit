using HelixToolkit.Wpf;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Overlay;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Overlay", "Overlays 2D text and geometry on the 3D model.")]
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();
        CompositionTarget.Rendering += this.CompositionTargetRendering;

        const int N = 9;
        for (int i = -N; i <= N; i++)
        {
            for (int j = -N; j <= N; j++)
            {
                var circle = new Ellipse { Width = 4, Height = 4, Fill = Brushes.Tomato };
                var text = new TextBlock { Text = "(" + i + "," + j + ")" };
                Overlay.SetPosition3D(circle, new Point3D(i, j, 0));
                Overlay.SetPosition3D(text, new Point3D(i, j, 0));
                this.overlay1.Children.Add(circle);
                this.overlay1.Children.Add(text);
            }
        }

        var text1 = new TextBlock
        {
            Text = "Hello world!",
            FontWeight = FontWeights.Bold,
            FontSize = 16,
            Foreground = Brushes.YellowGreen,
            Background = Brushes.Gray,
            Padding = new Thickness(4)
        };

        Overlay.SetPosition3D(text1, new Point3D(0, 0, 10));
        this.overlay1.Children.Add(text1);
    }

    /// <summary>
    /// The composition target rendering.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void CompositionTargetRendering(object? sender, EventArgs e)
    {
        var matrix = Viewport3DHelper.GetTotalTransform(this.view1.Viewport);
        foreach (FrameworkElement element in this.overlay1.Children)
        {
            var position = Overlay.GetPosition3D(element);
            var position2D = matrix.Transform(position);
            Canvas.SetLeft(element, position2D.X - element.ActualWidth / 2);
            Canvas.SetTop(element, position2D.Y - element.ActualHeight / 2);
        }
    }
}
