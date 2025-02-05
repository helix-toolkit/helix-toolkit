using HelixToolkit.SharpDX.Model.Scene2D;
using System.ComponentModel;
#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using Microsoft.UI.Xaml.Markup;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Core2D;
using System.Windows.Markup;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
[ContentProperty(Name = "Content")]
#elif WPF
[ContentProperty("Content")]
#else
#error Unknown framework
#endif
public class ContentPresenter2D : Element2D
{
    public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content", typeof(Element2DCore),
        typeof(ContentPresenter2D),
        new PropertyMetadata(null,
        (d, e) =>
        {
            if (d is not ContentPresenter2D model)
            {
                return;
            }

            if (model.SceneNode is not PresenterNode2D node)
            {
                return;
            }

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
    public Element2D? Content2D
    {
        set
        {
            SetValue(Content2DProperty, value);
        }
        get
        {
            return (Element2D?)GetValue(Content2DProperty);
        }
    }

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new PresenterNode2D();
    }
}
