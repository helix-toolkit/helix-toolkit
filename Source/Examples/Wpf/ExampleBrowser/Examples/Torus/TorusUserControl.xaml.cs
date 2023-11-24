using DependencyPropertyGenerator;
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

namespace Torus;

/// <summary>
/// Interaction logic for TorusUserControl.xaml
/// </summary>
[DependencyProperty<double>("UCTorusDiameter", DefaultValue = 100.0)]
[DependencyProperty<double>("UCTubeDiameter", DefaultValue = 10.0)]
[DependencyProperty<int>("UCThetaDiv", DefaultValue = 36)]
[DependencyProperty<int>("UCPhiDiv", DefaultValue = 36)]
public partial class TorusUserControl : UserControl
{
    // reason for issue #1419:
    // In the DependencyProperty registration the DefaultValue (PropertyMetaData) was not set to a useful value.
    // So the default values 0.0 (double) or 0 (int) were used. But 0.0 and 0 are invalid values for the Torus properties.
    // As a result of this an exception was thrown.

    public TorusUserControl()
    {
        InitializeComponent();
    }
}
