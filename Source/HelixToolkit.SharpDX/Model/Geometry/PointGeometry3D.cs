using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX;

[Serializable]
public class PointGeometry3D : Geometry3D
{
    public IEnumerable<Point> Points
    {
        get
        {
            if (Positions is not null)
            {
                for (var i = 0; i < Positions.Count; ++i)
                {
                    yield return new Point { P0 = Positions[i] };
                }
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

    public virtual bool HitTest(HitTestContext? context, Matrix modelMatrix, ref List<HitTestResult> hits, object? originalSource, float hitThickness)
    {
        if (context is null)
        {
            return false;
        }

        if (Positions == null || Positions.Count == 0)
        {
            return false;
        }
        if (Octree != null)
        {
            return Octree.HitTest(context, originalSource, this, modelMatrix, ref hits, hitThickness);
        }
        else
        {
            var svpm = context.RenderMatrices?.ScreenViewProjectionMatrix ?? Matrix.Identity;
            var smvpm = modelMatrix * svpm;

            var clickPoint = context.HitPointSP.ToVector3() * (context.RenderMatrices?.DpiScale ?? 1.0f);

            var result = new HitTestResult { IsValid = false, Distance = double.MaxValue };
            var maxDist = hitThickness;
            var lastDist = double.MaxValue;
            var index = 0;

            foreach (var point in Positions)
            {
                var p0 = Vector3Helper.TransformCoordinate(point, smvpm);
                var pv = p0 - clickPoint;
                var dist = pv.Length() / context.RenderMatrices?.DpiScale ?? 1.0f;
                if (dist < lastDist && dist <= maxDist)
                {
                    lastDist = dist;
                    var lp0 = point;
                    Vector3Helper.TransformCoordinate(ref lp0, ref modelMatrix, out var pvv);
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
        if (Bound.Size.LengthSquared() < 1e-1f)
        {
            var off = new Vector3(1f);
            Bound = new BoundingBox(Bound.Minimum - off, Bound.Maximum + off);
        }
        if (BoundingSphere.Radius < 1e-1f)
        {
            BoundingSphere = new BoundingSphere(BoundingSphere.Center, 1f);
        }
    }
}
