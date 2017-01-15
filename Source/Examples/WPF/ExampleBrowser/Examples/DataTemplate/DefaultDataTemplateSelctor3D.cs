namespace DataTemplateDemo
{
    using System.Windows;

    public class DefaultDataTemplateSelctor3D : DataTemplateSelector3D
    {
        public override DataTemplate3D SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (element != null && item != null)
            {
                // go through all types and base types to find a matching DataTemplate3D
                // this mirrors the behavior of DataTemplate
                foreach (var type in item.GetType().GetTypeAndBaseTypes())
                {
                    var key = type;
                    var template = element.TryFindResource(key) as DataTemplate3D;
                    if (template != null)
                        return template;
                }
            }

            return null;
        }
    }
}
