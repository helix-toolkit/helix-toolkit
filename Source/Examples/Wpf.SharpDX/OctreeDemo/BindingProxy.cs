using DependencyPropertyGenerator;
using System.Windows;

namespace OctreeDemo;

[DependencyProperty<object>("Data")]
public partial class BindingProxy : Freezable
{
    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }
}
