using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using SharpDX;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// 
/// </summary>
public class PointMaterialVariable : MaterialVariable
{
    private readonly PointMaterialCore material;

    public ShaderPass PointPass
    {
        get;
    }
    public ShaderPass ShadowPass
    {
        get;
    }
    public ShaderPass DepthPass
    {
        get;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointMaterialVariable"/> class.
    /// </summary>
    /// <param name="manager">The manager.</param>
    /// <param name="technique">The technique.</param>
    /// <param name="materialCore">The material core.</param>
    /// <param name="defaultPassName">Default pass name</param>
    public PointMaterialVariable(IEffectsManager manager, IRenderTechnique technique, PointMaterialCore materialCore,
        string defaultPassName = DefaultPassNames.Default)
        : base(manager, technique, DefaultPointLineConstantBufferDesc, materialCore)
    {
        PointPass = technique[defaultPassName];
        ShadowPass = technique[DefaultPassNames.ShadowPass];
        DepthPass = technique[DefaultPassNames.DepthPrepass];
        this.material = materialCore;
    }

    protected override void OnInitialPropertyBindings()
    {
        AddPropertyBinding(nameof(PointMaterialCore.PointColor), () => { WriteValue(PointLineMaterialStruct.ColorStr, material.PointColor); });
        AddPropertyBinding(nameof(PointMaterialCore.Width), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4(material.Width, material.Height, (int)material.Figure, material.FigureRatio)); });
        AddPropertyBinding(nameof(PointMaterialCore.Height), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4(material.Width, material.Height, (int)material.Figure, material.FigureRatio)); });
        AddPropertyBinding(nameof(PointMaterialCore.Figure), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4(material.Width, material.Height, (int)material.Figure, material.FigureRatio)); });
        AddPropertyBinding(nameof(PointMaterialCore.FigureRatio), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4(material.Width, material.Height, (int)material.Figure, material.FigureRatio)); });
        AddPropertyBinding(nameof(PointMaterialCore.EnableDistanceFading), () => { WriteValue(PointLineMaterialStruct.EnableDistanceFading, material.EnableDistanceFading ? 1 : 0); });
        AddPropertyBinding(nameof(PointMaterialCore.FadingNearDistance), () => { WriteValue(PointLineMaterialStruct.FadeNearDistance, material.FadingNearDistance); });
        AddPropertyBinding(nameof(PointMaterialCore.FadingFarDistance), () => { WriteValue(PointLineMaterialStruct.FadeFarDistance, material.FadingFarDistance); });
        AddPropertyBinding(nameof(PointMaterialCore.FixedSize), () => { WriteValue(PointLineMaterialStruct.FixedSize, material.FixedSize); });
        AddPropertyBinding(nameof(PointMaterialCore.EnableColorBlending), () => { WriteValue(PointLineMaterialStruct.EnableBlendingStr, material.EnableColorBlending); });
        AddPropertyBinding(nameof(PointMaterialCore.BlendingFactor), () => { WriteValue(PointLineMaterialStruct.BlendingFactorStr, material.BlendingFactor); });
    }

    public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
    {
        if (bufferModel.VertexBuffer[0] is null)
        {
            return;
        }

        DrawPoints(deviceContext, bufferModel.VertexBuffer[0]!.ElementCount, instanceCount);
    }

    public override ShaderPass GetPass(RenderType renderType, RenderContext context)
    {
        return PointPass;
    }

    public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
    {
        return ShadowPass;
    }

    public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
    {
        return ShaderPass.NullPass;
    }

    public override ShaderPass GetDepthPass(RenderType renderType, RenderContext context)
    {
        return DepthPass;
    }

    public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
    {
        return true;
    }

    protected override void UpdateInternalVariables(DeviceContextProxy deviceContext)
    {
    }
}
