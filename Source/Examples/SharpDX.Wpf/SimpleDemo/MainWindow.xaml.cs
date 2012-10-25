using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();

            /// set up the trackball
            //var trackball = new Wpf3DTools.Trackball();
            //TrackBall.EventSource = view1;            
            //m_viewport.Camera.Transform = trackball.Transform;
            //m_light.Transform = trackball.RotateTransform;
        }

        //public static readonly Wpf3DTools.Trackball TrackBall = new Wpf3DTools.Trackball();
    }
}
