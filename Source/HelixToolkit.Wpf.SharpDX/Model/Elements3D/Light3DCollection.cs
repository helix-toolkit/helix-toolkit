// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Light3DCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using HelixToolkit.Wpf.SharpDX.Model.Lights3D;

namespace HelixToolkit.Wpf.SharpDX
{
    public class Light3DCollection : GroupElement3D, ILight3D
    {
        public Light3DSceneShared Light3DSceneShared
        {
            private set; get;
        }

        public override void Attach(IRenderHost host)
        {
            Light3DSceneShared = host.Light3DSceneShared;
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
            foreach (var c in this.Children)
            {
                c.Render(context);
            }
        }
    }
}