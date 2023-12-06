namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// Default Pixel Shaders
/// </summary>
public static class DefaultPSShaderDescriptions
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshBlinnPhong = new(nameof(PSMeshBlinnPhong), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshBinnPhong);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshBlinnPhongOIT = new(nameof(PSMeshBlinnPhongOIT), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshBinnPhongOIT);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshBlinnPhongOITQuad = new(nameof(PSMeshBlinnPhongOITQuad), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshBinnPhongOITQuad);

    public static readonly ShaderDescription PSMeshOITDPInit = new(nameof(PSMeshOITDPInit), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshOITDPFirst);

    public static readonly ShaderDescription PSMeshBlinnPhongOITDP = new(nameof(PSMeshBlinnPhongOITDP), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshBlinnPhongOITDP);

    public static readonly ShaderDescription PSMeshOITDPBlending = new(nameof(PSMeshOITDPBlending), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshOITDPBlending);

    public static readonly ShaderDescription PSMeshOITDPFinal = new(nameof(PSMeshOITDPFinal), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshOITDPFinal);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshVertColor = new(nameof(PSMeshVertColor), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshVertColor);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshVertNormal = new(nameof(PSMeshVertNormal), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshNormal);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshVertPosition = new(nameof(PSMeshVertPosition), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshVertPosition);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshDiffuseMap = new(nameof(PSMeshDiffuseMap), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshDiffuseMap);
    /// <summary>
    /// The ps mesh diffuse map oit
    /// </summary>
    public static readonly ShaderDescription PSMeshDiffuseMapOIT = new(nameof(PSMeshDiffuseMapOIT), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshDiffuseMapOIT);

    public static readonly ShaderDescription PSMeshDiffuseMapOITDP = new(nameof(PSMeshDiffuseMapOITDP), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshDiffuseMapOITDP);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshColorStripe = new(nameof(PSMeshColorStripe), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshColorStripe);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshViewCube = new(nameof(PSMeshViewCube), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshViewCube);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSPoint = new(nameof(PSPoint), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSPoint);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSLine = new(nameof(PSLine), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSLine);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSLineColor = new(nameof(PSLineColor), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSLineColor);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSBillboardText = new(nameof(PSBillboardText), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSBillboardText);
    /// <summary>
    /// The ps billboard text oit
    /// </summary>
    public static readonly ShaderDescription PSBillboardTextOIT = new(nameof(PSBillboardTextOIT), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSBillboardTextOIT);

    public static readonly ShaderDescription PSBillboardTextOITDP = new(nameof(PSBillboardTextOITDP), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSBillboardTextOITDP);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshXRay = new(nameof(PSMeshXRay), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshXRay);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSShadow = new(nameof(PSShadow), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSShadow);
    #region Mesh Clipping
    /// <summary>
    /// /
    /// </summary>
    public static readonly ShaderDescription PSMeshClipBackface = new(nameof(PSMeshClipBackface), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshClipPlaneBackface);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshClipScreenQuad = new(nameof(PSMeshClipScreenQuad), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshClipPlaneQuad);
    #endregion
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSParticle = new(nameof(PSParticle), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSParticle);

    /// <summary>
    /// The ps particle oit
    /// </summary>
    public static readonly ShaderDescription PSParticleOIT = new(nameof(PSParticleOIT), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSParticleOIT);

    public static readonly ShaderDescription PSParticleOITDP = new(nameof(PSParticleOITDP), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSParticleOITDP);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSSkybox = new(nameof(PSSkybox), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSSkybox);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshWireframe = new(nameof(PSMeshWireframe), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshWireframe);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription PSMeshWireframeOIT = new(nameof(PSMeshWireframeOIT), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshWireframeOIT);

    public static readonly ShaderDescription PSMeshWireframeOITDP = new(nameof(PSMeshWireframeOITDP), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshWireframeOITDP);
    /// <summary>
    /// The ps depth stencil only
    /// </summary>
    public static readonly ShaderDescription PSDepthStencilOnly = new(nameof(PSDepthStencilOnly), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSDepthStencilTestOnly);

    /// <summary>
    ///
    /// </summary>
    public static readonly ShaderDescription PSMeshOutlineScreenQuad = new(nameof(PSMeshOutlineScreenQuad), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectOutlineScreenQuad);

    /// <summary>
    ///
    /// </summary>
    public static readonly ShaderDescription PSEffectFullScreenBlurVertical = new(nameof(PSEffectFullScreenBlurVertical), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectFullScreenBlurVertical);
    /// <summary>
    ///
    /// </summary>
    public static readonly ShaderDescription PSEffectFullScreenBlurHorizontal = new(nameof(PSEffectFullScreenBlurHorizontal), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectFullScreenBlurHorizontal);
    /// <summary>
    ///
    /// </summary>
    public static readonly ShaderDescription PSEffectMeshBorderHighlight = new(nameof(PSEffectMeshBorderHighlight), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectMeshBorderHighlight);
    /// <summary>
    /// The ps effect mesh border highlight
    /// </summary>
    public static readonly ShaderDescription PSEffectOutlineSmooth = new(nameof(PSEffectOutlineSmooth), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectOutlineSmooth);
    /// <summary>
    ///
    /// </summary>
    public static readonly ShaderDescription PSMeshOutlineQuadStencil = new(nameof(PSMeshOutlineQuadStencil), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectOutlineScreenQuadStencil);

    /// <summary>
    ///
    /// </summary>
    public static readonly ShaderDescription PSMeshOutlineQuadFinal = new(nameof(PSMeshOutlineQuadFinal), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectOutlineQuadFinal);

    /// <summary>
    ///
    /// </summary>
    public static readonly ShaderDescription PSEffectMeshXRay = new(nameof(PSEffectMeshXRay), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectMeshXRay);

    /// <summary>
    ///
    /// </summary>
    public static readonly ShaderDescription PSEffectBloomExtract = new(nameof(PSEffectBloomExtract), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectBloomExtract);
    /// <summary>
    /// The ps effect bloom vertical blur
    /// </summary>
    public static readonly ShaderDescription PSEffectBloomVerticalBlur = new(nameof(PSEffectBloomVerticalBlur), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectBloomVerticalBlur);
    /// <summary>
    /// The ps effect bloom horizontal blur
    /// </summary>
    public static readonly ShaderDescription PSEffectBloomHorizontalBlur = new(nameof(PSEffectBloomHorizontalBlur), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectBloomHorizontalBlur);

    /// <summary>
    /// The ps effect bloom combine
    /// </summary>
    public static readonly ShaderDescription PSEffectBloomCombine = new(nameof(PSEffectBloomCombine), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectBloomCombine);

    /// <summary>
    /// The ps effect FXAA
    /// </summary>
    public static readonly ShaderDescription PSEffectFXAA = new(nameof(PSEffectFXAA), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectFXAA);
    /// <summary>
    /// The ps effect luma
    /// </summary>
    public static readonly ShaderDescription PSEffectLUMA = new(nameof(PSEffectLUMA), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectLUMA);
    /// <summary>
    /// The ps effect x ray grid, this is based on BlinnPhong
    /// </summary>
    public static readonly ShaderDescription PSEffectXRayGrid = new(nameof(PSEffectXRayGrid), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectXRayGrid);
    /// <summary>
    /// The ps effect x ray grid, this is based on diffuse shading
    /// </summary>
    public static readonly ShaderDescription PSEffectDiffuseXRayGrid = new(nameof(PSEffectDiffuseXRayGrid), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSEffectDiffuseXRayGrid);
    /// <summary>
    /// The ps plane grid
    /// </summary>
    public static readonly ShaderDescription PSPlaneGrid = new(nameof(PSPlaneGrid), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSPlaneGrid);

    /// <summary>
    /// The ps mesh PBR
    /// </summary>
    public static readonly ShaderDescription PSMeshPBR = new(nameof(PSMeshPBR), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshPBR);

    /// <summary>
    /// The ps mesh PBR
    /// </summary>
    public static readonly ShaderDescription PSMeshPBROIT = new(nameof(PSMeshPBROIT), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshPBROIT);
    public static readonly ShaderDescription PSMeshPBROITDP = new(nameof(PSMeshPBROITDP), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSMeshPBROITDP);
    /// <summary>
    /// The ps sprite
    /// </summary>
    public static readonly ShaderDescription PSSprite2D = new(nameof(PSSprite2D), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSSprite2D);

    /// <summary>
    /// The ps screen dup
    /// </summary>
    public static readonly ShaderDescription PSScreenDup = new(nameof(PSScreenDup), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSScreenDup);

    /// <summary>
    /// The ps volume3d
    /// </summary>
    public static readonly ShaderDescription PSVolume3D = new(nameof(PSVolume3D), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSVolume3D);

    /// <summary>
    /// The ps volume cube
    /// </summary>
    public static readonly ShaderDescription PSVolumeCube = new(nameof(PSVolumeCube), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSVolumeCube);

    /// <summary>
    /// The ps volume3d
    /// </summary>
    public static readonly ShaderDescription PSVolumeDiffuse3D = new(nameof(PSVolumeDiffuse3D), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSVolumeDiffuse);
    /// <summary>
    /// The psssao p1
    /// </summary>
    public static readonly ShaderDescription PSSSAOP1 = new(nameof(PSSSAOP1), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSSSAOP1);
    /// <summary>
    /// The psssao
    /// </summary>
    public static readonly ShaderDescription PSSSAO = new(nameof(PSSSAO), ShaderStage.Pixel, new ShaderReflector(),
       DefaultPSShaderByteCodes.PSSSAO);

    /// <summary>
    /// The ps ssao blur
    /// </summary>
    public static readonly ShaderDescription PSSSAOBlur = new(nameof(PSSSAOBlur), ShaderStage.Pixel, new ShaderReflector(),
        DefaultPSShaderByteCodes.PSSSAOBlur);
}
