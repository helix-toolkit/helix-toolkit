using HelixToolkit.SharpDX.Model.Scene;
#if WINUI
using Microsoft.UI.Xaml.Markup;
#else
using System.Windows;
using System.Windows.Markup;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

#if WINUI
[ContentProperty(Name = "Content")]
#else
[ContentProperty("Content")]
#endif
public class Element3DPresenter : Element3D
{
    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    /// <value>
    /// The content.
    /// </value>
    public Element3D? Content
    {
        get
        {
            return (Element3D?)GetValue(ContentProperty);
        }
        set
        {
            SetValue(ContentProperty, value);
        }
    }

    /// <summary>
    /// The content property
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(Element3D), typeof(Element3DPresenter), new PropertyMetadata(null, (d, e) =>
        {
            var model = d as Element3DPresenter;
            if (e.OldValue != null)
            {
                model?.RemoveLogicalChild(e.OldValue);
                if (e.OldValue is Element3D ele)
                {
                    (model?.SceneNode as GroupNode)?.RemoveChildNode(ele.SceneNode);
                }
            }
            if (e.NewValue != null)
            {
                model?.AddLogicalChild(e.NewValue);
                if (e.NewValue is Element3D ele)
                {
                    (model?.SceneNode as GroupNode)?.AddChildNode(ele.SceneNode);
                }
            }
        }));

    public Element3DPresenter()
    {
        Loaded += Element3DPresenter_Loaded;
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new GroupNode();
    }

    private void Element3DPresenter_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Content != null)
        {
#if WPF
            RemoveLogicalChild(Content);
            AddLogicalChild(Content);
#endif
        }
    }
}
