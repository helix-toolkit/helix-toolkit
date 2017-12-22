/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public static class ShaderVariableNames
    {
        //Common
        public static string WorldMatrix = "mWorld";
        public static string ViewMatrix = "mView";
        public static string ProjectionMatrix = "mProjection";
        public static string ViewPortMatrix = "vViewport";
        public static string ViewFrustum = "vFrustum";
        public static string CameraPosition = "vEyePos";

        //Material Variables
        public static string MaterialAmbientVariable = "vMaterialAmbient";
        public static string MaterialDiffuseVariable = "vMaterialDiffuse";
        public static string MaterialEmissiveVariable = "vMaterialEmissive";
        public static string MaterialSpecularVariable = "vMaterialSpecular";
        public static string MaterialReflectVariable = "vMaterialReflect";
        public static string MaterialShininessVariable = "sMaterialShininess";
        public static string HasDiffuseMapVariable = "bHasDiffuseMap";
        public static string HasDiffuseAlphaMapVariable = "bHasAlphaMap";
        public static string HasNormalMapVariable = "bHasNormalMap";
        public static string HasDisplacementMapVariable = "bHasDisplacementMap";
        public static string HasShadowMapVariable = "bHasShadowMap";
        public static string TextureDiffuseMapVariable = "texDiffuseMap";
        public static string TextureNormalMapVariable = "texNormalMap";
        public static string TextureDisplacementMapVariable = "texDisplacementMap";
        public static string TextureShadowMapVariable = "texShadowMap";
        public static string TextureDiffuseAlphaMapVariable = "texAlphaMap";

        //Mesh
        public static string InvertNormal = "bInvertNormal";
        public static string HasInstance = "bHasInstances";
        public static string HasInstanceParams = "bHasInstanceParams";
        public static string HasBones = "bHasBones";
        public static string SkinMatrics = "SkinMatrices";

        //Point
        public static string PointParams = "vPointParams";
        public static string PointColor = "vPointColor";

        //Line
        public static string LineParams = "vLineParams";
        public static string LineColor = "vLineColor";

        //Billboard
        public static string HasTextureVariable = "bHasTexture";
        public static string BillboardTextureVariable = "billboardTexture";
        public static string BillboardAlphaTextureVariable = "billboardAlphaTexture";
        public static string HasAlphaTextureVariable = "bHasAlphaTexture";
        public static string BillboardFixedSizeVariable = "bBillboardFixedSize";

        //CrossSection
        public static string CrossPlaneParams = "CrossPlaneParams";
        public static string EnableCrossPlane = "EnableCrossPlane";
        public static string CrossSectionColor = "CrossSectionColor";
        public static string SectionFillTexture = "SectionFillTexture";

        //Patch
        public static string TessellationFactorVariable = "vTessellation";

        //Particle System
        public static string CurrentSimulationStateVariable = "CurrentSimulationState";
        public static string NewSimulationStateVariable = "NewSimulationState";
        public static string SimulationStateVariable = "SimulationState";
        public static string ParticleSizeVariable = "ParticleSize";
        public static string RandomVectorVariable = "RandomVector";
        public static string RandomSeedVariable = "RandomSeed";
        public static string NumTextureColumnVariable = "NumTexCol";
        public static string NumTextureRowVariable = "NumTexRow";
        public static string AnimateByEnergyLevelVariable = "AnimateByEnergyLevel";
    }
}
