// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using global::SharpDX;

    public class GroupModel3D : GroupElement3D, IHitable, IVisible
    {
        protected override void OnRender(IRenderContext renderContext)
        {
            foreach (var c in this.Items)
            {
                c.Render(renderContext);
            }
        }      

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            bool hit = false;
            foreach (var c in this.Items)
            {
                if (c is IHitable)
                {
                    if (((IHitable)c).HitTest(context, ray, ref hits))
                    {
                        hit = true;
                    }
                }
            }
            if (hit)
            {
                var pos = ray.Position;
                hits = hits.OrderBy(x => Vector3.DistanceSquared(pos, x.PointHit)).ToList();
            }            
            return hit;
        }
    }
}