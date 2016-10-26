// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window1.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for Window1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ExampleBrowser;

namespace ModelViewer
{
    /// <summary>
    /// Interaction logic for Window1.
    /// </summary>
    [Example(null,"Model viewer.")]
    public partial class Window1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Window1"/> class.
        /// </summary>
        public Window1()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel(new FileDialogService(), view1);
        }
    }
}