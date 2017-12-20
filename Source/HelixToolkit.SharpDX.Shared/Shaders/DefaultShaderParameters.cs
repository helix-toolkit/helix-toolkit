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
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class DefaultVSShaderByteCodes
    {
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSPoint
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsPoint;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSBillboard
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsBillboard;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSMeshXRay
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsMeshXRay;
#else
                throw new NotImplementedException();
#endif
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class DefaultPSShaderByteCodes
    {
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSPoint
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psPoint;
#else
                throw new NotImplementedException();
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSLine
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psLine;
#else
                throw new NotImplementedException();
#endif

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSBillboardText
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psBillboardText;
#else
                throw new NotImplementedException();
#endif

            }
        }

        public static byte[] PSMeshXRay
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshXRay;
#else
                throw new NotImplementedException();
#endif
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DefaultGSShaderByteCodes
    {
        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSPoint
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.gsPoint;
#else
                throw new NotImplementedException();
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSLine
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.gsLine;
#else
                throw new NotImplementedException();
#endif

            }
        }

        public static byte[] GSBillboard
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.gsBillboard;
#else
                throw new NotImplementedException();
#endif

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DefaultInputLayout
    {
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public static InputElement[] VSInputPoint { get; } = new InputElement[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
            new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
            //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
            new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
        };

        /// <summary>
        /// 
        /// </summary>
        public static InputElement[] VSInputBillboard { get; } = new InputElement[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
            new InputElement("COLOR",    0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
            new InputElement("COLOR",    1, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 1, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 2, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 3, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 4, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 5, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 6, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 7, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
            new InputElement("TEXCOORD", 8, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 9, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 10, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 11, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
        };
    }
    /// <summary>
    /// 
    /// </summary>
    public static class DefaultVSShaderDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshDefault = new ShaderDescription(nameof(VSMeshDefault), ShaderStage.Vertex, FeatureLevel.Level_11_0, 
            DefaultVSShaderByteCodes.VSMeshDefault, 
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
                DefaultConstantBufferDescriptions.LightCB.CreateMapping(2),
                DefaultConstantBufferDescriptions.MaterialCB.CreateMapping(3)
            }, 
            null);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshInstancing = new ShaderDescription(nameof(VSMeshInstancing), ShaderStage.Vertex, FeatureLevel.Level_11_0,
            DefaultVSShaderByteCodes.VSMeshInstancing,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
                DefaultConstantBufferDescriptions.LightCB.CreateMapping(2),
                DefaultConstantBufferDescriptions.MaterialCB.CreateMapping(3)
            },
            null);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshBoneSkinning = new ShaderDescription(nameof(VSMeshBoneSkinning), ShaderStage.Vertex, FeatureLevel.Level_11_0,
            DefaultVSShaderByteCodes.VSMeshBoneSkinning,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
                DefaultConstantBufferDescriptions.LightCB.CreateMapping(2),
                DefaultConstantBufferDescriptions.MaterialCB.CreateMapping(3),
                DefaultConstantBufferDescriptions.BoneCB.CreateMapping(4)
            }, 
            null);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSPoint = new ShaderDescription(nameof(VSPoint), ShaderStage.Vertex, FeatureLevel.Level_11_0,
            DefaultVSShaderByteCodes.VSPoint,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
            },
            null);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSBillboardText = new ShaderDescription(nameof(VSBillboardText), ShaderStage.Vertex, FeatureLevel.Level_11_0,
            DefaultVSShaderByteCodes.VSBillboard,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
            },
            null);

        public static ShaderDescription VSMeshXRay = new ShaderDescription(nameof(VSMeshXRay), ShaderStage.Vertex, FeatureLevel.Level_11_0,
            DefaultVSShaderByteCodes.VSMeshXRay,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
            });
    }

    /// <summary>
    /// Default Pixel Shaders
    /// </summary>
    public static class DefaultPSShaderDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshBlinnPhong = new ShaderDescription(nameof(PSMeshBlinnPhong), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshBinnPhong,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
                DefaultConstantBufferDescriptions.LightCB.CreateMapping(2),
                DefaultConstantBufferDescriptions.MaterialCB.CreateMapping(3)
            },
            new TextureMapping[] 
            {
                DefaultTextureBufferDescriptions.DiffuseMapTB.CreateMapping(0),
                DefaultTextureBufferDescriptions.AlphaMapTB.CreateMapping(1),
                DefaultTextureBufferDescriptions.NormalMapTB.CreateMapping(2),
                DefaultTextureBufferDescriptions.DisplacementMapTB.CreateMapping(3),              
                DefaultTextureBufferDescriptions.ShadowMapTB.CreateMapping(5)
            });
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshVertColor = new ShaderDescription(nameof(PSMeshVertColor), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshBinnPhong);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshVertNormal = new ShaderDescription(nameof(PSMeshVertNormal), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshNormal);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshVertPosition = new ShaderDescription(nameof(PSMeshVertPosition), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshVertPosition);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSPoint = new ShaderDescription(nameof(PSPoint), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSPoint,
            new ConstantBufferMapping[] 
            {
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1)
            });
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSLine = new ShaderDescription(nameof(PSLine), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSLine, 
            new ConstantBufferMapping[] 
            {
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1)
            });

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSBillboardText = new ShaderDescription(nameof(PSBillboardText), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSBillboardText,
            null, new TextureMapping[] { DefaultTextureBufferDescriptions.BillboardTB.CreateMapping(0) });

        public static ShaderDescription PSMeshXRay = new ShaderDescription(nameof(PSMeshXRay), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshXRay,
            new ConstantBufferMapping[] { DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1) });
    }

    /// <summary>
    /// Default Geometry Shaders
    /// </summary>
    public static class DefaultGSShaderDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription GSPoint = new ShaderDescription(nameof(GSPoint), ShaderStage.Geometry, FeatureLevel.Level_11_0,
            DefaultGSShaderByteCodes.GSPoint,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1)
            });
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription GSLine = new ShaderDescription(nameof(GSLine), ShaderStage.Geometry, FeatureLevel.Level_11_0,
            DefaultGSShaderByteCodes.GSLine,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1)
            });

        public static ShaderDescription GSBillboard = new ShaderDescription(nameof(GSBillboard), ShaderStage.Geometry, FeatureLevel.Level_11_0,
            DefaultGSShaderByteCodes.GSBillboard,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1)
            });
    }
    /// <summary>
    /// 
    /// </summary>
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

        public readonly static DepthStencilStateDescription DSSGreaterNoWrite = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.Zero,
            DepthComparison = Comparison.Greater
        };
    }

    public static class DefaultRasterDescriptions
    {

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
        public static TextureDescription DiffuseMapTB = new TextureDescription(nameof(DiffuseMapTB), ShaderStage.Pixel);
        public static TextureDescription AlphaMapTB = new TextureDescription(nameof(AlphaMapTB), ShaderStage.Pixel);
        public static TextureDescription NormalMapTB = new TextureDescription(nameof(NormalMapTB), ShaderStage.Pixel);
        public static TextureDescription DisplacementMapTB = new TextureDescription(nameof(DisplacementMapTB), ShaderStage.Pixel);
        public static TextureDescription CubeMapTB = new TextureDescription(nameof(CubeMapTB), ShaderStage.Pixel);
        public static TextureDescription ShadowMapTB = new TextureDescription(nameof(ShadowMapTB), ShaderStage.Pixel);
        public static TextureDescription BillboardTB = new TextureDescription(nameof(BillboardTB), ShaderStage.Pixel);
    }
}