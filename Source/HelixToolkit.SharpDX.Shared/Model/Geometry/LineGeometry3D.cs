/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using global::SharpDX;
    using System;
    using System.Collections.Generic;
#if !NETFX_CORE
    [Serializable]
#endif
    public class LineGeometry3D : Geometry3D
    {                       
        public IEnumerable<Line> Lines
        {
            get
            {
                for (int i = 0; i < Indices.Count; i += 2)
                {
                    yield return new Line { P0 = Positions[Indices[i]], P1 = Positions[Indices[i + 1]], };
                }
            }
        }

        protected override IOctree CreateOctree(OctreeBuildParameter parameter)
        {
            return new LineGeometryOctree(Positions, Indices, parameter);
        }

        protected override bool CanCreateOctree()
        {
            return Positions != null && Positions.Count > 0 && Indices != null && Indices.Count > 0;
        }

        public virtual bool HitTest(IRenderContext context, Matrix modelMatrix, ref Ray rayWS, ref List<HitTestResult> hits, object originalSource, float hitTestThickness)
        {
            if (Positions == null || Positions.Count == 0
                || Indices == null || Indices.Count == 0)
            {
                return false;
            }

            if(Octree != null) 
            {
                return Octree.HitTest(context, originalSource, modelMatrix, rayWS, ref hits, hitTestThickness);
            }
            else
            {
                var result = new LineHitTestResult { IsValid = false, Distance = double.MaxValue };
                var lastDist = double.MaxValue;
                var lineIndex = 0;
                foreach (var line in Lines)
                {
                    var t0 = Vector3.TransformCoordinate(line.P0, modelMatrix);
                    var t1 = Vector3.TransformCoordinate(line.P1, modelMatrix);
                    Vector3 sp, tp;
                    float sc, tc;
                    var rayToLineDistance = LineBuilder.GetRayToLineDistance(rayWS, t0, t1, out sp, out tp, out sc, out tc);
                    var svpm = context.ScreenViewProjectionMatrix;
                    Vector4 sp4;
                    Vector4 tp4;
                    Vector3.Transform(ref sp, ref svpm, out sp4);
                    Vector3.Transform(ref tp, ref svpm, out tp4);
                    var sp3 = sp4.ToVector3();
                    var tp3 = tp4.ToVector3();
                    var tv2 = new Vector2(tp3.X - sp3.X, tp3.Y - sp3.Y);
                    var dist = tv2.Length();
                    if (dist < lastDist && dist <= hitTestThickness)
                    {
                        lastDist = dist;
                        result.PointHit = sp;
                        result.NormalAtHit = sp - tp; // not normalized to get length
                        result.Distance = (rayWS.Position - sp).Length();
                        result.RayToLineDistance = rayToLineDistance;
                        result.ModelHit = originalSource;
                        result.IsValid = true;
                        result.Tag = lineIndex; // For compatibility
                        result.LineIndex = lineIndex;
                        result.TriangleIndices = null; // Since triangles are shader-generated
                        result.RayHitPointScalar = sc;
                        result.LineHitPointScalar = tc;
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
    }
}