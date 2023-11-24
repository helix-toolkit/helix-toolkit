using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class LineNode : MaterialGeometryNode
{
    private double hitTestThickness = 1;
    /// <summary>
    /// Used only for point/line hit test
    /// </summary>
    public double HitTestThickness
    {
        set => Set(ref hitTestThickness, value);
        get => hitTestThickness;
    }
    /// <summary>
    /// Called when [create buffer model].
    /// </summary>
    /// <param name="modelGuid"></param>
    /// <param name="geometry"></param>
    /// <returns></returns>
    protected override IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D? geometry)
    {
        var model = geometry != null && geometry.IsDynamic ? EffectsManager?.GeometryBufferManager?.Register<DynamicLineGeometryBufferModel>(modelGuid, geometry)
            : EffectsManager?.GeometryBufferManager?.Register<DefaultLineGeometryBufferModel>(modelGuid, geometry);

        return model ?? EmptyGeometryBufferModel.Empty;
    }

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new PointLineRenderCore();
    }

    /// <summary>
    /// Create raster state description.
    /// </summary>
    /// <returns></returns>
    protected override RasterizerStateDescription CreateRasterState()
    {
        return new RasterizerStateDescription()
        {
            FillMode = FillMode,
            CullMode = CullMode.None,
            DepthBias = DepthBias,
            DepthBiasClamp = -1000,
            SlopeScaledDepthBias = SlopeScaledDepthBias,
            IsDepthClipEnabled = IsDepthClipEnabled,
            IsFrontCounterClockwise = true,

            IsMultisampleEnabled = IsMSAAEnabled,
            //IsAntialiasedLineEnabled = true, // Intel HD 3000 doesn't like this (#10051) and it's not needed
            IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled
        };
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.Lines];
    }

    protected override bool CanRender(RenderContext context)
    {
        if (base.CanRender(context))
        {
            return !context.RenderHost.IsDeferredLighting;
        }
        else
        {
            return false;
        }
    }

    protected override bool OnCheckGeometry(Geometry3D? geometry)
    {
        return base.OnCheckGeometry(geometry) && geometry is LineGeometry3D;
    }

    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        return (Geometry as LineGeometry3D)?.HitTest(context, totalModelMatrix, ref hits, this.WrapperSource, (float)HitTestThickness) ?? false;
    }

    protected override bool PreHitTestOnBounds(HitTestContext? context)
    {
        if (context is null)
        {
            return false;
        }

        var rayWS = context.RayWS;
        return BoundsSphereWithTransform.Intersects(ref rayWS);
    }
}
