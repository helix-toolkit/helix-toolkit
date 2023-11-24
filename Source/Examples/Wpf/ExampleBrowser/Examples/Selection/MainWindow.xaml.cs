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
using System.Windows.Shapes;

namespace Selection;

[ExampleBrowser.Example("Selection", "Demonstrates various of selection.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        var vm = new MainWindowViewModel(this.view1.Viewport);
        this.DataContext = vm;
        this.view1.InputBindings.Add(new MouseBinding(vm.PointSelectionCommand, new MouseGesture(MouseAction.LeftClick)));
        this.view1.InputBindings.Add(new MouseBinding(vm.RectangleSelectionCommand, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control)));
        this.view1.InputBindings.Add(new MouseBinding(vm.CombinedSelectionCommand, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Shift)));
    }
}
