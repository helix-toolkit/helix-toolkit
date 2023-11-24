namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// Default buffer names from shader code. Name must match shader code to bind proper buffer
/// <para>Note: Constant buffer must match both name and struct size</para>
/// </summary>
public static class DefaultBufferNames
{
    public const string GlobalTransformCB = "cbTransforms";
    public const string ModelCB = "cbMesh";
    public const string SimpleMeshCB = "cbMeshSimple";
    public const string PointLineModelCB = "cbPointLineModel";
    public const string ParticleModelCB = "cbParticleModel";
    public const string PlaneGridModelCB = "cbPlaneGridModel";
    public const string LightCB = "cbLights";
    public const string ClipParamsCB = "cbClipping";
    public const string BorderEffectCB = "cbBorderEffect";
    public const string DynamicCubeMapCB = "cbDynamicCubeMap";
    public const string ScreenQuadCB = "cbScreenQuad";
    public const string VolumeModelCB = "cbVolumeModel";
    public const string SSAOCB = "cbSSAO";
    public const string ScreenDuplicationCB = "cbScreenClone";
    public const string MorphTargetCB = "cbMorphTarget";
    //-----------Materials--------------------
    public const string DiffuseMapTB = "texDiffuseMap";
    public const string AlphaMapTB = "texAlphaMap";
    public const string NormalMapTB = "texNormalMap";
    public const string DisplacementMapTB = "texDisplacementMap";
    public const string CubeMapTB = "texCubeMap";
    public const string ShadowMapTB = "texShadowMap";
    public const string SpecularTB = "texSpecularMap";
    public const string BillboardTB = "billboardTexture";
    public const string ColorStripe1DXTB = "texColorStripe1DX";
    public const string ColorStripe1DYTB = "texColorStripe1DY";
    public const string RMMapTB = "texRMMap";
    public const string AOMapTB = "texAOMap";
    public const string EmissiveTB = "texEmissiveMap";
    public const string IrradianceMap = "texIrradianceMap";
    //----------Particle--------------
    public const string ParticleFrameCB = "cbParticleFrame";
    public const string ParticleCreateParameters = "cbParticleCreateParameters";
    public const string ParticleMapTB = "texParticle";
    public const string CurrentSimulationStateUB = "CurrentSimulationState";
    public const string NewSimulationStateUB = "NewSimulationState";
    public const string SimulationStateTB = "SimulationState";
    //----------ShadowMap---------------
    public const string ShadowParamCB = "cbShadow";
    //----------Order Independent Transparent-----------
    public const string OITColorTB = "texOITColor";
    public const string OITAlphaTB = "texOITAlpha";
    public const string OITSortCB = "cbOITSortRender";
    //----------Bone Skin--------------
    public const string BoneSkinSB = "skinMatrices"; // Structured Buffer

    public const string MTWeightsB = "morphTargetWeights"; //Buffer<float>
    public const string MTDeltasB = "morphTargetDeltas"; //Buffer<float3>
    public const string MTOffsetsB = "morphTargetOffsets"; //Buffer<int>

    public const string SpriteTB = "texSprite";

    public const string VolumeTB = "texVolume";
    public const string VolumeFront = "texVolumeFront";
    public const string VolumeBack = "texVolumeBack";

    public const string SSAOMapTB = "texSSAOMap";
    public const string SSAONoiseTB = "texSSAONoise";
    public const string SSAODepthTB = "texSSAODepth";
}
