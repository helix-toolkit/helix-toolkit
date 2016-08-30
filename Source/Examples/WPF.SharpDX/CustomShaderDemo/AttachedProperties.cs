using System.Windows;

using HelixToolkit.Wpf.SharpDX;

namespace CustomShaderDemo
{
    public static class AttachedProperties
    {
        public static readonly DependencyProperty ShowSelectedProperty = DependencyProperty.RegisterAttached(
            "ShowSelected",
            typeof(bool),
            typeof(GeometryModel3D),
            new PropertyMetadata(false, ShowSelectedPropertyChanged));

        public static void SetShowSelected(UIElement element, bool value)
        {
            element.SetValue(ShowSelectedProperty, value);
        }

        public static bool GetShowSelected(UIElement element)
        {
            return (bool)element.GetValue(ShowSelectedProperty);
        }

        private static void ShowSelectedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if(obj is GeometryModel3D && obj.GetType() != typeof(BillboardTextModel3D))
            {
                var geom = (GeometryModel3D)obj;
                
                if (geom.IsAttached)
                {
                    var host = geom.RenderHost;
                    geom.Detach();
                    geom.Attach(host);
                }
            }
        }
    }
}
