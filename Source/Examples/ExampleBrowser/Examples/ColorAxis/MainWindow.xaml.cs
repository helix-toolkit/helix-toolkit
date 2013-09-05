using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColorAxisDemo
{
    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Brush ColorScheme { get; set; }
        public Brush ColorScheme2 { get; set; }

        public IList<string> Categories { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.ColorScheme = BrushHelper.CreateRainbowBrush(false);
            this.Categories = new[] { "Asia", "Africa", "North America", "South America", "Antarctica", "Europe", "Australia" };
            this.ColorScheme2 = BrushHelper.CreateSteppedGradientBrush(new[] { Colors.Yellow, Colors.Brown, Colors.Red, Colors.Green, Colors.White, Colors.Orange, Colors.Blue },false);
        }
    }
}
