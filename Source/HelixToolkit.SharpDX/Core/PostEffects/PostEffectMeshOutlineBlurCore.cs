using HelixToolkit.SharpDX.Core.Components;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// Outline blur effect
/// <para>Must not put in shared model across multiple viewport, otherwise may causes performance issue if each viewport sizes are different.</para>
/// </summary>
public class PostEffectMeshOutlineBlurCore : RenderCore, IPostEffectOutlineBlur
{
    #region Variables
    private SamplerStateProxy? sampler;
    private PostEffectBlurCore? blurCore;
    private ShaderPass? screenQuadPass;

    private ShaderPass? blurPassVertical;

    private ShaderPass? blurPassHorizontal;

    private ShaderPass? smoothPass;

    private ShaderPass? screenOutlinePass;

    private int textureSlot;

    private int samplerSlot;

    private readonly ConstantBufferComponent modelCB;
    private BorderEffectStruct modelStruct;
    private static readonly OffScreenTextureSize TextureSize = OffScreenTextureSize.Full;
    private static readonly Color4 Transparent = new(0, 0, 0, 0);

    private readonly bool useBlurCore = true;
    #endregion
    #region Properties
    private string effectName = DefaultRenderTechniqueNames.PostEffectMeshOutlineBlur;
    /// <summary>
    /// Gets or sets the name of the effect.
    /// </summary>
    /// <value>
    /// The name of the effect.
    /// </value>
    public string EffectName
    {
        set
        {
            SetAffectsCanRenderFlag(ref effectName, value);
        }
        get
        {
            return effectName;
        }
    }

    private Color4 color = Maths.Color.Red;
    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>
    /// The color of the border.
    /// </value>
    public Color4 Color
    {
        set
        {
            SetAffectsRender(ref color, value);
        }
        get
        {
            return color;
        }
    }

    private float scaleX = 1;
    /// <summary>
    /// Gets or sets the scale x.
    /// </summary>
    /// <value>
    /// The scale x.
    /// </value>
    public float ScaleX
    {
        set
        {
            SetAffectsRender(ref scaleX, value);
        }
        get
        {
            return scaleX;
        }
    }

    private float scaleY = 1;
    /// <summary>
    /// Gets or sets the scale y.
    /// </summary>
    /// <value>
    /// The scale y.
    /// </value>
    public float ScaleY
    {
        set
        {
            SetAffectsRender(ref scaleY, value);
        }
        get
        {
            return scaleY;
        }
    }

    private int numberOfBlurPass = 1;
    /// <summary>
    /// Gets or sets the number of blur pass.
    /// </summary>
    /// <value>
    /// The number of blur pass.
    /// </value>
    public int NumberOfBlurPass
    {
        set
        {
            SetAffectsRender(ref numberOfBlurPass, value);
        }
        get
        {
            return numberOfBlurPass;
        }
    }

    private OutlineMode drawMode = OutlineMode.Merged;

    public OutlineMode DrawMode
    {
        set
        {
            SetAffectsRender(ref drawMode, value);
        }
        get
        {
            return drawMode;
        }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PostEffectMeshOutlineBlurCore"/> class.
    /// </summary>
    public PostEffectMeshOutlineBlurCore(bool useBlurCore = true) : base(RenderType.PostEffect)
    {
        this.useBlurCore = useBlurCore;
        Color = Maths.Color.Red;
        modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes)));
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        screenQuadPass = technique?.GetPass(DefaultPassNames.ScreenQuad);
        blurPassVertical = technique?.GetPass(DefaultPassNames.EffectBlurVertical);
        blurPassHorizontal = technique?.GetPass(DefaultPassNames.EffectBlurHorizontal);
        smoothPass = technique?.GetPass(DefaultPassNames.EffectOutlineSmooth);
        screenOutlinePass = technique?.GetPass(DefaultPassNames.MeshOutline);
        textureSlot = screenOutlinePass?.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB) ?? 0;
        samplerSlot = screenOutlinePass?.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SurfaceSampler) ?? 0;
        sampler = technique?.EffectsManager?.StateManager?.Register(DefaultSamplers.LinearSamplerClampAni1);
        if (useBlurCore && blurPassVertical is not null && blurPassHorizontal is not null)
        {
            blurCore = new PostEffectBlurCore(blurPassVertical,
                blurPassHorizontal, textureSlot, samplerSlot, DefaultSamplers.LinearSamplerClampAni1, technique?.EffectsManager);
        }
        return true;
    }

    protected override bool OnUpdateCanRenderFlag()
    {
        return IsAttached && !string.IsNullOrEmpty(EffectName);
    }

    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {
        using var depthStencilBuffer = context.GetOffScreenDS(TextureSize, global::SharpDX.DXGI.Format.D32_Float_S8X24_UInt,
            out var width, out var height);
        using var renderTargetBuffer = context.GetOffScreenRT(TextureSize, global::SharpDX.DXGI.Format.R8G8B8A8_UNorm);
        OnUpdatePerModelStruct(context);
        var viewport = context.Viewport;
        if (drawMode == OutlineMode.Separated)
        {
            for (var i = 0; i < context.RenderHost.PerFrameNodesWithPostEffect.Count; ++i)
            {
                #region Render objects onto offscreen texture
                var mesh = context.RenderHost.PerFrameNodesWithPostEffect[i];
                deviceContext.SetRenderTarget(depthStencilBuffer, renderTargetBuffer, true,
                    Maths.Color.Transparent, true, DepthStencilClearFlags.Stencil, 0, 0);
                deviceContext.SetViewport(ref viewport);
                deviceContext.SetScissorRectangle(ref viewport);
                if (mesh.TryGetPostEffect(EffectName, out var effect))
                {
                    var color = Color;
                    if (effect is not null && effect.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out var attribute) && attribute is string colorStr)
                    {
                        color = colorStr.ToColor4();
                    }
                    if (modelStruct.Color != color)
                    {
                        modelStruct.Color = color;
                        modelCB.Upload(deviceContext, ref modelStruct);
                    }
                    context.CustomPassName = DefaultPassNames.EffectOutlineP1;
                    var pass = mesh?.EffectTechnique?[DefaultPassNames.EffectOutlineP1];
                    if (pass is null || pass.IsNULL)
                    {
                        continue;
                    }
                    pass.BindShader(deviceContext);
                    pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                    mesh?.RenderCustom(context, deviceContext);
                    DrawOutline(context, deviceContext, depthStencilBuffer, renderTargetBuffer);
                }
                #endregion
            }
        }
        else
        {
            deviceContext.SetRenderTarget(depthStencilBuffer, renderTargetBuffer, true,
                    Transparent, true, DepthStencilClearFlags.Stencil, 0, 0);
            deviceContext.SetViewport(ref viewport);
            deviceContext.SetScissorRectangle(ref viewport);
            #region Render objects onto offscreen texture
            var hasMesh = false;
            for (var i = 0; i < context.RenderHost.PerFrameNodesWithPostEffect.Count; ++i)
            {
                var mesh = context.RenderHost.PerFrameNodesWithPostEffect[i];
                if (mesh.TryGetPostEffect(EffectName, out var effect))
                {
                    var color = Color;
                    if (effect is not null && effect.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out var attribute) && attribute is string colorStr)
                    {
                        color = colorStr.ToColor4();
                    }
                    if (modelStruct.Color != color)
                    {
                        modelStruct.Color = color;
                        modelCB.Upload(deviceContext, ref modelStruct);
                    }
                    context.CustomPassName = DefaultPassNames.EffectOutlineP1;
                    var pass = mesh?.EffectTechnique?[DefaultPassNames.EffectOutlineP1];
                    if (pass is null || pass.IsNULL)
                    {
                        continue;
                    }
                    pass.BindShader(deviceContext);
                    pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                    mesh?.RenderCustom(context, deviceContext);
                    hasMesh = true;
                }
            }
            #endregion
            if (hasMesh)
            {
                DrawOutline(context, deviceContext, depthStencilBuffer, renderTargetBuffer);
            }
        }
    }

    private void DrawOutline(RenderContext context, DeviceContextProxy deviceContext,
        ShaderResourceViewProxy? depthStencilBuffer, ShaderResourceViewProxy? source)
    {
        var buffer = context.RenderHost.RenderBuffer;

        if (buffer is null)
        {
            return;
        }

        if (buffer.FullResPPBuffer is null)
        {
            return;
        }

        var sourceViewport = new ViewportF(0, 0, buffer.FullResPPBuffer.Width, buffer.FullResPPBuffer.Height);
        deviceContext.SetViewport(ref sourceViewport);
        deviceContext.SetScissorRectangle(ref sourceViewport);
        #region Do Blur Pass
        if (useBlurCore)
        {
            if (blurCore is not null && source is not null)
            {
                for (var i = 0; i < numberOfBlurPass; ++i)
                {
                    blurCore.Run(context, deviceContext, source, ref sourceViewport, PostEffectBlurCore.BlurDepth.One, ref modelStruct);
                }
            }
        }
        else
        {
            if (blurPassHorizontal is not null && blurPassVertical is not null && context.RenderHost.RenderBuffer?.FullResPPBuffer is not null)
            {
                blurPassHorizontal.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
                for (var i = 0; i < numberOfBlurPass; ++i)
                {
                    deviceContext.SetRenderTarget(context.RenderHost.RenderBuffer.FullResPPBuffer.NextRTV);
                    blurPassHorizontal.PixelShader.BindTexture(deviceContext, textureSlot, source);
                    blurPassHorizontal.BindShader(deviceContext);
                    blurPassHorizontal.BindStates(deviceContext, StateType.All);
                    deviceContext.Draw(4, 0);

                    deviceContext.SetRenderTarget(source);
                    blurPassVertical.PixelShader.BindTexture(deviceContext, textureSlot, context.RenderHost.RenderBuffer.FullResPPBuffer.NextRTV);
                    blurPassVertical.BindShader(deviceContext);
                    blurPassVertical.BindStates(deviceContext, StateType.All);
                    deviceContext.Draw(4, 0);
                }
            }
        }

        #region Draw back with stencil test
        if (context.RenderHost.RenderBuffer?.FullResPPBuffer is not null && screenQuadPass is not null)
        {
            deviceContext.SetRenderTarget(depthStencilBuffer, context.RenderHost.RenderBuffer.FullResPPBuffer.NextRTV,
                true, Maths.Color.Transparent, false);
            screenQuadPass.PixelShader.BindTexture(deviceContext, textureSlot, source);
            screenQuadPass.BindShader(deviceContext);
            screenQuadPass.BindStates(deviceContext, StateType.All);
            deviceContext.Draw(4, 0);
        }
        #endregion

        #region Draw outline onto original target
        if (context.RenderHost.RenderBuffer?.FullResPPBuffer is not null && screenOutlinePass is not null)
        {
            deviceContext.SetRenderTarget(buffer.FullResPPBuffer.CurrentRTV);
            screenOutlinePass.PixelShader.BindTexture(deviceContext, textureSlot, context.RenderHost.RenderBuffer.FullResPPBuffer.NextRTV);
            screenOutlinePass.BindShader(deviceContext);
            screenOutlinePass.BindStates(deviceContext, StateType.All);
            deviceContext.Draw(4, 0);
            screenOutlinePass.PixelShader.BindTexture(deviceContext, textureSlot, null);
        }
        #endregion

        #endregion
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref blurCore);
        RemoveAndDispose(ref sampler);
    }

    private void OnUpdatePerModelStruct(RenderContext context)
    {
        modelStruct.Param.M11 = scaleX;
        modelStruct.Param.M12 = ScaleY;
        modelStruct.Color = new Color4();
        modelStruct.ViewportScale = (int)TextureSize;
    }
}
