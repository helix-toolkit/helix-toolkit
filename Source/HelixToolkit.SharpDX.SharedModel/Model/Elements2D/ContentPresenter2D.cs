using HelixToolkit.SharpDX.Model.Scene2D;
using System.ComponentModel;
#if WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using Microsoft.UI.Xaml.Markup;
#else
using HelixToolkit.Wpf.SharpDX.Core2D;
using System.Windows.Markup;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

#if WINUI
[ContentProperty(Name = "Content")]
#else
[ContentProperty("Content")]
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
