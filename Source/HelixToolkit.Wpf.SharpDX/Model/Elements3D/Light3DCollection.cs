namespace HelixToolkit.Wpf.SharpDX
{
    public class Light3DCollection : GroupElement3D, ILight3D
    {
        public override void Attach(IRenderHost host)
        {             
            foreach (var c in this.Children)
            {
                c.Attach(host);
            }
        }

        public override void Detach()
        {            
            foreach (var c in this.Children)
            {
                c.Detach();
            }
        }

        public override void Render(RenderContext context)
        {            
            foreach (var c in this.Children)
            {
                c.Render(context);
            }
        }
    }
}
