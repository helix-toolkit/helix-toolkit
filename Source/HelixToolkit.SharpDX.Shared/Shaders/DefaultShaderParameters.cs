using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Model;
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

        public static byte[] VSMeshInstancing
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsMeshInstancing;
#else
                throw new NotImplementedException();
#endif
            }
        }

        public static byte[] VSMeshBoneSkinning
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsBoneSkinning;
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

        public static byte[] PSMeshVertColor
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psColor;
#else
                throw new NotImplementedException();
#endif

            }
        }

        public static byte[] PSMeshVertPosition
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psPositions;
#else
                throw new NotImplementedException();
#endif

            }
        }

        public static byte[] PSMeshNormal
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psNormals;
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

        public static InputElement[] VSInputInstancing { get; } = new InputElement[]
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
            new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("COLOR", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
            new InputElement("COLOR", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
            new InputElement("COLOR", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 5, Format.R32G32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
        };

        public static InputElement[] VSInputBoneSkinning { get; } = new InputElement[]
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
            new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("BONEIDS", 0, Format.R32G32B32A32_SInt, InputElement.AppendAligned, 2),
            new InputElement("BONEWEIGHTS", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2),
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

        public static ShaderDescription VSMeshInstancing = new ShaderDescription("VSMeshInstancing", ShaderStage.Vertex, FeatureLevel.Level_11_0,
            DefaultVSShaderByteCodes.VSMeshInstancing,
            new ConstantBufferDescription[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB,
                DefaultConstantBufferDescriptions.ModelCB,
                DefaultConstantBufferDescriptions.LightCB,
                DefaultConstantBufferDescriptions.MaterialCB
            },
            null);

        public static ShaderDescription VSMeshBoneSkinning = new ShaderDescription("VSMeshBoneSkinning", ShaderStage.Vertex, FeatureLevel.Level_11_0,
            DefaultVSShaderByteCodes.VSMeshBoneSkinning,
            new ConstantBufferDescription[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB,
                DefaultConstantBufferDescriptions.ModelCB,
                DefaultConstantBufferDescriptions.LightCB,
                DefaultConstantBufferDescriptions.MaterialCB,
                DefaultConstantBufferDescriptions.BoneCB
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
                DefaultConstantBufferDescriptions.ModelCB,
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

        public static ShaderDescription PSMeshVertColor = new ShaderDescription("PSColor", ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshBinnPhong);
        public static ShaderDescription PSMeshVertNormal = new ShaderDescription("PSNormal", ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshNormal);
        public static ShaderDescription PSMeshVertPosition = new ShaderDescription("PSPosition", ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshVertPosition);
    }

    public static class DefaultBlendStateDescriptions
    {
        public readonly static BlendStateDescription BSNormal = new BlendStateDescription();
        public readonly static BlendStateDescription NoBlend = new BlendStateDescription();
        public readonly static BlendStateDescription BSXRayBlending = new BlendStateDescription();
        static DefaultBlendStateDescriptions()
        {
            BSNormal.RenderTarget[0]=new RenderTargetBlendDescription()
            {
                AlphaBlendOperation = BlendOperation.Add,
                BlendOperation = BlendOperation.Add,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationAlphaBlend = BlendOption.DestinationAlpha,
                SourceAlphaBlend = BlendOption.SourceAlpha,
                IsBlendEnabled = true,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };

            NoBlend.RenderTarget[0] = new RenderTargetBlendDescription() { IsBlendEnabled = false };
            BSXRayBlending.RenderTarget[0] = new RenderTargetBlendDescription()
            {
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };
        }
    }

    public static class DefaultDepthStencilDescriptions
    {
        public readonly static DepthStencilStateDescription DSSDepthLess = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.All,
            DepthComparison = Comparison.Less,
            IsStencilEnabled = false
        };

        public readonly static DepthStencilStateDescription DSSDepthLessEqual = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.All,
            DepthComparison = Comparison.LessEqual,
            IsStencilEnabled = false
        };

        public readonly static DepthStencilStateDescription DSSLessNoWrite = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.Zero,
            DepthComparison = Comparison.Less,
            IsStencilEnabled = false
        };
    }

    public static class DefaultConstantBufferDescriptions
    {
        public static string GlobalTransformCBName = "cbTransform";
        public static string ModelCBName = "cbModel";
        public static string LightsCBName = "cbLights";
        public static string MaterialCBName = "cbMaterial";
        public static string BoneCBName = "cbSkinMatrices";
        public static ConstantBufferDescription GlobalTransformCB = new ConstantBufferDescription(GlobalTransformCBName, GlobalTransformStruct.SizeInBytes, 0);
        public static ConstantBufferDescription ModelCB = new ConstantBufferDescription(ModelCBName, ModelStruct.SizeInBytes, 1);
        public static ConstantBufferDescription LightCB = new ConstantBufferDescription(LightsCBName, LightsBufferModel.SizeInBytes, 2);
        public static ConstantBufferDescription MaterialCB = new ConstantBufferDescription(MaterialCBName, MaterialStruct.SizeInBytes, 3);
        public static ConstantBufferDescription BoneCB = new ConstantBufferDescription(BoneCBName, BoneMatricesStruct.SizeInBytes, 4);
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