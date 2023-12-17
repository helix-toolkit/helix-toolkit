using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX;

[Serializable]
public class LineGeometry3D : Geometry3D
{
    public IEnumerable<Line> Lines
    {
        get
        {
            if (Indices is not null && Positions is not null)
            {
                for (var i = 0; i < Indices.Count; i += 2)
                {
                    yield return new Line { P0 = Positions[Indices[i]], P1 = Positions[Indices[i + 1]], };
                }
            }
        }
    }

    protected override IOctreeBasic CreateOctree(OctreeBuildParameter parameter)
    {
        return new StaticLineGeometryOctree(Positions, Indices, parameter);
    }

    protected override bool CanCreateOctree()
    {
        return Positions != null && Positions.Count > 0 && Indices != null && Indices.Count > 0;
    }

    public virtual bool HitTest(HitTestContext? context, Matrix modelMatrix, ref List<HitTestResult> hits, object? originalSource, float hitTestThickness)
    {
        if (context is null)
        {
            return false;
        }

        if (Positions == null || Positions.Count == 0
            || Indices == null || Indices.Count == 0)
        {
            return false;
        }

        if (Octree != null)
        {
            return Octree.HitTest(context, originalSource, this, modelMatrix, ref hits, hitTestThickness);
        }
        else
        {
            var result = new LineHitTestResult { IsValid = false, Distance = double.MaxValue };
            var lastDist = double.MaxValue;
            var lineIndex = 0;
            foreach (var line in Lines)
            {
                var t0 = Vector3Helper.TransformCoordinate(line.P0, modelMatrix);
                var t1 = Vector3Helper.TransformCoordinate(line.P1, modelMatrix);
                var rayToLineDistance = LineBuilder.GetRayToLineDistance(context.RayWS, t0, t1, out var sp, out var tp, out var sc, out var tc);
                var svpm = context.RenderMatrices?.ScreenViewProjectionMatrix ?? Matrix.Identity;
                Vector3Helper.TransformCoordinate(ref sp, ref svpm, out var sp3);
                Vector3Helper.TransformCoordinate(ref tp, ref svpm, out var tp3);
                var tv2 = new Vector2(tp3.X - sp3.X, tp3.Y - sp3.Y);
                var dist = tv2.Length() / context.RenderMatrices?.DpiScale ?? 1.0f;
                if (dist < lastDist && dist <= hitTestThickness)
                {
                    lastDist = dist;
                    result.PointHit = sp;
                    result.NormalAtHit = sp - tp; // not normalized to get length
                    result.Distance = (context.RayWS.Position - sp).Length();
                    result.RayToLineDistance = rayToLineDistance;
                    result.ModelHit = originalSource;
                    result.IsValid = true;
                    result.Tag = lineIndex; // For compatibility
                    result.LineIndex = lineIndex;
                    result.TriangleIndices = null; // Since triangles are shader-generated
                    result.RayHitPointScalar = sc;
                    result.LineHitPointScalar = tc;
                    result.Geometry = this;
                }

                lineIndex++;
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
            var off = new Vector3(0.5f);
            Bound = new BoundingBox(Bound.Minimum - off, Bound.Maximum + off);
        }
        if (BoundingSphere.Radius < 1e-1f)
        {
            BoundingSphere = new BoundingSphere(BoundingSphere.Center, 0.5f);
        }
    }
}
