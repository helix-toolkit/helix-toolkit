/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUG
using SharpDX;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
    {
        /// <summary>
        /// Static octree for line geometry
        /// </summary>
        public class StaticLineGeometryOctree : StaticOctree<KeyValuePair<int, BoundingBox>>
        {
            /// <summary>
            /// 
            /// </summary>
            protected readonly IList<Vector3> Positions;
            /// <summary>
            /// 
            /// </summary>
            protected readonly IList<int> Indices;
            /// <summary>
            /// Initializes a new instance of the <see cref="StaticLineGeometryOctree"/> class.
            /// </summary>
            /// <param name="positions">The positions.</param>
            /// <param name="indices">The indices.</param>
            /// <param name="parameter">The parameter.</param>
            public StaticLineGeometryOctree(IList<Vector3> positions, IList<int> indices, OctreeBuildParameter parameter)
                : base(parameter)
            {
                Positions = positions;
                Indices = indices;
            }
            /// <summary>
            /// Gets the objects.
            /// </summary>
            /// <returns></returns>
            protected override KeyValuePair<int, BoundingBox>[] GetObjects()
            {
                var objects = new KeyValuePair<int, BoundingBox>[Indices.Count / 2];
                // Construct triangle index and its bounding box KeyValuePair
                for (int i = 0; i < Indices.Count / 2; ++i)
                {
                    objects[i] = new KeyValuePair<int, BoundingBox>(i, GetBoundingBox(i));
                }
                return objects;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private BoundingBox GetBoundingBox(int triangleIndex)
            {
                var actual = triangleIndex * 2;
                var v1 = Positions[Indices[actual++]];
                var v2 = Positions[Indices[actual]];
                var maxX = Math.Max(v1.X, v2.X);
                var maxY = Math.Max(v1.Y, v2.Y);
                var maxZ = Math.Max(v1.Z, v2.Z);

                var minX = Math.Min(v1.X, v2.X);
                var minY = Math.Min(v1.Y, v2.Y);
                var minZ = Math.Min(v1.Z, v2.Z);

                return new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            }

            protected override BoundingBox GetMaxBound()
            {
                return BoundingBoxExtensions.FromPoints(Positions);
            }

            protected override BoundingBox GetBoundingBoxFromItem(ref KeyValuePair<int, BoundingBox> item)
            {
                return item.Value;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="octant"></param>
            /// <param name="context"></param>
            /// <param name="model"></param>
            /// <param name="geometry"></param>
            /// <param name="modelMatrix"></param>
            /// <param name="rayWS"></param>
            /// <param name="rayModel"></param>
            /// <param name="hits"></param>
            /// <param name="isIntersect"></param>
            /// <param name="hitThickness"></param>
            /// <returns></returns>
            protected override bool HitTestCurrentNodeExcludeChild(ref Octant octant, RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
            {
                isIntersect = false;
                if (!octant.IsBuilt)
                {
                    return false;
                }
                var isHit = false;
                var bound = octant.Bound;
                bound.Maximum += new Vector3(hitThickness);
                bound.Minimum -= new Vector3(hitThickness);
                var lastDist = double.MaxValue;
                //Hit test in local space.
                if (rayModel.Intersects(ref bound))
                {
                    isIntersect = true;
                    if (octant.Count == 0)
                    {
                        return false;
                    }
                    var result = new LineHitTestResult { IsValid = false, Distance = double.MaxValue };
                    result.Distance = double.MaxValue;
                    for (int i = octant.Start; i < octant.End; ++i)
                    {
                        var idx = Objects[i].Key * 2;
                        var idx1 = Indices[idx];
                        var idx2 = Indices[idx + 1];
                        var v0 = Positions[idx1];
                        var v1 = Positions[idx2];

                        var t0 = Vector3.TransformCoordinate(v0, modelMatrix);
                        var t1 = Vector3.TransformCoordinate(v1, modelMatrix);
                        var rayToLineDistance = LineBuilder.GetRayToLineDistance(rayWS, t0, t1, out Vector3 sp, out Vector3 tp, out float sc, out float tc);
                        var svpm = context.ScreenViewProjectionMatrix;
                        Vector3.TransformCoordinate(ref sp, ref svpm, out var sp3);
                        Vector3.TransformCoordinate(ref tp, ref svpm, out var tp3);
                        var tv2 = new Vector2(tp3.X - sp3.X, tp3.Y - sp3.Y);
                        var dist = tv2.Length();
                        if (dist < lastDist && dist <= hitThickness)
                        {
                            lastDist = dist;
                            result.PointHit = sp;
                            result.NormalAtHit = sp - tp; // not normalized to get length
                            result.Distance = (rayWS.Position - sp).Length();
                            result.RayToLineDistance = rayToLineDistance;
                            result.ModelHit = model;
                            result.IsValid = true;
                            result.Tag = idx; // For compatibility
                            result.LineIndex = idx;
                            result.TriangleIndices = null; // Since triangles are shader-generated
                            result.RayHitPointScalar = sc;
                            result.LineHitPointScalar = tc;
                            result.Geometry = geometry;
                            isHit = true;
                        }
                    }

                    if (isHit)
                    {
                        isHit = false;
                        if (hits.Count > 0)
                        {
                            if (hits[0].Distance > result.Distance)
                            {
                                hits[0] = result;
                                isHit = true;
                            }
                        }
                        else
                        {
                            hits.Add(result);
                            isHit = true;
                        }
                    }
                }
                return isHit;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="octant"></param>
            /// <param name="context"></param>
            /// <param name="sphere"></param>
            /// <param name="result"></param>
            /// <param name="isIntersect"></param>
            /// <returns></returns>
            protected override bool FindNearestPointBySphereExcludeChild(ref Octant octant, RenderContext context, ref BoundingSphere sphere,
                ref List<HitTestResult> result, ref bool isIntersect)
            {
                bool isHit = false;
                var tempResult = new LineHitTestResult();
                tempResult.Distance = float.MaxValue;
                if (!BoxDisjointSphere(octant.Bound, ref sphere))
                {
                    isIntersect = true;
                    for (int i = octant.Start; i < octant.End; ++i)
                    {
                        if (!BoxDisjointSphere(Objects[i].Value, ref sphere))
                        {
                            var idx = Objects[i].Key * 3;
                            var t1 = Indices[idx];
                            var t2 = Indices[idx + 1];
                            var v0 = Positions[t1];
                            var v1 = Positions[t2];
                            float distance = LineBuilder.GetPointToLineDistance2D(ref sphere.Center, ref v0, ref v1, out Vector3 cloestPoint, out float t);
                            if (tempResult.Distance > distance)
                            {
                                tempResult.Distance = distance;
                                tempResult.IsValid = true;
                                tempResult.PointHit = cloestPoint;
                                tempResult.TriangleIndices = null;
                                tempResult.RayHitPointScalar = t;
                                tempResult.Tag = tempResult.LineIndex = Objects[i].Key;
                                isHit = true;
                            }
                        }
                    }
                    if (isHit)
                    {
                        isHit = false;
                        if (result.Count > 0)
                        {
                            if (result[0].Distance > tempResult.Distance)
                            {
                                result[0] = tempResult;
                                isHit = true;
                            }
                        }
                        else
                        {
                            result.Add(tempResult);
                            isHit = true;
                        }
                    }
                }
                else
                {
                    isIntersect = false;
                }
                return isHit;
            }
        }
    }

}
