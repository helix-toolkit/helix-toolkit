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
    public static class DefaultConstantBufferDescriptions
    {
        public static ConstantBufferDescription GlobalTransformCB = new ConstantBufferDescription(nameof(GlobalTransformCB), GlobalTransformStruct.SizeInBytes);
        public static ConstantBufferDescription ModelCB = new ConstantBufferDescription(nameof(ModelCB), ModelStruct.SizeInBytes);
        public static ConstantBufferDescription LightCB = new ConstantBufferDescription(nameof(LightCB), Model.LightsBufferModel.SizeInBytes);
        public static ConstantBufferDescription MaterialCB = new ConstantBufferDescription(nameof(MaterialCB), MaterialStruct.SizeInBytes);
        public static ConstantBufferDescription BoneCB = new ConstantBufferDescription(nameof(BoneCB), BoneMatricesStruct.SizeInBytes);
        public static ConstantBufferDescription ClipParamsCB = new ConstantBufferDescription(nameof(ClipParamsCB), ClipPlaneStruct.SizeInBytes);
    }

    public static class DefaultTextureBufferDescriptions
    {
        public static TextureDescription DiffuseMapTB = new TextureDescription(nameof(DiffuseMapTB), ShaderStage.Pixel);
        public static TextureDescription AlphaMapTB = new TextureDescription(nameof(AlphaMapTB), ShaderStage.Pixel);
        public static TextureDescription NormalMapTB = new TextureDescription(nameof(NormalMapTB), ShaderStage.Pixel);
        public static TextureDescription DisplacementMapTB = new TextureDescription(nameof(DisplacementMapTB), ShaderStage.Pixel);
        public static TextureDescription CubeMapTB = new TextureDescription(nameof(CubeMapTB), ShaderStage.Pixel);
        public static TextureDescription ShadowMapTB = new TextureDescription(nameof(ShadowMapTB), ShaderStage.Pixel);
        public static TextureDescription BillboardTB = new TextureDescription(nameof(BillboardTB), ShaderStage.Pixel);
    }
}
