// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AudioDemo
{
    using System.ComponentModel;
    using System.Windows;

    using ExampleBrowser;
    using NAudioWpfDemo;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Showing a spectrogram from NAudio using Transform3Ds.")]
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