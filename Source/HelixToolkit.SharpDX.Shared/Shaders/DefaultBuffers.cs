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
    public static class DefaultBufferNames
    {
        public static string GlobalTransformCB = "cbTransforms";
        public static string ModelCB = "cbModel";
        public static string LightCB = "cbLights";
        public static string MaterialCB = "cbMaterial";
        public static string BoneCB = "BoneSkinning";
        public static string ClipParamsCB = "cbClipping";
        public static string DiffuseMapTB = "texDiffuseMap";
        public static string AlphaMapTB = "texAlphaMap";
        public static string NormalMapTB = "texNormalMap";
        public static string DisplacementMapTB = "texDisplacementMap";
        public static string CubeMapTB = "texCubeMap";
        public static string ShadowMapTB = "texShadowMap";
        public static string BillboardTB = "billboardTexture";

    }

    //public static class DefaultConstantBufferDescriptions
    //{
    //    public static ConstantBufferDescription GlobalTransformCB = new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes);
    //    public static ConstantBufferDescription ModelCB = new ConstantBufferDescription(DefaultBufferNames.ModelCB, ModelStruct.SizeInBytes);
    //    public static ConstantBufferDescription LightCB = new ConstantBufferDescription(DefaultBufferNames.LightCB, Model.LightsBufferModel.SizeInBytes);
    //    public static ConstantBufferDescription MaterialCB = new ConstantBufferDescription(DefaultBufferNames.MaterialCB, MaterialStruct.SizeInBytes);
    //    public static ConstantBufferDescription BoneCB = new ConstantBufferDescription(DefaultBufferNames.BoneCB, BoneMatricesStruct.SizeInBytes);
    //    public static ConstantBufferDescription ClipParamsCB = new ConstantBufferDescription(DefaultBufferNames.ClipParamsCB, ClipPlaneStruct.SizeInBytes);
    //}

    //public static class DefaultTextureBufferDescriptions
    //{
    //    public static TextureDescription DiffuseMapTB = new TextureDescription(nameof(DiffuseMapTB), ShaderStage.Pixel);
    //    public static TextureDescription AlphaMapTB = new TextureDescription(nameof(AlphaMapTB), ShaderStage.Pixel);
    //    public static TextureDescription NormalMapTB = new TextureDescription(nameof(NormalMapTB), ShaderStage.Pixel);
    //    public static TextureDescription DisplacementMapTB = new TextureDescription(nameof(DisplacementMapTB), ShaderStage.Pixel);
    //    public static TextureDescription CubeMapTB = new TextureDescription(nameof(CubeMapTB), ShaderStage.Pixel);
    //    public static TextureDescription ShadowMapTB = new TextureDescription(nameof(ShadowMapTB), ShaderStage.Pixel);
    //    public static TextureDescription BillboardTB = new TextureDescription(nameof(BillboardTB), ShaderStage.Pixel);
    //}
}
