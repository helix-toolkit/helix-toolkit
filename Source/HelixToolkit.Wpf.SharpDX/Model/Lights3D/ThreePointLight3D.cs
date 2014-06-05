namespace HelixToolkit.Wpf.SharpDX.Model.Lights3D
{
    public class ThreePointLight3D : GroupElement3D, ILight3D
    {
        public ThreePointLight3D()
        {
            //TODO: http://www.3drender.com/light/3point.html
        }

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
