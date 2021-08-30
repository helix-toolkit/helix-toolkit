/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
using System;
using System.Collections.Generic;
#if !NETFX_CORE && !WINUI_NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Utilities;
#if !NETFX_CORE && !WINUI_NET5_0
    [Serializable]
#endif
    public class PointGeometry3D : Geometry3D
    {
        public IEnumerable<Point> Points
        {
            get
            {
                for (int i = 0; i < Positions.Count; ++i)
                {
                    yield return new Point { P0 = Positions[i] };
                }
            }
        }

        protected override IOctreeBasic CreateOctree(OctreeBuildParameter parameter)
        {

            return new StaticPointGeometryOctree(Positions, parameter);
        }

        protected override bool CanCreateOctree()
        {
            return Positions != null && Positions.Count > 0;
        }

        public virtual bool HitTest(HitTestContext context, Matrix modelMatrix, ref List<HitTestResult> hits, object originalSource, float hitThickness)
        {
            if(Positions==null || Positions.Count == 0)
            { return false; }
            if (Octree != null)
            {
                return Octree.HitTest(context, originalSource, this, modelMatrix, ref hits, hitThickness);
            }
            else
            {
                var svpm = context.RenderMatrices.ScreenViewProjectionMatrix;
                var smvpm = modelMatrix * svpm;

                var clickPoint = context.HitPointSP.ToVector3() * context.RenderMatrices.DpiScale;

                var result = new HitTestResult { IsValid = false, Distance = double.MaxValue };
                var maxDist = hitThickness;
                var lastDist = double.MaxValue;
                var index = 0;

                foreach (var point in Positions)
                {
                    var p0 = Vector3.TransformCoordinate(point, smvpm);
                    var pv = p0 - clickPoint;
                    var dist = pv.Length() / context.RenderMatrices.DpiScale;
                    if (dist < lastDist && dist <= maxDist)
                    {
                        lastDist = dist;
                        var lp0 = point;
                        Vector3.TransformCoordinate(ref lp0, ref modelMatrix, out var pvv);
                        result.Distance = (context.RayWS.Position - pvv).Length();
                        result.PointHit = pvv;
                        result.ModelHit = originalSource;
                        result.IsValid = true;
                        result.Tag = index;
                        result.Geometry = this;
                    }

                    index++;
                }

                if (result.IsValid)
                {
                    hits.Add(result);
                }

                return result.IsValid;
            }
        }

        public override void UpdateBounds()
        {
            base.UpdateBounds();
            if(Bound.Size.LengthSquared() < 1e-1f)
            {
                var off = new Vector3(1f);
                Bound = new BoundingBox(Bound.Minimum - off, Bound.Maximum + off);
            }
            if(BoundingSphere.Radius < 1e-1f)
            {
                BoundingSphere = new BoundingSphere(BoundingSphere.Center, 1f);
            }
        }
    }
}
