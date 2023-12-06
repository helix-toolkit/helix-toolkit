using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.CuttingOperation = value;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.CuttingOperation;
            }

            return global::HelixToolkit.SharpDX.CuttingOperation.Intersect;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.SectionColor = value;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.SectionColor;
            }

            return Color.Zero;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                var v = core.PlaneEnabled;
                v.X = value;
                core.PlaneEnabled = v;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.PlaneEnabled.X;
            }

            return false;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                var v = core.PlaneEnabled;
                v.Y = value;
                core.PlaneEnabled = v;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.PlaneEnabled.Y;
            }

            return false;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                var v = core.PlaneEnabled;
                v.Z = value;
                core.PlaneEnabled = v;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.PlaneEnabled.Z;
            }

            return false;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                var v = core.PlaneEnabled;
                v.W = value;
                core.PlaneEnabled = v;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.PlaneEnabled.W;
            }

            return false;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                var v = core.Plane5To8Enabled;
                v.X = value;
                core.Plane5To8Enabled = v;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.Plane5To8Enabled.X;
            }

            return false;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                var v = core.Plane5To8Enabled;
                v.Y = value;
                core.Plane5To8Enabled = v;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.Plane5To8Enabled.Y;
            }

            return false;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                var v = core.Plane5To8Enabled;
                v.Z = value;
                core.Plane5To8Enabled = v;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.Plane5To8Enabled.Z;
            }

            return false;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                var v = core.Plane5To8Enabled;
                v.W = value;
                core.Plane5To8Enabled = v;
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return core.Plane5To8Enabled.W;
            }

            return false;
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.Plane1Params = PlaneToVector(ref value);
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return VectorToPlane(core.Plane1Params);
            }

            return new Plane();
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.Plane2Params = PlaneToVector(ref value);
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return VectorToPlane(core.Plane2Params);
            }

            return new Plane();
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.Plane3Params = PlaneToVector(ref value);
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return VectorToPlane(core.Plane3Params);
            }

            return new Plane();
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.Plane4Params = PlaneToVector(ref value);
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return VectorToPlane(core.Plane4Params);
            }

            return new Plane();
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.Plane5Params = PlaneToVector(ref value);
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return VectorToPlane(core.Plane5Params);
            }

            return new Plane();
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.Plane6Params = PlaneToVector(ref value);
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return VectorToPlane(core.Plane6Params);
            }

            return new Plane();
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.Plane7Params = PlaneToVector(ref value);
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return VectorToPlane(core.Plane7Params);
            }

            return new Plane();
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
            if (RenderCore is ICrossSectionRenderParams core)
            {
                core.Plane8Params = PlaneToVector(ref value);
            }
        }
        get
        {
            if (RenderCore is ICrossSectionRenderParams core)
            {
                return VectorToPlane(core.Plane8Params);
            }

            return new Plane();
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

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.CrossSection];
    }

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new CrossSectionMeshRenderCore();
    }

    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        var hitsBeforeCheck = hits?.Count ?? 0;
        if (Geometry is not MeshGeometry3D meshGeometry3d)
            return false;
        if (meshGeometry3d.ReturnMultipleHitsOnHitTest)
            throw new InvalidOperationException($"All hit tests should be called on the same thread, {nameof(Geometry)}.{nameof(meshGeometry3d.ReturnMultipleHitsOnHitTest)} would not be true if that was the case");
        meshGeometry3d.ReturnMultipleHitsOnHitTest = true;
        var result = meshGeometry3d.HitTest(context, totalModelMatrix, ref hits!, this.WrapperSource);
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
        for (int i = hits.Count - 1; i >= hitsBeforeCheck; i--)
        {
            var pointTimesNormal = (hits[i].PointHit * plane.Normal);
            float distanceToPlane = pointTimesNormal.X + pointTimesNormal.Y + pointTimesNormal.Z - plane.D;
            if (distanceToPlane < 0)
            {
                hits.RemoveAt(i);
            }
        }
        if (hits.Count == hitsBeforeCheck)
            return false;
        return true;
    }

    private bool RemoveHitPointInFrontOfAllCrossingPlanes(List<HitTestResult> hits, int hitsBeforeCheck)
    {
        for (var i = hits.Count - 1; i >= hitsBeforeCheck; i--)
        {
            var hitPoint = hits[i].PointHit;
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
        if (hits.Count - hitsBeforeCheck == 0)
        {
            return;
        }
        var minDistance = double.MaxValue;
        for (var i = hits.Count - 1; i >= hitsBeforeCheck; i--)
        {
            var hit = hits[i];
            if (minDistance > hit.Distance)
            {
                minDistance = hit.Distance;
            }
        }
        if (minDistance < double.MaxValue)
        {
            var foundMinDistance = false;
            // Loop backwards to remove at end of list when possible
            for (var i = hits.Count - 1; i >= hitsBeforeCheck; i--)
            {
                if (hits[i].Distance > minDistance || (foundMinDistance && hits[i].Distance == minDistance))
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
