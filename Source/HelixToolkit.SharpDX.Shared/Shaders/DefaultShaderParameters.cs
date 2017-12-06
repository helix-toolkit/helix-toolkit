using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public static class DefaultVSShaderByteCodes
    {
        public static byte[] VSMeshDefault
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsMeshDefault;
#else
                throw new NotImplementedException();
#endif
            }
        }
    }

    public static class DefaultPSShaderByteCodes
    {
        public static byte[] PSMeshBinnPhong
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshBlinnPhong;
#else
                throw new NotImplementedException();
#endif

            }
        }
    }

    public static class DefaultInputLayout
    {
        public static InputElement[] VSInput { get; } = new InputElement[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
            new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
            new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),  
           
            //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
            new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1)
        };
    }

    public static class DefaultVSShaderDescriptions
    {
        public static ShaderDescription VSMeshDefault = new ShaderDescription("VSMeshDefault", ShaderStage.Vertex, FeatureLevel.Level_11_0, 
            DefaultVSShaderByteCodes.VSMeshDefault, 
            new ConstantBufferDescription[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB,
                DefaultConstantBufferDescriptions.ModelCB,
                DefaultConstantBufferDescriptions.LightCB,
                DefaultConstantBufferDescriptions.MaterialCB
            }, 
            null);
    }

    public static class DefaultPSShaderDescriptions
    {
        public static ShaderDescription PSMeshBlinnPhong = new ShaderDescription("PSBlinnPhong", ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshBinnPhong,
            new ConstantBufferDescription[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB,
                DefaultConstantBufferDescriptions.LightCB,
                DefaultConstantBufferDescriptions.MaterialCB
            },
            new TextureDescription[] 
            {
                DefaultTextureBufferDescriptions.DiffuseMapTB,
                DefaultTextureBufferDescriptions.AlphaMapTB,
                DefaultTextureBufferDescriptions.NormalMapTB,
                DefaultTextureBufferDescriptions.DisplacementMapTB,              
                DefaultTextureBufferDescriptions.ShadowMapTB
            });
    }

    public static class DefaultConstantBufferDescriptions
    {
        public static ConstantBufferDescription GlobalTransformCB = new ConstantBufferDescription("cbTransforms", GlobalTransformStruct.SizeInBytes, typeof(GlobalTransformStruct), 0);
        public static ConstantBufferDescription ModelCB = new ConstantBufferDescription("cbModel", ModelStruct.SizeInBytes, typeof(ModelStruct), 1);
        public static ConstantBufferDescription LightCB = new ConstantBufferDescription("cbLights", LightsStruct.SizeInBytes, typeof(LightsStruct), 2);
        public static ConstantBufferDescription MaterialCB = new ConstantBufferDescription("cbMaterial", MaterialStruct.SizeInBytes, typeof(MaterialStruct), 3);
    }

    public static class DefaultTextureBufferDescriptions
    {
        public static TextureDescription DiffuseMapTB = new TextureDescription(0, "texDiffuseMap", ShaderStage.Pixel);
        public static TextureDescription AlphaMapTB = new TextureDescription(1, "texAlphaMap", ShaderStage.Pixel);
        public static TextureDescription NormalMapTB = new TextureDescription(2, "texNormalMap", ShaderStage.Pixel);
        public static TextureDescription DisplacementMapTB = new TextureDescription(3, "texDisplacementMap", ShaderStage.Pixel);
        public static TextureDescription CubeMapTB = new TextureDescription(4, "texCubeMap", ShaderStage.Pixel);
        public static TextureDescription ShadowMapTB = new TextureDescription(5, "texShadowMap", ShaderStage.Pixel);
    }
}