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
        public static string ModelCB = "cbModel";
        public static string LightCB = "cbLights";
        public static string MaterialCB = "cbMaterial";
        public static string BoneCB = "cbBoneSkinning";
        public static string ClipParamsCB = "cbClipping";
        public static string TessellationParamsCB = "cbTessellation";

        //-----------Materials--------------------
        public static string DiffuseMapTB = "texDiffuseMap";
        public static string AlphaMapTB = "texAlphaMap";
        public static string NormalMapTB = "texNormalMap";
        public static string DisplacementMapTB = "texDisplacementMap";
        public static string CubeMapTB = "texCubeMap";
        public static string ShadowMapTB = "texShadowMap";
        public static string BillboardTB = "billboardTexture";
        //----------Particle--------------
        public static string ParticleFrameCB = "cbParticleFrame";
        public static string ParticleCreateParameters = "cbParticleCreateParameters";
        public static string ParticleMapTB = "texDiffuseMap";
        public static string CurrentSimulationStateUB = "CurrentSimulationState";
        public static string NewSimulationStateUB = "NewSimulationState";
        public static string SimulationStateTB = "SimulationState";
    }
}
