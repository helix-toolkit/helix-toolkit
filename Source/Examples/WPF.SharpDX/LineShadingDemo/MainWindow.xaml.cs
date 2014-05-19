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
using HelixToolkit.Wpf.SharpDX;

namespace LineShadingDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainViewModel();
            this.DataContext = viewModel;

            // mouse events            
            this.view1.MouseDown += (o, e) =>
            {                
                var hits = this.view1.FindHits(e.GetPosition(this.view1));
                if (hits.Count > 0)
                {
                    foreach (var hit in hits.Where(h => h.IsValid))
                    {
                        hit.ModelHit.RaiseEvent(new MouseDown3DEventArgs(hit.ModelHit, hit, e.GetPosition(this.view1) ));
                        if (e.Handled)
                        {
                            break;
                        }
                    }
                }
            };
        }
    }
}
