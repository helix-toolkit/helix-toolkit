using HelixToolkit.Wpf.SharpDX;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OrderIndependentTransparentRendering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeometryModel3D selectedModel;
        public MainWindow()
        {
            InitializeComponent();
            view.AddHandler(Element3D.MouseDown3DEvent, new RoutedEventHandler((s, e) =>
            {
                var arg = e as MouseDown3DEventArgs;

                if (arg.HitTestResult == null)
                {
                    return;
                }
                //if (selectedModel != null)
                //{
                //    selectedModel.PostEffects = null;
                //    selectedModel = null;
                //}
                selectedModel = arg.HitTestResult.ModelHit as GeometryModel3D;
                if (selectedModel != null)
                {
                    selectedModel.PostEffects = string.IsNullOrEmpty(selectedModel.PostEffects) ? $"highlight[color:#FFFF00]" : null;
                }
            }));
        }
    }
}
