using HelixToolkit.Wpf.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX.Render;
using SharpDX;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace HelixToolkit.Wpf.SharpDX
{
    [ContentProperty("Content")]
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
            DependencyProperty.Register("Content", typeof(Element3D), typeof(Element3DPresenter), new PropertyMetadata(null, (d,e)=> 
            {
                var model = d as Element3DPresenter;               
                if(e.OldValue != null)
                {
                    model.RemoveLogicalChild(e.OldValue);
                    (model.SceneNode as GroupNode).RemoveChildNode(e.OldValue as Element3D);
                }
                if(e.NewValue != null)
                {
                    model.AddLogicalChild(e.NewValue);
                    (model.SceneNode as GroupNode).AddChildNode(e.NewValue as Element3D);
                }
            }));


        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode();
        }
    }
}
