namespace HelixToolkit.SharpDX.Wpf
{
    public class GroupElement3D : Element3D
    {
        public Element3DCollection Children { get; private set; }

        public GroupElement3D()
        {
            this.Children = new Element3DCollection();
        }

        public override void Attach(IRenderHost host)
        {
            base.Attach(host);
            foreach (var c in this.Children)
            {
                c.Attach(host);
            }
        }

        public override void Detach()
        {
            base.Detach();
            foreach (var c in this.Children)
            {
                c.Detach();
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