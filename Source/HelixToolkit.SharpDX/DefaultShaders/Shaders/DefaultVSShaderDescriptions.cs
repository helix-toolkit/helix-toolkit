namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public static class DefaultVSShaderDescriptions
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSMeshDefault = new(nameof(VSMeshDefault), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshDefault);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSMeshBatched = new(nameof(VSMeshBatched), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshBatched);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSMeshTessellation = new(nameof(VSMeshTessellation), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshTessellation);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSMeshShadow = new(nameof(VSMeshShadow), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshShadow);
    /// <summary>
    /// The vs mesh ssao
    /// </summary>
    public static readonly ShaderDescription VSMeshSSAO = new(nameof(VSMeshSSAO), ShaderStage.Vertex,
        new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshSSAO);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSMeshBatchedShadow = new(nameof(VSMeshBatchedShadow), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshBatchedShadow);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSMeshBatchedSSAO = new(nameof(VSMeshBatchedSSAO), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshBatchedSSAO);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSMeshInstancing = new(nameof(VSMeshInstancing), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshInstancing);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSMeshInstancingTessellation = new(nameof(VSMeshInstancingTessellation), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshInstancingTessellation);

    /// <summary>
    /// The vs mesh bone skinned basic
    /// </summary>
    public static readonly ShaderDescription VSMeshBoneSkinnedBasic = new(nameof(VSMeshBoneSkinnedBasic), ShaderStage.Vertex,
        new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshBoneSkinningBasic);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSPoint = new(nameof(VSPoint), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSPoint);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSPointShadow = new(nameof(VSPointShadow), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSPointShadow);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSBillboardText = new(nameof(VSBillboardText), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSBillboard);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSBillboardInstancing = new(nameof(VSBillboardInstancing), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSBillboardInstancing);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSMeshClipPlane = new(nameof(VSMeshClipPlane), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshClipPlane);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSFullScreenQuad = new(nameof(VSFullScreenQuad), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSMeshClipPlaneQuad);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSParticle = new(nameof(VSParticle), ShaderStage.Vertex,
        new ShaderReflector(),
        DefaultVSShaderByteCodes.VSParticle);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription VSSkybox = new(nameof(VSSkybox), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSSkybox);
    /// <summary>
    /// The vs mesh wireframe
    /// </summary>
    public static readonly ShaderDescription VSMeshWireframe = new(nameof(VSMeshWireframe), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshWireframe);
    /// <summary>
    /// The vs mesh depth
    /// </summary>
    public static readonly ShaderDescription VSMeshDepth = new(nameof(VSMeshDepth), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshDepth);
    /// <summary>
    /// The vs mesh batched wireframe
    /// </summary>
    public static readonly ShaderDescription VSMeshBatchedWireframe = new(nameof(VSMeshBatchedWireframe), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshBatchedWireframe);
    /// <summary>
    /// The vs bone skinning wireframe
    /// </summary>
    public static readonly ShaderDescription VSBoneSkinningWireframe = new(nameof(VSBoneSkinningWireframe), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshBoneSkinningWireframe);

    /// <summary>
    /// The vs mesh outline pass1
    /// </summary>
    public static readonly ShaderDescription VSMeshOutlinePass1 = new(nameof(VSMeshOutlinePass1), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshOutlineP1);

    /// <summary>
    /// The vs mesh outline pass1
    /// </summary>
    public static readonly ShaderDescription VSMeshOutlineScreenQuad = new(nameof(VSMeshOutlineScreenQuad), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshOutlineScreenQuad);
    /// <summary>
    /// The vs plane grid
    /// </summary>
    public static readonly ShaderDescription VSPlaneGrid = new(nameof(VSPlaneGrid), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSPlaneGrid);
    /// <summary>
    /// The vs screen quad
    /// </summary>
    public static readonly ShaderDescription VSScreenQuad = new(nameof(VSScreenQuad), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSScreenQuad);
    /// <summary>
    /// The vs sprite
    /// </summary>
    public static readonly ShaderDescription VSSprite2D = new(nameof(VSSprite2D), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSSprite2D);

    /// <summary>
    /// The vs volume3d
    /// </summary>
    public static readonly ShaderDescription VSVolume3D = new(nameof(VSVolume3D), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSVolume3D);
    /// <summary>
    /// The vsssao
    /// </summary>
    public static readonly ShaderDescription VSSSAO = new(nameof(VSSSAO), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSSSAO);

    /// <summary>
    /// The vs screen dup
    /// </summary>
    public static readonly ShaderDescription VSScreenDup = new(nameof(VSScreenDup), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSScreenDup);

    /// <summary>
    /// The vs screen dup mouse cursor
    /// </summary>
    public static readonly ShaderDescription VSScreenDupCursor = new(nameof(VSScreenDupCursor), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSScreenDupCursor);
}
