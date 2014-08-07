namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows;
    using System.Windows.Markup;

    [ContentProperty("Children")]
    public abstract class GroupElement3D : Element3D //, IElement3DCollection
    {

        public Element3DCollection Children
        {
            get { return (Element3DCollection)this.GetValue(ChildrenProperty); }
            set { this.SetValue(ChildrenProperty, value); }
        }

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(Element3DCollection), typeof(GroupElement3D), new UIPropertyMetadata(new Element3DCollection()));

        public GroupElement3D()
        {
            this.Children = new Element3DCollection();
        }

        public override void Attach(IRenderHost host)
        {
            base.Attach(host);
            foreach (var c in this.Children)
            {
                if (c.Parent == null)
                {
                    this.AddLogicalChild(c);                    
                }

                c.Attach(host);
            }
        }

        public override void Detach()
        {
            base.Detach();
            foreach (var c in this.Children)
            {
                c.Detach();
                if (c.Parent == this)
                {
                    this.RemoveLogicalChild(c);                    
                }
            }
        }

        public override void Render(RenderContext context)
        {
            base.Render(context);
            foreach (var c in this.Children)
            {
                c.Render(context);
            }
        }
    }
}
