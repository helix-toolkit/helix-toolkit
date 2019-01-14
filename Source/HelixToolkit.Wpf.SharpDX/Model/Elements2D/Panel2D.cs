using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Model.Scene2D;

    [ContentProperty("Children")]
    public class Panel2D : Element2D
    {
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(Panel2D), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public ObservableCollection<Element2D> Children
        {
            get;
        } = new ObservableCollection<Element2D>();

        public Panel2D()
        {
            Children.CollectionChanged += Items_CollectionChanged;
            EnableBitmapCache = false;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                DetachChildren(e.OldItems);
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach(var item in SceneNode.ItemsInternal)
                {
                    this.RemoveLogicalChild(item.WrapperSource);
                }
                (SceneNode as PanelNode2D).Clear();
                if (IsAttached)
                {
                    AttachChildren(sender as IEnumerable);
                }
            }
            else if (e.NewItems != null && IsAttached)
            {
                AttachChildren(e.NewItems);
            }
        }

        protected void AttachChildren(IEnumerable children)
        {
            var s = SceneNode as PanelNode2D;
            foreach (Element2D c in children)
            {
                if (c.Parent == null)
                {
                    this.AddLogicalChild(c);
                }
                s.AddChildNode(c);
            }
        }

        protected void DetachChildren(IEnumerable children)
        {
            var s = SceneNode as PanelNode2D;
            foreach (Element2D c in children)
            {
                if (c.Parent == this)
                {
                    this.RemoveLogicalChild(c);
                }
                s.RemoveChildNode(c);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AttachChildren(Children);
        }

        protected override void OnDetached()
        {
            DetachChildren(Children);
            base.OnDetached();
        }

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new PanelNode2D();
        }
    }
}
