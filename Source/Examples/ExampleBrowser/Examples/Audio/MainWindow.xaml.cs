// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows;
using NAudioWpfDemo;

namespace AudioDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}