/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using SharpDX.DXGI;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Helper;
    /// <summary>
    /// 
    /// </summary>
    public static class DefaultVSShaderByteCodes
    {
        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshDefault
        {
            get;
        } = "vsMeshDefault";
        /// <summary>
        /// Gets the vs mesh batched.
        /// </summary>
        /// <value>
        /// The vs mesh batched.
        /// </value>
        public static string VSMeshBatched
        {
            get;
        } = "vsMeshBatched";
        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshTessellation
        {
            get;
        } = "vsMeshTessellation";
        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshShadow
        {
            get;
        } = "vsMeshShadow";
        /// <summary>
        ///
        /// </summary>
        public static string VSMeshBatchedShadow
        {
            get;
        } = "vsMeshBatchedShadow";
        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshInstancing
        {
            get;
        } = "vsMeshInstancing";

        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshInstancingTessellation
        {
            get;
        } = "vsMeshInstancingTessellation";

        public static string VSMeshBoneSkinningBasic
        {
            get;
        } = "vsBoneSkinningBasic";

        /// <summary>
        /// 
        /// </summary>
        public static string VSPoint
        {
            get;
        } = "vsPoint";
        /// <summary>
        /// 
        /// </summary>
        public static string VSPointShadow
        {
            get;
        } = "vsPointShadow";
        /// <summary>
        /// 
        /// </summary>
        public static string VSBillboard
        {
            get;
        } = "vsBillboard";

        /// <summary>
        /// 
        /// </summary>
        public static string VSBillboardInstancing
        {
            get;
        } = "vsBillboardInstancing";
        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshClipPlane
        {
            get;
        } = "vsMeshClipPlane";
        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshClipPlaneQuad
        {
            get;
        } = "vsMeshClipPlaneQuad";

        /// <summary>
        /// 
        /// </summary>
        public static string VSParticle
        {
            get;
        } = "vsParticle";

        /// <summary>
        /// 
        /// </summary>
        public static string VSSkybox
        {
            get;
        } = "vsSkybox";

        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshWireframe
        {
            get;
        } = "vsMeshWireframe";
        /// <summary>
        /// Gets the vs mesh batched wireframe.
        /// </summary>
        /// <value>
        /// The vs mesh batched wireframe.
        /// </value>
        public static string VSMeshBatchedWireframe
        {
            get;
        } = "vsMeshBatchedWireframe";
        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshBoneSkinningWireframe
        {
            get;
        } = "vsBoneSkinningWireframe";

        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshOutlineP1
        {
            get;
        } = "vsMeshOutlinePass1";


        /// <summary>
        /// 
        /// </summary>
        public static string VSMeshOutlineScreenQuad
        {
            get;
        } = "vsMeshOutlineScreenQuad";

        /// <summary>
        /// Gets the vs plane grid.
        /// </summary>
        /// <value>
        /// The vs plane grid.
        /// </value>
        public static string VSPlaneGrid
        {
            get;
        } = "vsPlaneGrid";
#if !NETFX_CORE
        /// <summary>
        /// 
        /// </summary>
        public static string VSScreenDup
        {
            get;
        } = "vsScreenDup";
        /// <summary>
        /// 
        /// </summary>
        public static string VSScreenDupCursor
        {
            get;
        } = "vsScreenDupCursor";
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
            new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),  
            new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 1),  
            new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2),             
            //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
            new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1)
        };

        public static InputElement[] VSMeshBatchedInput { get; } = new InputElement[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
            new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
            new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
            new InputElement("COLOR",    1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
        };
        /// <summary>
        /// 
        /// </summary>
        public static InputElement[] VSInputInstancing { get; } = new InputElement[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
            new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),  
            new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 1),       
            new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2),             
            //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
            new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
            new InputElement("COLOR", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 4, InputClassification.PerInstanceData, 1),
            new InputElement("COLOR", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 4, InputClassification.PerInstanceData, 1),
            new InputElement("COLOR", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 4, InputClassification.PerInstanceData, 1),
            new InputElement("TEXCOORD", 5, Format.R32G32_Float, InputElement.AppendAligned, 4, InputClassification.PerInstanceData, 1),
        };

        /// <summary>
        /// Gets the vs input bone skinned basic.
        /// </summary>
        /// <value>
        /// The vs input bone skinned basic.
        /// </value>
        public static InputElement[] VSInputBoneSkinnedBasic { get; } = new InputElement[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
            new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
            new InputElement("BONEIDS", 0, Format.R32G32B32A32_SInt, InputElement.AppendAligned, 1),
            new InputElement("BONEWEIGHTS", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1),
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
        public static ShaderDescription VSMeshBatched = new ShaderDescription(nameof(VSMeshBatched), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshBatched);
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
        public static ShaderDescription VSMeshBatchedShadow = new ShaderDescription(nameof(VSMeshBatchedShadow), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshBatchedShadow);
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
        /// The vs mesh bone skinned basic
        /// </summary>
        public static ShaderDescription VSMeshBoneSkinnedBasic = new ShaderDescription(nameof(VSMeshBoneSkinnedBasic), ShaderStage.Vertex,
            new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshBoneSkinningBasic);

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
        public static ShaderDescription VSMeshClipPlane = new ShaderDescription(nameof(VSMeshClipPlane), ShaderStage.Vertex,
            new ShaderReflector(),
            DefaultVSShaderByteCodes.VSMeshClipPlane);

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
        /// The vs mesh batched wireframe
        /// </summary>
        public static ShaderDescription VSMeshBatchedWireframe = new ShaderDescription(nameof(VSMeshBatchedWireframe), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshBatchedWireframe);
        /// <summary>
        /// The vs bone skinning wireframe
        /// </summary>
        public static ShaderDescription VSBoneSkinningWireframe = new ShaderDescription(nameof(VSBoneSkinningWireframe), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshBoneSkinningWireframe);

        /// <summary>
        /// The vs mesh outline pass1
        /// </summary>
        public static ShaderDescription VSMeshOutlinePass1 = new ShaderDescription(nameof(VSMeshOutlinePass1), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshOutlineP1);

        /// <summary>
        /// The vs mesh outline pass1
        /// </summary>
        public static ShaderDescription VSMeshOutlineScreenQuad = new ShaderDescription(nameof(VSMeshOutlineScreenQuad), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSMeshOutlineScreenQuad);
        /// <summary>
        /// The vs plane grid
        /// </summary>
        public static ShaderDescription VSPlaneGrid = new ShaderDescription(nameof(VSPlaneGrid), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSPlaneGrid);
#if !NETFX_CORE
        /// <summary>
        /// The vs screen dup
        /// </summary>
        public static ShaderDescription VSScreenDup = new ShaderDescription(nameof(VSScreenDup), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSScreenDup);

        /// <summary>
        /// The vs screen dup mouse cursor
        /// </summary>
        public static ShaderDescription VSScreenDupCursor = new ShaderDescription(nameof(VSScreenDupCursor), ShaderStage.Vertex, new ShaderReflector(), DefaultVSShaderByteCodes.VSScreenDupCursor);

#endif
    }
}
