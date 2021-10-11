#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Controls;
namespace HelixToolkit.UWP
#endif
{
#if !WINUI
    using Model.Scene;
#endif
    
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Element3D" />
    [ContentProperty(Name = "Content")]
    public class Element3DPresenter : Element3D
    {
        public Element3D Content
        {
            get
            {
                return (Element3D)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(Element3D), typeof(Element3DPresenter), new PropertyMetadata(null, (d, e) =>
            {
                (d as Element3DPresenter).Element3DPresenter_ContentChanged(d, e.NewValue as Element3D);
            }));


        private Element3D currentContent;

        public Element3DPresenter()
        {
        }

        private void Element3DPresenter_ContentChanged(object sender, Element3D content)
        {
            if (currentContent != null)
            {
                (SceneNode as GroupNode).RemoveChildNode(currentContent.SceneNode);
                AttachChild(null);
            }
            if (Content is Element3D elem)
            {
                currentContent = elem;
                (SceneNode as GroupNode).AddChildNode(elem.SceneNode);
                AttachChild(elem);
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode();
        }
    }
}