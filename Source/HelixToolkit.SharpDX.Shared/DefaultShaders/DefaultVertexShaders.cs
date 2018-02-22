/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;

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
        /// <summary>
        /// 
        /// </summary>
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
        public static byte[] VSMeshShadow
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsMeshShadow;
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
        public static byte[] VSMeshInstancingTessellation
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsMeshInstancingTessellation;
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
        public static byte[] VSMeshBoneSkinningShadow
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsBoneSkinningShadow;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSMeshBoneSkinningTessellation
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsBoneSkinningTessellation;
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
        public static byte[] VSPointShadow
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsPointShadow;
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
        public static byte[] VSBillboardInstancing
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsBillboardInstancing;
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSParticle
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsParticle;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSSkybox
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsSkybox;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSMeshWireframe
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsMeshWireframe;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSMeshBoneSkinningWireframe
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsBoneSkinningWireframe;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSMeshOutlineP1
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.vsMeshOutlinePass1;
#else
                throw new NotImplementedException();
#endif
            }
        }

#if !NETFX_CORE
        /// <summary>
        /// 
        /// </summary>
        public static byte[] VSScreenDup
        {
            get
            {
                return Properties.Resources.vsScreenDup;
            }
        }
#endif
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
            new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
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
            //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
            new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 5, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 6, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 7, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
        };
        /// <summary>
        /// Gets the vs input billboard instancing.
        /// </summary>
        /// <value>
        /// The vs input billboard instancing.
        /// </value>
        public static InputElement[] VSInputBillboardInstancing { get; } = new InputElement[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
            new InputElement("COLOR",    0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
            new InputElement("COLOR",    1, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 1, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 2, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 3, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
            new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 5, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 6, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 7, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            new InputElement("COLOR", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 8, Format.R32G32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 9, Format.R32G32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
        };
        /// <summary>
        /// Gets the vs input particle.
        /// </summary>
        /// <value>
        /// The vs input particle.
        /// </value>
        public static InputElement[] VSInputParticle { get; } = new InputElement[]
        {
            new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerInstanceData, 1),
        };
        /// <summary>
        /// Gets the vs input skybox.
        /// </summary>
        /// <value>
        /// The vs input skybox.
        /// </value>
        public static InputElement[] VSInputSkybox { get; } = new InputElement[]
        {
            new InputElement("SV_POSITION", 0, Format.R32G32B32_Float,  InputElement.AppendAligned, 0),
        };

#if !NETFX_CORE
        /// <summary>
        /// Gets the vs input screen dup.
        /// </summary>
        /// <value>
        /// The vs input screen dup.
        /// </value>
        public static InputElement[] VSInputScreenDup { get; } = new InputElement[]
        {
            new InputElement("SV_POSITION", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32_Float, InputElement.AppendAligned, 0)
        };
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DefaultVSShaderDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshDefault = new ShaderDescription(nameof(VSMeshDefault), ShaderStage.Vertex, 
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshDefault);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshTessellation = new ShaderDescription(nameof(VSMeshTessellation), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshTessellation);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshShadow = new ShaderDescription(nameof(VSMeshShadow), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshShadow);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshInstancing = new ShaderDescription(nameof(VSMeshInstancing), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshInstancing);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshInstancingTessellation = new ShaderDescription(nameof(VSMeshInstancingTessellation), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshInstancingTessellation);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshBoneSkinning = new ShaderDescription(nameof(VSMeshBoneSkinning), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshBoneSkinning);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshBoneSkinningTessellation = new ShaderDescription(nameof(VSMeshBoneSkinningTessellation), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshBoneSkinningTessellation);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshBoneSkinningShadow = new ShaderDescription(nameof(VSMeshBoneSkinningShadow), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshBoneSkinningShadow);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSPoint = new ShaderDescription(nameof(VSPoint), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSPoint);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSPointShadow = new ShaderDescription(nameof(VSPointShadow), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSPointShadow);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSBillboardText = new ShaderDescription(nameof(VSBillboardText), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSBillboard);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSBillboardInstancing = new ShaderDescription(nameof(VSBillboardInstancing), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSBillboardInstancing);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSMeshXRay = new ShaderDescription(nameof(VSMeshXRay), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshXRay);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSScreenQuad = new ShaderDescription(nameof(VSScreenQuad), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshClipPlaneQuad);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSParticle = new ShaderDescription(nameof(VSParticle), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSParticle);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription VSSkybox = new ShaderDescription(nameof(VSSkybox), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSSkybox);
        /// <summary>
        /// The vs mesh wireframe
        /// </summary>
        public static ShaderDescription VSMeshWireframe = new ShaderDescription(nameof(VSMeshWireframe), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshWireframe);

        /// <summary>
        /// The vs bone skinning wireframe
        /// </summary>
        public static ShaderDescription VSBoneSkinningWireframe = new ShaderDescription(nameof(VSBoneSkinningWireframe), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshBoneSkinningWireframe);

        /// <summary>
        /// The vs mesh outline pass1
        /// </summary>
        public static ShaderDescription VSMeshOutlinePass1 = new ShaderDescription(nameof(VSMeshOutlinePass1), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshOutlineP1);
#if !NETFX_CORE
        /// <summary>
        /// The vs screen dup
        /// </summary>
        public static ShaderDescription VSScreenDup = new ShaderDescription(nameof(VSScreenDup), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSScreenDup);
#endif
    }
}
