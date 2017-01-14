namespace DataTemplateDemo
{
    using System.Windows;

    public class DefaultDatTemplateSelctor3D : DataTemplateSelector3D
    {
        public override DataTemplate3D SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (element != null && item != null)
            {
                var key = item.GetType();
                return element.FindResource(key) as DataTemplate3D;
            }

            return null;
        }
    }
}
