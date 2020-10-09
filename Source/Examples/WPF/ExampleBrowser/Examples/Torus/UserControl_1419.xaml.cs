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

namespace ExampleBrowser.Examples.Torus
{
    /// <summary>
    /// Interaction logic for UserControl_1419.xaml
    /// </summary>
    public partial class UserControl_1419 : UserControl
    {
        public UserControl_1419()
        {
            InitializeComponent();
        }

        public double UCTorusDiameter
        {
            get { return (double)GetValue(UCTorusDiameterProperty); }
            set { SetValue(UCTorusDiameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UCTorusDiameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UCTorusDiameterProperty =
            DependencyProperty.Register("UCTorusDiameter", typeof(double), typeof(UserControl_1419), new PropertyMetadata(100.0));



        public double UCTubeDiameter
        {
            get { return (double)GetValue(UCTubeDiameterProperty); }
            set { SetValue(UCTubeDiameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UCTubeDiameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UCTubeDiameterProperty =
            DependencyProperty.Register("UCTubeDiameter", typeof(double), typeof(UserControl_1419), new PropertyMetadata(10.0));



        public int UCThetaDiv
        {
            get { return (int)GetValue(UCThetaDivProperty); }
            set { SetValue(UCThetaDivProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UCThetaDiv.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UCThetaDivProperty =
            DependencyProperty.Register("UCThetaDiv", typeof(int), typeof(UserControl_1419), new PropertyMetadata(36));


        public int UCPhiDiv
        {
            get { return (int)GetValue(UCPhiDivProperty); }
            set { SetValue(UCPhiDivProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UCPhiDev.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UCPhiDivProperty =
            DependencyProperty.Register("UCPhiDiv", typeof(int), typeof(UserControl_1419), new PropertyMetadata(36));



    }
}
