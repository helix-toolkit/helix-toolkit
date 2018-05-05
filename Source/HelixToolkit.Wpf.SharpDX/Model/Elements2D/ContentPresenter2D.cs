using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Model.Scene2D;

    [ContentProperty("Content")]
    public class ContentPresenter2D : Element2D
    {
        public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content", typeof(Element2DCore), 
            typeof(ContentPresenter2D),
            new PropertyMetadata(null, 
            (d, e) =>
            {
                var model = d as ContentPresenter2D;
                var node = model.SceneNode as PresenterNode2D;
                if (e.OldValue is Element2D old)
                {
                    model.RemoveLogicalChild(old);
                    node.Content = null;
                }
                if (e.NewValue is Element2D newElement)
                {
                    model.AddLogicalChild(newElement);
                    node.Content = newElement;
                }
            }));

        [Bindable(true)]
        public Element2D Content2D
        {
            set
            {
                SetValue(Content2DProperty, value);
            }
            get
            {
                return (Element2D)GetValue(Content2DProperty);
            }
        }

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new PresenterNode2D();
        }
    }
}
