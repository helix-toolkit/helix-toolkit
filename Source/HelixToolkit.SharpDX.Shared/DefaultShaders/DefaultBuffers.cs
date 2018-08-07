/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    /// <summary>
    /// Default buffer names from shader code. Name must match shader code to bind proper buffer
    /// <para>Note: Constant buffer must match both name and struct size</para>
    /// </summary>
    public static class DefaultBufferNames
    {
        public static string GlobalTransformCB = "cbTransforms";
        public static string ModelCB = "cbMeshModel";
        public static string MeshPhongCB = "cbMeshPhongMaterial";
        public static string PointLineModelCB = "cbPointLineModel";
        public static string PlaneGridModelCB = "cbPlaneGridModel";
        public static string LightCB = "cbLights";
        public static string ClipParamsCB = "cbClipping";
        public static string BorderEffectCB = "cbBorderEffect";
        public static string DynamicCubeMapCB = "cbDynamicCubeMap";
#if !NETFX_CORE
        public static string ScreenDuplicationCB = "cbScreenClone";
#endif
        //-----------Materials--------------------
        public static string DiffuseMapTB = "texDiffuseMap";
        public static string AlphaMapTB = "texAlphaMap";
        public static string NormalMapTB = "texNormalMap";
        public static string DisplacementMapTB = "texDisplacementMap";
        public static string CubeMapTB = "texCubeMap";
        public static string ShadowMapTB = "texShadowMap";
        public static string BillboardTB = "billboardTexture";
        public static string ColorStripe1DXTB = "texColorStripe1DX";
        public static string ColorStripe1DYTB = "texColorStripe1DY";
        //----------Particle--------------
        public static string ParticleFrameCB = "cbParticleFrame";
        public static string ParticleCreateParameters = "cbParticleCreateParameters";
        public static string ParticleMapTB = "texParticle";
        public static string CurrentSimulationStateUB = "CurrentSimulationState";
        public static string NewSimulationStateUB = "NewSimulationState";
        public static string SimulationStateTB = "SimulationState";
        //----------ShadowMap---------------
        public static string ShadowParamCB = "cbShadow";
        //----------Order Independent Transparent-----------
        public static string OITColorTB = "texOITColor";
        public static string OITAlphaTB = "texOITAlpha";
        //----------Bone Skin--------------
        public static string BoneSkinSB = "skinMatrices"; // Structured Buffer
    }

    public static class DefaultSamplerStateNames
    {
        public static string DiffuseMapSampler = "samplerDiffuse";
        public static string AlphaMapSampler = "samplerAlpha";
        public static string NormalMapSampler = "samplerNormal";
        public static string DisplacementMapSampler = "samplerDisplace";
        public static string CubeMapSampler = "samplerCube";
        public static string ShadowMapSampler = "samplerShadow";
        public static string ParticleTextureSampler = "samplerParticle";
        public static string BillboardTextureSampler = "samplerBillboard";
    }
}
