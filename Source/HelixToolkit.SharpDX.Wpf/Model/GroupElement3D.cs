namespace HelixToolkit.SharpDX.Wpf
{
    public class GroupElement3D : Element3D
    {
        public Element3DCollection Children { get; private set; }

        public GroupElement3D()
        {
            this.Children = new Element3DCollection();
        }

        protected override void Attach(IRenderHost host)
        {
            base.Attach(host);
            foreach (IRenderable c in this.Children) c.Attach(host);
        }
        
        protected override void Detach()
        {
            base.Detach();
            foreach (IRenderable c in this.Children) c.Detach();
        }
        
        protected override void Render()
        {
            base.Render();
            foreach (IRenderable c in this.Children) c.Render();
        }
    }
}