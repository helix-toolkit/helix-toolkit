// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PenroseTriangleDemo
{
    using System.Windows.Input;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Shows a Penrose triangle in 3D.")]
    public partial class MainWindow
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
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                var d = new SaveFileDialog { Title = "Export model", Filter = Exporters.Filter, DefaultExt = Exporters.DefaultExtension };
                if (d.ShowDialog().Value)
                {
                    view.Export(d.FileName);
                }
            }
        }
    }
}