using System.Windows;

namespace DataTemplate;

public class DataTemplateSelector3D
{
    public virtual DataTemplate3D? SelectTemplate(object item, DependencyObject? container)
    {
        return null;
    }
}
