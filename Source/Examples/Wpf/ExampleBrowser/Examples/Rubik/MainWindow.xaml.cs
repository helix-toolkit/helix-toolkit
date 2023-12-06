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

namespace Rubik;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Rubik", "Rubik's cube visualized.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.Loaded += this.MainWindowLoaded;
    }

    private void MainWindowLoaded(object? sender, RoutedEventArgs e)
    {
        view1.ZoomExtents();
    }

    private void HandleKeyDown(object? sender, KeyEventArgs e)
    {
        this.info.Text = string.Empty;
        if ((e.Key == Key.Back || e.Key == Key.Escape) && cube1.CanUnscramble())
        {
            cube1.Unscramble();
            e.Handled = true;
        }

        if (e.Key == Key.Space)
        {
            cube1.Scramble();
            e.Handled = true;
        }

        if (e.Key == Key.Add)
        {
            cube1.Size++;
            e.Handled = true;
        }

        if (e.Key == Key.Subtract && cube1.Size > 2)
        {
            cube1.Size--;
            e.Handled = true;
        }

        if (e.Key == Key.H)
        {
            StringBuilder sb = new();
            sb.AppendLine("Keys:");
            sb.AppendLine("h/H = show info text");
            sb.AppendLine("l/L = rotate Left");
            sb.AppendLine("r/R = rotate Right");
            sb.AppendLine("u/U = rotate Up");
            sb.AppendLine("d/D = rotate Down");
            sb.AppendLine("b/B = rotate Back");
            sb.AppendLine("f/F = rotate Front");
            sb.AppendLine("Backspace/Escape = undo");
            sb.AppendLine("Space = do random move");
            sb.AppendLine("Numpad +/- = change size");

            info.Text = sb.ToString();

            e.Handled = true;
            return;
        }

        cube1.Rotate(e.Key);
    }
}
