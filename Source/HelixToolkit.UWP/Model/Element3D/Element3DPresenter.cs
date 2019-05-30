using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace HelixToolkit.UWP
{
    using Model.Scene;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Element3D" />
    [ContentProperty(Name = "Content")]
    public class Element3DPresenter : Element3D
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public Element3D Content
        {
            get { return (Element3D)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
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
                    model.Items.Remove(e.OldValue);
                    if (e.OldValue is Element3D ele)
                    { (model.SceneNode as GroupNode).RemoveChildNode(ele.SceneNode); }
                }
                if (e.NewValue != null)
                {
                    model.Items.Add(e.NewValue);
                    if (e.NewValue is Element3D ele)
                    { (model.SceneNode as GroupNode).AddChildNode(ele.SceneNode); }
                }
            }));


        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode();
        }
    }
}