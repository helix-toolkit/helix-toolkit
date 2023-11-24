using HelixToolkit.SharpDX.Core.Components;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

public class DrawScreenQuadCore : RenderCore
{
    private string passName = DefaultPassNames.Default;
    public string PassName
    {
        set
        {
            if (SetAffectsRender(ref passName, value) && IsAttached)
            {
                pass = EffectTechnique?[value];
                textureSlot = pass?.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB) ?? 0;
                samplerSlot = pass?.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultSamplerStateNames.SurfaceSampler) ?? 0;
            }
        }
        get
        {
            return passName;
        }
    }

    public ScreenQuadModelStruct ModelStruct;

    private TextureModel? texture;
    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>
    /// The texture.
    /// </value>
    public TextureModel? Texture
    {
        set
        {
            if (SetAffectsRender(ref texture, value) && IsAttached)
            {
                UpdateTexture(value);
            }
        }
        get
        {
            return texture;
        }
    }
    private SamplerStateDescription samplerDescription = DefaultSamplers.LinearSamplerClampAni1;
    /// <summary>
    /// Gets or sets the sampler description.
    /// </summary>
    /// <value>
    /// The sampler description.
    /// </value>
    public SamplerStateDescription SamplerDescription
    {
        set
        {
            if (SetAffectsRender(ref samplerDescription, value) && IsAttached)
            {

            }
        }
        get
        {
            return samplerDescription;
        }
    }

    private ShaderPass? pass;
    private readonly ConstantBufferComponent modelCB;
    private ShaderResourceViewProxy? textureProxy;
    private SamplerStateProxy? sampler;
    private int textureSlot;
    private int samplerSlot;

    public DrawScreenQuadCore() : base(RenderType.Opaque)
    {
        modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.ScreenQuadCB, ScreenQuadModelStruct.SizeInBytes)));
        ModelStruct = new ScreenQuadModelStruct()
        {
            TopLeft = new Vector4(-1, 1, 1, 1),
            TopRight = new Vector4(1, 1, 1, 1),
            BottomLeft = new Vector4(-1, -1, 1, 1),
            BottomRight = new Vector4(1, -1, 1, 1),
            TexTopLeft = new Vector2(0, 1),
            TexTopRight = new Vector2(1, 1),
            TexBottomLeft = new Vector2(0, 0),
            TexBottomRight = new Vector2(1, 0),
        };
    }

    private void UpdateTexture(TextureModel? texture)
    {
        var newTexture = texture == null ?
            null : EffectTechnique?.EffectsManager?.MaterialTextureManager?.Register(texture);
        RemoveAndDispose(ref textureProxy);
        textureProxy = newTexture;
    }

    private void UpdateSampler()
    {
        var newSampler = EffectTechnique?.EffectsManager?.StateManager?.Register(samplerDescription);
        RemoveAndDispose(ref sampler);
        sampler = newSampler;
    }

    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {
        if (pass is null || pass.IsNULL)
        {
            return;
        }
        ModelStruct.mWorld = ModelMatrix;
        modelCB.Upload(deviceContext, ref ModelStruct);
        pass.BindShader(deviceContext);
        pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
        pass.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
        pass.PixelShader.BindTexture(deviceContext, textureSlot, textureProxy);
        deviceContext.Draw(4, 0);
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        pass = technique?[passName];
        textureSlot = pass?.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB) ?? 0;
        samplerSlot = pass?.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SurfaceSampler) ?? 0;
        UpdateTexture(texture);
        UpdateSampler();
        return true;
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref textureProxy);
        RemoveAndDispose(ref sampler);
    }
}
