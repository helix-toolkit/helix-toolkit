using HelixToolkit.SharpDX.Model.Scene;
using Microsoft.UI.Xaml.Markup;

namespace HelixToolkit.WinUI.SharpDX;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Element3D" />
[ContentProperty(Name = "Content")]
public class Element3DPresenter : Element3D
{
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

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(Element3D), typeof(Element3DPresenter),
            new PropertyMetadata(null, (d, e) =>
            {
                (d as Element3DPresenter)?.Element3DPresenter_ContentChanged(d, e.NewValue as Element3D);
            }));


    private Element3D? currentContent;

    public Element3DPresenter()
    {
    }

    private void Element3DPresenter_ContentChanged(object? sender, Element3D? content)
    {
        if (currentContent is not null)
        {
            (SceneNode as GroupNode)?.RemoveChildNode(currentContent.SceneNode);
            AttachChild(null);
        }
        if (Content is Element3D elem)
        {
            currentContent = elem;
            (SceneNode as GroupNode)?.AddChildNode(elem.SceneNode);
            AttachChild(elem);
        }
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new GroupNode();
    }
}
