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

namespace GroupElementDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private GroupModel3D? tempGroup;

    public MainWindow()
    {
        InitializeComponent();

        Closed += (s, e) => (DataContext as IDisposable)?.Dispose();
    }

    private void AttachGroupButton_Click(object? sender, RoutedEventArgs e)
    {
        if (tempGroup is not null)
        {
            view1.Items.Add(tempGroup);
            tempGroup = null;
        }
    }

    private void DetachGroupButton_Click(object? sender, RoutedEventArgs e)
    {
        if (tempGroup is null)
        {
            tempGroup = group1;
            view1.Items.Remove(group1);
        }
    }
}
