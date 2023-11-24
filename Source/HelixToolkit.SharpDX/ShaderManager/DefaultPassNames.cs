namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public struct DefaultPassNames
{
    /// <summary>
    /// 
    /// </summary>
    public const string Default = "Default";
    /// <summary>
    /// The Physics Based Rendering
    /// </summary>
    public const string PBR = "PhysicsBasedRendering";
    /// <summary>
    /// The PBR face normal pass
    /// </summary>
    public const string PBRFaceNormal = "PhysicsBasedRenderingFaceNormal";
    /// <summary>
    /// 
    /// </summary>
    public const string Diffuse = "RenderDiffuse";
    /// <summary>
    /// The diffuse oit
    /// </summary>
    public const string DiffuseOIT = "RenderDiffuseOIT";
    public const string DiffuseOITDP = "RenderDiffuseOITDepthPeeling";
    /// <summary>
    /// 
    /// </summary>
    public const string Colors = "RenderColors";
    /// <summary>
    /// 
    /// </summary>
    public const string Positions = "RenderPositions";
    /// <summary>
    /// 
    /// </summary>
    public const string Normals = "RenderNormals";
    /// <summary>
    /// 
    /// </summary>
    public const string NormalVector = "RenderNormalVector";
    /// <summary>
    /// 
    /// </summary>
    public const string PerturbedNormals = "RenderPerturbedNormals";
    /// <summary>
    /// 
    /// </summary>
    public const string Tangents = "RenderTangents";
    /// <summary>
    /// 
    /// </summary>
    public const string TexCoords = "RenderTexCoords";
    /// <summary>
    /// 
    /// </summary>
    public const string ViewCube = "RenderViewCube";
    /// <summary>
    ///
    /// </summary>
    public const string ColorStripe1D = "ColorStripe1D";
    /// <summary>
    /// The mesh transparent
    /// </summary>
    public const string OITPass = "MeshOITPass";

    #region Deep peeling
    public const string OITDepthPeelingInit = "OITDepthPeelingFirst";

    public const string OITDepthPeeling = "OITDepthPeeling";

    public const string OITDepthPeelingBlending = "OITDepthPeelingBlending";

    public const string OITDepthPeelingFinal = "OITDepthPeelingFinal";
    #endregion
    /// <summary>
    /// The oit pass PBR
    /// </summary>
    public const string PBROITPass = "MeshPhysicsBasedOITPass";
    public const string PBROITDPPass = "MeshPhysicsBasedOITDepthPeelingPass";
    /// <summary>
    /// 
    /// </summary>
    public const string WireframeOITPass = "WireframeOIT";
    public const string WireframeOITDPPass = "WireframeOITDP";
    /// <summary>
    /// 
    /// </summary>
    public const string MeshTriTessellation = "MeshTriTessellation";

    public const string MeshTriTessellationOIT = "MeshTriTessellationOIT";

    public const string MeshTriTessellationOITDP = "MeshTriTessellationOITDP";
    /// <summary>
    /// 
    /// </summary>
    public const string MeshPBRTriTessellation = "MeshPBRTriTessellation";

    public const string MeshPBRTriTessellationOIT = "MeshPBRTriTessellationOIT";

    public const string MeshPBRTriTessellationOITDP = "MeshPBRTriTessellationOITDP";
    /// <summary>
    /// 
    /// </summary>
    public const string MeshQuadTessellation = "MeshQuadTessellation";
    /// <summary>
    /// 
    /// </summary>
    public const string MeshOutline = "RenderMeshOutline";
    /// <summary>
    /// The mesh bone skinned
    /// </summary>
    public const string PreComputeMeshBoneSkinned = "MeshBoneSkinned";

    /// <summary>
    /// 
    /// </summary>
    public const string EffectBlurVertical = "EffectBlurVertical";

    /// <summary>
    /// 
    /// </summary>
    public const string EffectBlurHorizontal = "EffectBlurHorizontal";
    /// <summary>
    /// 
    /// </summary>
    public const string EffectOutlineSmooth = "EffectOutlineSmooth";

    /// <summary>
    /// 
    /// </summary>
    public const string EffectOutlineP1 = "RenderMeshOutlineP1";

    /// <summary>
    /// 
    /// </summary>
    public const string EffectMeshXRayP1 = "EffectMeshXRayP1";
    /// <summary>
    /// 
    /// </summary>
    public const string EffectMeshXRayP2 = "EffectMeshXRayP2";
    /// <summary>
    /// 
    /// </summary>
    public const string EffectMeshXRayGridP1 = "EffectMeshXRayGridP1";
    /// <summary>
    /// 
    /// </summary>
    public const string EffectMeshXRayGridP2 = "EffectMeshXRayGridP2";
    /// <summary>
    /// 
    /// </summary>
    public const string EffectMeshXRayGridP3 = "EffectMeshXRayGridP3";
    /// <summary>
    /// The effect mesh diffuse x ray grid p3
    /// </summary>
    public const string EffectMeshDiffuseXRayGridP3 = "EffectMeshDiffueXRayGridP3";
    /// <summary>
    /// 
    /// </summary>
    public const string ShadowPass = "RenderShadow";
    /// <summary>
    /// 
    /// </summary>
    public const string Backface = "RenderBackface";
    /// <summary>
    /// 
    /// </summary>
    public const string ScreenQuad = "ScreenQuad";
    /// <summary>
    /// 
    /// </summary>
    public const string ScreenQuadCopy = "ScreenQuadCopy";
    /// <summary>
    /// The wireframe
    /// </summary>
    public const string Wireframe = "Wireframe";
    /// <summary>
    /// The depth prepass
    /// </summary>
    public const string DepthPrepass = "DepthPrepass";
    /// <summary>
    /// Calculate luma and set luma as Alpha channel before FXAA pass.
    /// </summary>
    public const string LumaPass = "LumaPass";
    /// <summary>
    /// 
    /// </summary>
    public const string FXAAPass = "FXAAPass";
    /// <summary>
    /// The ssao pass
    /// </summary>
    public const string MeshSSAOPass = "MeshSSAOPass";
}
