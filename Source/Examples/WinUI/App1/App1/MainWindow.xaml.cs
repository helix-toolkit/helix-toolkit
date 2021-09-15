using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
        }

        public MainViewModel ViewModel { get; set; }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            // myButton.Content = "Clicked";
        }

        private void viewport_OnMouse3DDown(object sender, HelixToolkit.WinUI.MouseDown3DEventArgs e)
        {
            int k = 0;
        }

        private void Viewport3DX_OnMouse3DDown(object sender, HelixToolkit.WinUI.MouseDown3DEventArgs e)
        {

        }
    }
}
