using System.Windows;

namespace DataTemplate;

public class DefaultDataTemplateSelctor3D : DataTemplateSelector3D
{
    public override DataTemplate3D? SelectTemplate(object item, DependencyObject? container)
    {
        if (container is FrameworkElement element && item != null)
        {
            // go through all types and base types to find a matching DataTemplate3D
            // this mirrors the behavior of DataTemplate
            foreach (var type in item.GetType().GetTypeAndBaseTypes())
            {
                var key = type;
                if (element.TryFindResource(key) is DataTemplate3D template)
                    return template;
            }
        }

        return null;
    }
}
