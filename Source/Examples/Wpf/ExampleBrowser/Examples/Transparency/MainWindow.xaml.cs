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

namespace Transparency;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Transparency", "Uses 'depth sorting' to show a transparent model. The sorting frequency is reduced to show what is going on.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
