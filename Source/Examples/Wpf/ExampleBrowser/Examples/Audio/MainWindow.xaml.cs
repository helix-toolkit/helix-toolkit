using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Audio;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Audio", "Showing a spectrogram from NAudio using Transform3Ds.")]
public partial class MainWindow : Window
{
    // NAudio still not supporting 64bit
    // http://naudio.codeplex.com/Thread/View.aspx?ThreadId=46000

    private readonly ControlPanelViewModel viewModel;

    public MainWindow()
    {
        InitializeComponent();
        Closing += MainWindow_Closing;
        viewModel = new ControlPanelViewModel(null, analyzer);
        DataContext = viewModel;
    }

    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        viewModel.Dispose();
    }
}
