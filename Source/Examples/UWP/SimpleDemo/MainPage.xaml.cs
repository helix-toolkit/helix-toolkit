using HelixToolkit.UWP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleDemoW10
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GeometryModel3D selectedElement = null;      

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Viewport3DX_OnMouse3DDown(object sender, HelixToolkit.UWP.MouseDown3DEventArgs e)
        {
            if (e.HitTestResult != null && e.HitTestResult.ModelHit is GeometryModel3D element)
            {
                if(selectedElement == element)
                {
                    selectedElement.PostEffects = null;
                    selectedElement = null;
                    return;
                }
                if(selectedElement != null)
                {
                    selectedElement.PostEffects = null;
                }
                selectedElement = element;
                if (selectedElement.Name != "floor")
                {
                    selectedElement.PostEffects = string.IsNullOrEmpty(selectedElement.PostEffects) ? "border[color:#00FFDE]" : null;
                }
            }
        }
    }
}
