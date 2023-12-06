namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public static class DefaultPSShaderByteCodes
{
    /// <summary>
    /// 
    /// </summary>
    public static string PSMeshBinnPhong
    {
        get;
    } = "psMeshBlinnPhong";
    /// <summary>
    /// Gets the ps mesh binn phong order independent transparent shader.
    /// </summary>
    /// <value>
    /// The ps mesh binn phong order independent transparent shader.
    /// </value>
    public static string PSMeshBinnPhongOIT
    {
        get;
    } = "psMeshBlinnPhongOIT";

    /// <summary>
    /// Gets the ps mesh binn phong oit quad.
    /// </summary>
    /// <value>
    /// The ps mesh binn phong oit quad.
    /// </value>
    public static string PSMeshBinnPhongOITQuad
    {
        get;
    } = "psMeshBlinnPhongOITQuad";

    public static string PSMeshOITDPFirst
    {
        get;
    } = "psMeshOITDPFirst";

    public static string PSMeshBlinnPhongOITDP
    {
        get;
    } = "psMeshBlinnPhongOITDP";

    public static string PSMeshOITDPBlending
    {
        get;
    } = "psMeshOITDPBlending";

    public static string PSMeshOITDPFinal
    {
        get;
    } = "psMeshOITDPFinal";
    /// <summary>
    /// Gets the ps mesh diffuse map oit.
    /// </summary>
    /// <value>
    /// The ps mesh diffuse map oit.
    /// </value>
    public static string PSMeshDiffuseMapOIT
    {
        get;
    } = "psMeshDiffuseMapOIT";

    public static string PSMeshDiffuseMapOITDP
    {
        get;
    } = "psMeshDiffuseMapOITDP";
    /// <summary>
    /// 
    /// </summary>
    public static string PSMeshVertColor
    {
        get;
    } = "psColor";
    /// <summary>
    /// 
    /// </summary>
    public static string PSMeshVertPosition
    {
        get;
    } = "psPositions";
    /// <summary>
    /// 
    /// </summary>
    public static string PSMeshNormal
    {
        get;
    } = "psNormals";

    public static string PSMeshDiffuseMap
    {
        get;
    } = "psDiffuseMap";
    public static string PSMeshColorStripe
    {
        get;
    } = "psMeshColorStripe";

    public static string PSMeshViewCube
    {
        get;
    } = "psViewCube";
    /// <summary>
    /// 
    /// </summary>
    public static string PSShadow
    {
        get;
    } = "psShadow";

    /// <summary>
    /// 
    /// </summary>
    public static string PSPoint
    {
        get;
    } = "psPoint";
    /// <summary>
    /// 
    /// </summary>
    public static string PSLine
    {
        get;
    } = "psLine";

    /// <summary>
    /// 
    /// </summary>
    public static string PSLineColor
    {
        get;
    } = "psLineColor";

    /// <summary>
    /// 
    /// </summary>
    public static string PSBillboardText
    {
        get;
    } = "psBillboardText";
    /// <summary>
    /// Gets the ps billboard text order independent transparent shader.
    /// </summary>
    /// <value>
    /// The ps billboard text order independent transparent shader.
    /// </value>
    public static string PSBillboardTextOIT
    {
        get;
    } = "psBillboardTextOIT";

    public static string PSBillboardTextOITDP
    {
        get;
    } = "psBillboardTextOITDP";
    /// <summary>
    /// 
    /// </summary>
    public static string PSMeshXRay
    {
        get;
    } = "psMeshXRay";

    /// <summary>
    /// 
    /// </summary>
    public static string PSMeshClipPlaneBackface
    {
        get;
    } = "psMeshClipPlaneBackface";
    /// <summary>
    /// 
    /// </summary>
    public static string PSMeshClipPlaneQuad
    {
        get;
    } = "psMeshClipPlaneQuad";
    /// <summary>
    /// 
    /// </summary>
    public static string PSParticle
    {
        get;
    } = "psParticle";
    /// <summary>
    /// Gets the ps particle order independent transparent shader.
    /// </summary>
    public static string PSParticleOIT
    {
        get;
    } = "psParticleOIT";

    public static string PSParticleOITDP
    {
        get;
    } = "psParticleOITDP";
    /// <summary>
    /// 
    /// </summary>
    public static string PSSkybox
    {
        get;
    } = "psSkybox";

    /// <summary>
    /// 
    /// </summary>
    public static string PSMeshWireframe
    {
        get;
    } = "psWireframe";
    /// <summary>
    /// Gets the ps mesh wireframe oit.
    /// </summary>
    /// <value>
    /// The ps mesh wireframe oit.
    /// </value>
    public static string PSMeshWireframeOIT
    {
        get;
    } = "psWireframeOIT";

    public static string PSMeshWireframeOITDP
    {
        get;
    } = "psWireframeOITDP";
    /// <summary>
    /// 
    /// </summary>
    public static string PSDepthStencilTestOnly
    {
        get;
    } = "psDepthStencilOnly";
    /// <summary>
    /// Gets the ps mesh outline screen quad.
    /// </summary>
    /// <value>
    /// The ps mesh outline screen quad.
    /// </value>
    public static string PSEffectOutlineScreenQuad
    {
        get;
    } = "psEffectOutlineQuad";
    /// <summary>
    /// Gets the ps effect full screen blur vertical.
    /// </summary>
    /// <value>
    /// The ps effect full screen blur vertical.
    /// </value>
    public static string PSEffectFullScreenBlurVertical
    {
        get;
    } = "psEffectGaussianBlurVertical";

    /// <summary>
    /// Gets the ps effect full screen blur horizontal.
    /// </summary>
    /// <value>
    /// The ps effect full screen blur horizontal.
    /// </value>
    public static string PSEffectFullScreenBlurHorizontal
    {
        get;
    } = "psEffectGaussianBlurHorizontal";

    /// <summary>
    /// Gets the ps mesh border highlight
    /// </summary>
    /// <value>
    /// The ps mesh mesh border highlight
    /// </value>
    public static string PSEffectMeshBorderHighlight
    {
        get;
    } = "psEffectMeshBorderHighlight";

    /// <summary>
    /// Gets the ps effect outline smooth.
    /// </summary>
    /// <value>
    /// The ps effect outline smooth.
    /// </value>
    public static string PSEffectOutlineSmooth
    {
        get;
    } = "psEffectOutlineSmooth";
    /// <summary>
    /// Gets the ps mesh outline screen quad stencil.
    /// </summary>
    /// <value>
    /// The ps mesh outline screen quad stencil.
    /// </value>
    public static string PSEffectOutlineScreenQuadStencil
    {
        get;
    } = "psEffectOutlineQuadStencil";
    /// <summary>
    /// Gets the ps mesh outline quad final.
    /// </summary>
    /// <value>
    /// The ps mesh outline quad final.
    /// </value>
    public static string PSEffectOutlineQuadFinal
    {
        get;
    } = "psEffectOutlineQualFinal";
    /// <summary>
    /// 
    /// </summary>
    /// <value>
    /// 
    /// </value>
    public static string PSEffectMeshXRay
    {
        get;
    } = "psEffectMeshXRay";
    /// <summary>
    /// Gets the ps effect bloom extract.
    /// </summary>
    /// <value>
    /// The ps effect bloom extract.
    /// </value>
    public static string PSEffectBloomExtract
    {
        get;
    } = "psEffectBloomExtract";
    /// <summary>
    /// Gets the ps effect bloom vertical blur.
    /// </summary>
    /// <value>
    /// The ps effect bloom vertical blur.
    /// </value>
    public static string PSEffectBloomVerticalBlur
    {
        get;
    } = "psEffectBloomBlurVertical";
    /// <summary>
    /// Gets the ps effect bloom horizontal blur.
    /// </summary>
    /// <value>
    /// The ps effect bloom horizontal blur.
    /// </value>
    public static string PSEffectBloomHorizontalBlur
    {
        get;
    } = "psEffectBloomBlurHorizontal";
    /// <summary>
    /// Gets the ps effect bloom combine.
    /// </summary>
    /// <value>
    /// The ps effect bloom combine.
    /// </value>
    public static string PSEffectBloomCombine
    {
        get;
    } = "psEffectBloomCombine";
    /// <summary>
    /// Gets the ps effect fxaa.
    /// </summary>
    /// <value>
    /// The ps effect fxaa.
    /// </value>
    public static string PSEffectFXAA
    {
        get;
    } = "psFXAA";
    /// <summary>
    /// Gets the ps effect luma.
    /// </summary>
    /// <value>
    /// The ps effect luma.
    /// </value>
    public static string PSEffectLUMA
    {
        get;
    } = "psLuma";
    /// <summary>
    /// Gets the ps effect x ray grid. This is based on BlinnPhong
    /// </summary>
    /// <value>
    /// The ps effect x ray grid.
    /// </value>
    public static string PSEffectXRayGrid
    {
        get;
    } = "psEffectMeshXRayGrid";
    /// <summary>
    /// Gets the ps effect diffuse x ray grid.
    /// </summary>
    /// <value>
    /// The ps effect diffuse x ray grid.
    /// </value>
    public static string PSEffectDiffuseXRayGrid
    {
        get;
    } = "psEffectMeshDiffuseXRayGrid";

    /// <summary>
    /// Gets the ps plane grid.
    /// </summary>
    /// <value>
    /// The ps plane grid.
    /// </value>
    public static string PSPlaneGrid
    {
        get;
    } = "psPlaneGrid";

    /// <summary>
    /// Gets the ps mesh PBR.
    /// </summary>
    /// <value>
    /// The ps mesh PBR.
    /// </value>
    public static string PSMeshPBR
    {
        get;
    } = "psMeshPBR";

    /// <summary>
    /// Gets the ps mesh PBR OIT.
    /// </summary>
    /// <value>
    /// The ps mesh PBR.
    /// </value>
    public static string PSMeshPBROIT
    {
        get;
    } = "psMeshPBROIT";
    public static string PSMeshPBROITDP
    {
        get;
    } = "psMeshPBROITDP";
    /// <summary>
    /// Gets the ps sprite.
    /// </summary>
    /// <value>
    /// The ps sprite.
    /// </value>
    public static string PSSprite2D
    {
        get;
    } = "psSprite";


    /// <summary>
    /// 
    /// </summary>
    public static string PSScreenDup
    {
        get;
    } = "psScreenDup";

    public static string PSVolume3D
    {
        get;
    } = "psVolume";

    public static string PSVolumeCube
    {
        get;
    } = "psVolumeCube";

    public static string PSVolumeDiffuse
    {
        get;
    } = "psVolumeDiffuse";

    public static string PSSSAOP1
    {
        get;
    } = "psSSAOP1";

    public static string PSSSAO
    {
        get;
    } = "psSSAO";

    public static string PSSSAOBlur
    {
        get;
    } = "psSSAOBlur";
}
