/*
The MIT License(MIT)
Copyright(c) 2020 Helix Toolkit contributors
*/

using System;
using SharpDX;

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
    namespace Model.Scene
    {
        using Core;
        using System.Collections.Generic;

        /// <summary>
        /// 
        /// </summary>
        public class CrossSectionMeshNode : MeshNode
        {
            /// <summary>
            /// Gets or sets the cutting operation.
            /// </summary>
            /// <value>
            /// The cutting operation.
            /// </value>
            public CuttingOperation CuttingOperation
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).CuttingOperation = value;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).CuttingOperation;
                }
            }
            /// <summary>
            /// Gets or sets the color of the cross section.
            /// </summary>
            /// <value>
            /// The color of the cross section.
            /// </value>
            public Color4 CrossSectionColor
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).SectionColor = value;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).SectionColor;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether [enable plane1].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable plane1]; otherwise, <c>false</c>.
            /// </value>
            public bool EnablePlane1
            {
                set
                {
                    var v = (RenderCore as ICrossSectionRenderParams).PlaneEnabled;
                    v.X = value;
                    (RenderCore as ICrossSectionRenderParams).PlaneEnabled = v;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).PlaneEnabled.X;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether [enable plane2].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable plane2]; otherwise, <c>false</c>.
            /// </value>
            public bool EnablePlane2
            {
                set
                {
                    var v = (RenderCore as ICrossSectionRenderParams).PlaneEnabled;
                    v.Y = value;
                    (RenderCore as ICrossSectionRenderParams).PlaneEnabled = v;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).PlaneEnabled.Y;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether [enable plane3].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable plane3]; otherwise, <c>false</c>.
            /// </value>
            public bool EnablePlane3
            {
                set
                {
                    var v = (RenderCore as ICrossSectionRenderParams).PlaneEnabled;
                    v.Z = value;
                    (RenderCore as ICrossSectionRenderParams).PlaneEnabled = v;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).PlaneEnabled.Z;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether [enable plane4].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable plane4]; otherwise, <c>false</c>.
            /// </value>
            public bool EnablePlane4
            {
                set
                {
                    var v = (RenderCore as ICrossSectionRenderParams).PlaneEnabled;
                    v.W = value;
                    (RenderCore as ICrossSectionRenderParams).PlaneEnabled = v;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).PlaneEnabled.W;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [enable plane5].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable plane5]; otherwise, <c>false</c>.
            /// </value>
            public bool EnablePlane5
            {
                set
                {
                    var v = (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled;
                    v.X = value;
                    (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled = v;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled.X;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [enable plane6].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable plane6]; otherwise, <c>false</c>.
            /// </value>
            public bool EnablePlane6
            {
                set
                {
                    var v = (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled;
                    v.Y = value;
                    (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled = v;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled.Y;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [enable plane7].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable plane7]; otherwise, <c>false</c>.
            /// </value>
            public bool EnablePlane7
            {
                set
                {
                    var v = (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled;
                    v.Z = value;
                    (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled = v;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled.Z;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [enable plane8].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable plane8]; otherwise, <c>false</c>.
            /// </value>
            public bool EnablePlane8
            {
                set
                {
                    var v = (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled;
                    v.W = value;
                    (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled = v;
                }
                get
                {
                    return (RenderCore as ICrossSectionRenderParams).Plane5To8Enabled.W;
                }
            }

            /// <summary>
            /// Gets or sets the plane1.
            /// </summary>
            /// <value>
            /// The plane1.
            /// </value>
            public Plane Plane1
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).Plane1Params = PlaneToVector(ref value);
                }
                get
                {
                    return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane1Params);
                }
            }
            /// <summary>
            /// Gets or sets the plane2.
            /// </summary>
            /// <value>
            /// The plane2.
            /// </value>
            public Plane Plane2
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).Plane2Params = PlaneToVector(ref value);
                }
                get
                {
                    return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane2Params);
                }
            }
            /// <summary>
            /// Gets or sets the plane3.
            /// </summary>
            /// <value>
            /// The plane3.
            /// </value>
            public Plane Plane3
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).Plane3Params = PlaneToVector(ref value);
                }
                get
                {
                    return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane3Params);
                }
            }
            /// <summary>
            /// Gets or sets the plane4.
            /// </summary>
            /// <value>
            /// The plane4.
            /// </value>
            public Plane Plane4
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).Plane4Params = PlaneToVector(ref value);
                }
                get
                {
                    return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane4Params);
                }
            }

            /// <summary>
            /// Gets or sets the plane5.
            /// </summary>
            /// <value>
            /// The plane5.
            /// </value>
            public Plane Plane5
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).Plane5Params = PlaneToVector(ref value);
                }
                get
                {
                    return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane5Params);
                }
            }

            /// <summary>
            /// Gets or sets the plane6.
            /// </summary>
            /// <value>
            /// The plane6.
            /// </value>
            public Plane Plane6
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).Plane6Params = PlaneToVector(ref value);
                }
                get
                {
                    return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane6Params);
                }
            }

            /// <summary>
            /// Gets or sets the plane7.
            /// </summary>
            /// <value>
            /// The plane7.
            /// </value>
            public Plane Plane7
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).Plane7Params = PlaneToVector(ref value);
                }
                get
                {
                    return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane7Params);
                }
            }

            /// <summary>
            /// Gets or sets the plane8.
            /// </summary>
            /// <value>
            /// The plane8.
            /// </value>
            public Plane Plane8
            {
                set
                {
                    (RenderCore as ICrossSectionRenderParams).Plane8Params = PlaneToVector(ref value);
                }
                get
                {
                    return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane8Params);
                }
            }

            /// <summary>
            /// The PlaneToVector
            /// </summary>
            /// <param name="p">The <see cref="Plane"/></param>
            /// <returns>The <see cref="Vector4"/></returns>
            private static Vector4 PlaneToVector(ref Plane p)
            {
                return new Vector4(p.Normal, p.D);
            }

            private static Plane VectorToPlane(Vector4 v)
            {
                return new Plane(v.ToXYZ(), v.W);
            }

            /// <summary>
            /// Override this function to set render technique during Attach Host.
            /// <para>If <see cref="SceneNode.OnSetRenderTechnique" /> is set, then <see cref="SceneNode.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
            /// </summary>
            /// <param name="host"></param>
            /// <returns>
            /// Return RenderTechnique
            /// </returns>
            protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
            {
                return host.EffectsManager[DefaultRenderTechniqueNames.CrossSection];
            }

            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                return new CrossSectionMeshRenderCore();
            }

            protected override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
            {
                int hitsBeforeCheck = hits?.Count ?? 0;
                var meshGeometry3d = Geometry as MeshGeometry3D;
                if (meshGeometry3d == null)
                    return false;
                if(meshGeometry3d.ReturnMultipleHitsOnHitTest)
                    throw new InvalidOperationException($"All hit tests should be called on the same thread, {nameof(Geometry)}.{nameof(meshGeometry3d.ReturnMultipleHitsOnHitTest)} would not be true if that was the case");
                meshGeometry3d.ReturnMultipleHitsOnHitTest = true;
                bool result = meshGeometry3d.HitTest(context, totalModelMatrix, ref hits, this.WrapperSource);
                meshGeometry3d.ReturnMultipleHitsOnHitTest = false;
                var operation = CuttingOperation;
                if (result)
                {
                    switch (operation)
                    {
                        case CuttingOperation.Intersect:
                            // Remove any hit point behinds any of the clip plane.
                            if (EnablePlane1)
                                result = RemoveHitPointBehindCrossingPlane(Plane1, hits, hitsBeforeCheck);
                            if (result && EnablePlane2)
                                result = RemoveHitPointBehindCrossingPlane(Plane2, hits, hitsBeforeCheck);
                            if (result && EnablePlane3)
                                result = RemoveHitPointBehindCrossingPlane(Plane3, hits, hitsBeforeCheck);
                            if (result && EnablePlane4)
                                result = RemoveHitPointBehindCrossingPlane(Plane4, hits, hitsBeforeCheck);
                            if (result && EnablePlane5)
                                result = RemoveHitPointBehindCrossingPlane(Plane5, hits, hitsBeforeCheck);
                            if (result && EnablePlane6)
                                result = RemoveHitPointBehindCrossingPlane(Plane6, hits, hitsBeforeCheck);
                            if (result && EnablePlane7)
                                result = RemoveHitPointBehindCrossingPlane(Plane7, hits, hitsBeforeCheck);
                            if (result && EnablePlane8)
                                result = RemoveHitPointBehindCrossingPlane(Plane8, hits, hitsBeforeCheck);
                            break;
                        case CuttingOperation.Subtract:
                            // Remove any hit point in front of all clip planes
                            result = RemoveHitPointInFrontOfAllCrossingPlanes(hits, hitsBeforeCheck);
                            break;
                    }
                    if (result)
                        RemoveAllButClosest(hits, hitsBeforeCheck);
                }
                return result;
            }

            private static bool RemoveHitPointBehindCrossingPlane(Plane plane, List<HitTestResult> hits, int hitsBeforeCheck)
            {
                // Loop backwards to remove at end of list when possible
                for (int i = hits.Count-1; i >= hitsBeforeCheck; i--)
                {
                    if (hits[i].PointHit.PointToPlanePosition(ref plane) == PlaneIntersectionType.Back)
                    {
                        hits.RemoveAt(i);
                    }
                }
                return hits.Count > hitsBeforeCheck;
            }

            private bool RemoveHitPointInFrontOfAllCrossingPlanes(List<HitTestResult> hits, int hitsBeforeCheck)
            {
                for (int i = hits.Count - 1; i >= hitsBeforeCheck; i--)
                {
                    Vector3 hitPoint = hits[i].PointHit;
                    if (EnablePlane1)
                    {
                        if (hitPoint.PointToPlanePosition(Plane1) != PlaneIntersectionType.Front)
                        {
                            continue;
                        }
                    }
                    if (EnablePlane2)
                    {
                        if (hitPoint.PointToPlanePosition(Plane2) != PlaneIntersectionType.Front)
                        {
                            continue;
                        }
                    }
                    if (EnablePlane3)
                    {
                        if (hitPoint.PointToPlanePosition(Plane3) != PlaneIntersectionType.Front)
                        {
                            continue;
                        }
                    }
                    if (EnablePlane4)
                    {
                        if (hitPoint.PointToPlanePosition(Plane4) != PlaneIntersectionType.Front)
                        {
                            continue;
                        }
                    }
                    if (EnablePlane5)
                    {
                        if (hitPoint.PointToPlanePosition(Plane5) != PlaneIntersectionType.Front)
                        {
                            continue;
                        }
                    }
                    if (EnablePlane6)
                    {
                        if (hitPoint.PointToPlanePosition(Plane6) != PlaneIntersectionType.Front)
                        {
                            continue;
                        }
                    }
                    if (EnablePlane7)
                    {
                        if (hitPoint.PointToPlanePosition(Plane7) != PlaneIntersectionType.Front)
                        {
                            continue;
                        }
                    }
                    if (EnablePlane8)
                    {
                        if (hitPoint.PointToPlanePosition(Plane8) != PlaneIntersectionType.Front)
                        {
                            continue;
                        }
                    }
                    hits.RemoveAt(i);
                }
                return hits.Count > hitsBeforeCheck;
            }

            /// <summary>
            /// Removes all but the closes hit point, this is done here despite similar checks being done further up the call stack. 
            /// This minimizes the risk of breaking callers assumptions of one hit per object
            /// </summary>
            /// <param name="hits">All hits so far</param>
            /// <param name="hitsBeforeCheck">The number of hits before this object was processed</param>
            private static void RemoveAllButClosest(List<HitTestResult> hits, int hitsBeforeCheck)
            {
                if(hits.Count - hitsBeforeCheck == 0)
                {
                    return;
                }
                double minDistance = double.MaxValue;
                for (int i = hits.Count - 1; i >= hitsBeforeCheck; i--)
                {
                    var hit = hits[i];
                    if (minDistance > hit.Distance)
                    {
                        minDistance = hit.Distance;
                    }
                }
                if(minDistance < double.MaxValue)
                {
                    bool foundMinDistance = false;
                    // Loop backwards to remove at end of list when possible
                    for (int i = hits.Count - 1; i >= hitsBeforeCheck; i--)
                    {
                        if(hits[i].Distance > minDistance || (foundMinDistance && hits[i].Distance == minDistance))
                        {
                            hits.RemoveAt(i);
                        }
                        else
                        {
                            foundMinDistance = true;
                        }
                    }
                }
            }
        }
    }

}