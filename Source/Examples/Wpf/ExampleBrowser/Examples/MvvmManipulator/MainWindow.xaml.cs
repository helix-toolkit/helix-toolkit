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

namespace MvvmManipulator;

/// <summary>
/// Interaction logic for Window1.
/// </summary>
[ExampleBrowser.Example("MvvmManipulator", "Translate/Rotate manipulations.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }
}
