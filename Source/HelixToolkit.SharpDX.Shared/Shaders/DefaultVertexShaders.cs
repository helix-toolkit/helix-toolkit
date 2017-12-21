using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{

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

        public static byte[] VSMeshTessellation
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsMeshTessellation;
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

        public static byte[] VSMeshClipPlaneQuad
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsMeshClipPlaneQuad;
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

        public static ShaderDescription VSMeshTessellation = new ShaderDescription(nameof(VSMeshTessellation), ShaderStage.Vertex, FeatureLevel.Level_11_0,
            DefaultVSShaderByteCodes.VSMeshTessellation,
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

        public static ShaderDescription VSScreenQuad = new ShaderDescription(nameof(VSScreenQuad), ShaderStage.Vertex, FeatureLevel.Level_11_0,
            DefaultVSShaderByteCodes.VSMeshClipPlaneQuad);
    }
}
