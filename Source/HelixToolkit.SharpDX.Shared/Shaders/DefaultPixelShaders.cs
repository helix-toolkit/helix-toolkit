using SharpDX.Direct3D;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
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

        public static byte[] PSMeshClipPlane
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshClipPlane;
#else
                throw new NotImplementedException();
#endif
            }
        }

        public static byte[] PSMeshClipPlaneBackface
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshClipPlaneBackface;
#else
                throw new NotImplementedException();
#endif
            }
        }

        public static byte[] PSMeshClipPlaneQuad
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshClipPlaneQuad;
#else
                throw new NotImplementedException();
#endif
            }
        }
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
            DefaultPSShaderByteCodes.PSMeshVertColor);
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

        #region Mesh Clipping
        public static ShaderDescription PSMeshClipPlane = new ShaderDescription(nameof(PSMeshClipPlane), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshClipPlane,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
                DefaultConstantBufferDescriptions.LightCB.CreateMapping(2),
                DefaultConstantBufferDescriptions.MaterialCB.CreateMapping(3),
                DefaultConstantBufferDescriptions.ClipParamsCB.CreateMapping(5)

            },
            new TextureMapping[]
            {
                DefaultTextureBufferDescriptions.DiffuseMapTB.CreateMapping(0),
                DefaultTextureBufferDescriptions.AlphaMapTB.CreateMapping(1),
                DefaultTextureBufferDescriptions.NormalMapTB.CreateMapping(2),
                DefaultTextureBufferDescriptions.DisplacementMapTB.CreateMapping(3),
                DefaultTextureBufferDescriptions.ShadowMapTB.CreateMapping(5)
            });

        public static ShaderDescription PSMeshClipBackface = new ShaderDescription(nameof(PSMeshClipBackface), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshClipPlaneBackface, new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.ClipParamsCB.CreateMapping(5)
            });

        public static ShaderDescription PSMeshClipScreenQuad = new ShaderDescription(nameof(PSMeshClipScreenQuad), ShaderStage.Pixel, FeatureLevel.Level_11_0,
            DefaultPSShaderByteCodes.PSMeshClipPlaneQuad,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.ClipParamsCB.CreateMapping(5)
            });
        #endregion
    }
}
