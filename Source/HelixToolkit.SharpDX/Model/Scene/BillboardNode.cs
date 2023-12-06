using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class BillboardNode : MaterialGeometryNode
{
    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new PointLineRenderCore();
    }

    /// <summary>
    /// Called when [create buffer model].
    /// </summary>
    /// <param name="modelGuid"></param>
    /// <param name="geometry"></param>
    /// <returns></returns>
    protected override IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D? geometry)
    {
        var buffer = geometry != null && geometry.IsDynamic ? EffectsManager?.GeometryBufferManager?.Register<DynamicBillboardBufferModel>(modelGuid, geometry)
            : EffectsManager?.GeometryBufferManager?.Register<DefaultBillboardBufferModel>(modelGuid, geometry);
        if (geometry is IBillboardText b && Material is IBillboardRenderParams m)
        {
            m.Type = b.Type;
        }
        return buffer ?? EmptyGeometryBufferModel.Empty;
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager?[DefaultRenderTechniqueNames.BillboardText];
    }

    public override bool TestViewFrustum(ref BoundingFrustum viewFrustum)
    {
        if (!EnableViewFrustumCheck)
        {
            return true;
        }
        if (Geometry is IBillboardText billboard && !billboard.IsInitialized)
        {
            return true;
        }
        return BoundingFrustumExtensions.Intersects(ref viewFrustum, ref BoundManager.BoundsSphereWithTransform);// viewFrustum.Intersects(ref sphere);
    }

    /// <summary>
    /// Called when [check geometry].
    /// </summary>
    /// <param name="geometry">The geometry.</param>
    /// <returns></returns>
    protected override bool OnCheckGeometry(Geometry3D? geometry)
    {
        return geometry is IBillboardText;
    }
    /// <summary>
    /// Create raster state description.
    /// </summary>
    /// <returns></returns>
    protected override RasterizerStateDescription CreateRasterState()
    {
        return new RasterizerStateDescription()
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.None,
            DepthBias = DepthBias,
            DepthBiasClamp = -1000,
            SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
            IsDepthClipEnabled = true,
            IsFrontCounterClockwise = false,

            IsMultisampleEnabled = false,
            //IsAntialiasedLineEnabled = true,                    
            IsScissorEnabled = !IsThrowingShadow && IsScissorEnabled,
        };
    }

    /// <summary>
    /// Called when [hit test].
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="totalModelMatrix">The total model matrix.</param>
    /// <param name="hits">The hits.</param>
    /// <returns></returns>
    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        if (Material is BillboardMaterialCore c)
        {
            return (Geometry as BillboardBase)?.HitTest(context, totalModelMatrix, ref hits, this.WrapperSource, c.FixedSize) ?? false;
        }
        else
        {
            return false;
        }
    }

    protected override bool PreHitTestOnBounds(HitTestContext? context)
    {
        return true;
    }
}
