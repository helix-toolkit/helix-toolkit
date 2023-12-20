using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public class SkyDomeRenderCore : GeometryRenderCore, ISkyboxRenderParams
{
    #region Default Mesh
    private static readonly MeshGeometry3D SphereMesh;

    static SkyDomeRenderCore()
    {
        var builder = new MeshBuilder(false, false);
        builder.AddSphere(Vector3.Zero, 1);
        SphereMesh = builder.ToMeshGeometry3D();
    }
    #endregion

    #region Variables
    private ShaderResourceViewProxy? cubeTextureRes;
    private int cubeTextureSlot;
    private SamplerStateProxy? textureSampler;
    private int textureSamplerSlot;
    private ShaderPass? DefaultShaderPass;
    private SkyDomeBufferModel? skyBuffer;
    #endregion

    #region Properties
    private TextureModel? cubeTexture = null;
    /// <summary>
    /// Gets or sets the cube texture.
    /// </summary>
    /// <value>
    /// The cube texture.
    /// </value>
    public TextureModel? CubeTexture
    {
        set
        {
            if (SetAffectsRender(ref cubeTexture, value) && IsAttached)
            {
                UpdateTexture();
            }
        }
        get
        {
            return cubeTexture;
        }
    }
    /// <summary>
    /// Gets the mip map levels for current cube texture.
    /// </summary>
    /// <value>
    /// The mip map levels.
    /// </value>
    public int MipMapLevels { private set; get; } = 0;

    private SamplerStateDescription samplerDescription = DefaultSamplers.EnvironmentSampler;
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
                var newSampler = EffectTechnique?.EffectsManager?.StateManager?.Register(value);
                RemoveAndDispose(ref textureSampler);
                textureSampler = newSampler;
            }
        }
        get
        {
            return samplerDescription;
        }
    }
    /// <summary>
    /// Gets or sets the name of the shader cube texture.
    /// </summary>
    /// <value>
    /// The name of the shader cube texture.
    /// </value>
    public string ShaderCubeTextureName { set; get; } = DefaultBufferNames.CubeMapTB;
    /// <summary>
    /// Gets or sets the name of the shader cube texture sampler.
    /// </summary>
    /// <value>
    /// The name of the shader cube texture sampler.
    /// </value>
    public string ShaderCubeTextureSamplerName { set; get; } = DefaultSamplerStateNames.CubeMapSampler;
    /// <summary>
    /// Skip environment map rendering, but still keep it available for other object to use.
    /// </summary>
    public bool SkipRendering { set; get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SkyBoxRenderCore"/> class.
    /// </summary>
    public SkyDomeRenderCore()
    {
        RasterDescription = DefaultRasterDescriptions.RSSkyDome;
    }
    /// <summary>
    /// Called when [attach].
    /// </summary>
    /// <param name="technique">The technique.</param>
    /// <returns></returns>
    protected override bool OnAttach(IRenderTechnique? technique)
    {
        if (base.OnAttach(technique))
        {
            DefaultShaderPass = technique?[DefaultPassNames.Default];
            if (DefaultShaderPass is not null)
            {
                OnDefaultPassChanged(DefaultShaderPass);
            }
            skyBuffer = new SkyDomeBufferModel
            {
                Geometry = SphereMesh
            };
            GeometryBuffer = skyBuffer;
            UpdateTexture();
            textureSampler = technique?.EffectsManager?.StateManager?.Register(SamplerDescription);
            return true;
        }
        else
        {
            return false;
        }
    }
    private void UpdateTexture()
    {
        MipMapLevels = 0;
        RemoveAndDispose(ref cubeTextureRes);
        if (CubeTexture != null && Device is not null)
        {
            cubeTextureRes = new ShaderResourceViewProxy(Device);
            cubeTextureRes.CreateView(cubeTexture);
            if (cubeTextureRes.TextureView != null && cubeTextureRes.TextureView.Description.Dimension == ShaderResourceViewDimension.TextureCube)
            {
                MipMapLevels = cubeTextureRes.TextureView.Description.TextureCube.MipLevels;
            }
        }
    }
    protected override void OnDetach()
    {
        MipMapLevels = 0;
        RemoveAndDispose(ref textureSampler);
        RemoveAndDispose(ref cubeTextureRes);
        GeometryBuffer = null;
        RemoveAndDispose(ref skyBuffer);
        base.OnDetach();
    }

    /// <summary>
    /// Called when [default pass changed].
    /// </summary>
    /// <param name="pass">The pass.</param>
    protected void OnDefaultPassChanged(ShaderPass pass)
    {
        cubeTextureSlot = pass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderCubeTextureName);
        textureSamplerSlot = pass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderCubeTextureSamplerName);
    }
    /// <summary>
    /// Called when [render].
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="deviceContext">The device context.</param>
    protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
    {
        if (context.SharedResource is null || DefaultShaderPass is null || GeometryBuffer?.IndexBuffer is null)
        {
            return;
        }

        context.SharedResource.EnvironementMap = cubeTextureRes;
        context.SharedResource.EnvironmentMapMipLevels = MipMapLevels;
        if (SkipRendering)
        {
            return;
        }
        DefaultShaderPass.BindShader(deviceContext);
        DefaultShaderPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
        DefaultShaderPass.PixelShader.BindTexture(deviceContext, cubeTextureSlot, cubeTextureRes);
        DefaultShaderPass.PixelShader.BindSampler(deviceContext, textureSamplerSlot, textureSampler);
        deviceContext.DrawIndexed(GeometryBuffer.IndexBuffer.ElementCount, 0, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    private sealed class SkyDomeBufferModel : MeshGeometryBufferModel<Vector3>
    {
        public SkyDomeBufferModel() : base(NativeHelper.SizeOf<Vector3>())
        {
            Topology = PrimitiveTopology.TriangleList;
        }

        protected override void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D? geometry, IDeviceResources? deviceResources)
        {
            if (bufferIndex == 0 && geometry != null && geometry.Positions != null && geometry.Positions.Count > 0)
            {
                buffer.UploadDataToBuffer(context, geometry.Positions, geometry.Positions.Count);
            }
        }
    }

    protected sealed override void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext)
    {
    }

    protected sealed override void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
    {
    }

    protected sealed override void OnRenderDepth(RenderContext context, DeviceContextProxy deviceContext, Shaders.ShaderPass? customPass)
    {
    }
}
