using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Model;

public class VolumeMaterialVariable<T> : MaterialVariable
{
    private readonly VolumeTextureMaterialCoreBase<T> material;
    private readonly ShaderPass volumePass;
    private readonly int texSlot, gradientSlot;
    private readonly int samplerSlot;
    private ShaderResourceViewProxy? texture;
    private ShaderResourceViewProxy? transferMap;
    private SamplerStateProxy? sampler;


    public Func<VolumeTextureMaterialCoreBase<T>, IEffectsManager?, ShaderResourceViewProxy?>? OnCreateTexture;

    public VolumeMaterialVariable(IEffectsManager manager, IRenderTechnique technique, VolumeTextureMaterialCoreBase<T> material,
        string volumePassName = DefaultPassNames.Default)
        : base(manager, technique, DefaultVolumeConstantBufferDesc, material)
    {
        this.material = material;
        volumePass = technique[volumePassName];
        texSlot = volumePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.VolumeTB);
        gradientSlot = volumePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.ColorStripe1DXTB);
        samplerSlot = volumePass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.VolumeSampler);
    }

    protected override void OnInitialPropertyBindings()
    {
        base.OnInitialPropertyBindings();
        AddPropertyBinding(nameof(VolumeTextureMaterialCoreBase<T>.VolumeTexture), () => { UpdateTexture(material); });
        AddPropertyBinding(nameof(IVolumeTextureMaterial.Sampler), () =>
        {
            var newSampler = EffectsManager?.StateManager?.Register(material.Sampler);
            RemoveAndDispose(ref sampler);
            sampler = newSampler;
        });
        AddPropertyBinding(nameof(IVolumeTextureMaterial.SampleDistance),
            () => UpdateStepSize());
        AddPropertyBinding(nameof(IVolumeTextureMaterial.MaxIterations),
            () => WriteValue(VolumeParamsStruct.MaxIterations, material.MaxIterations));
        AddPropertyBinding(nameof(IVolumeTextureMaterial.IterationOffset),
            () => WriteValue(VolumeParamsStruct.IterationOffset, material.IterationOffset));
        AddPropertyBinding(nameof(IVolumeTextureMaterial.IsoValue),
            () => WriteValue(VolumeParamsStruct.IsoValue, (float)material.IsoValue));
        AddPropertyBinding(nameof(IVolumeTextureMaterial.Color),
            () => WriteValue(VolumeParamsStruct.Color, material.Color));
        AddPropertyBinding(nameof(IVolumeTextureMaterial.TransferMap),
            () => UpdateGradientMap());
        AddPropertyBinding(nameof(IVolumeTextureMaterial.EnablePlaneAlignment),
            () => WriteValue(VolumeParamsStruct.EnablePlaneAlignment, material.EnablePlaneAlignment));
    }

    private void UpdateStepSize()
    {
        if (texture?.Resource != null && texture.Resource.Dimension == ResourceDimension.Texture3D)
        {
            using var res = texture.Resource.QueryInterface<Texture3D>();
            var desc = res.Description;
            var maxSize = Math.Max(desc.Width, Math.Max(desc.Height, desc.Depth));
            var steps = 1f / maxSize * (float)material.SampleDistance;
            WriteValue(VolumeParamsStruct.StepSize, steps);
        }
        else
        {
            WriteValue(VolumeParamsStruct.StepSize, 1);
        }
        WriteValue(VolumeParamsStruct.ActualSampleDistance, (float)material.SampleDistance);
        WriteValue(VolumeParamsStruct.BaseSampleDistance, 1.0f);
    }

    private void UpdateTexture(VolumeTextureMaterialCoreBase<T> material)
    {
        var newTexture = OnCreateTexture?.Invoke(material, EffectsManager);
        RemoveAndDispose(ref texture);
        texture = newTexture;
        if (texture != null)
        {
            UpdateStepSize();
        }
    }

    public void UpdateGradientMap()
    {
        RemoveAndDispose(ref transferMap);
        if (material.TransferMap != null && EffectsManager?.Device is not null)
        {
            transferMap = ShaderResourceViewProxy.CreateViewFromColorArray(EffectsManager.Device, material.TransferMap);
        }
        WriteValue(VolumeParamsStruct.HasGradientMapX, material.TransferMap != null);
    }

    public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
    {
        if (texture != null)
        {
            shaderPass.PixelShader.BindTexture(deviceContext, texSlot, texture);
            shaderPass.PixelShader.BindTexture(deviceContext, gradientSlot, transferMap);
            shaderPass.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
    {
        if (bufferModel.IndexBuffer is null)
        {
            return;
        }

        DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
    }

    public override ShaderPass GetPass(RenderType renderType, RenderContext context)
    {
        return volumePass;
    }

    public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
    {
        return ShaderPass.NullPass;
    }

    public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
    {
        return ShaderPass.NullPass;
    }

    public override ShaderPass GetDepthPass(RenderType renderType, RenderContext context)
    {
        return ShaderPass.NullPass;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref texture);
        RemoveAndDispose(ref transferMap);
        RemoveAndDispose(ref sampler);
        base.OnDispose(disposeManagedResources);
    }
}
