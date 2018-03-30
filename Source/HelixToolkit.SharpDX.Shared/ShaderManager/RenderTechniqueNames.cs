/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public struct DefaultRenderTechniqueNames
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Blinn = "RenderBlinn";
        /// <summary>
        /// 
        /// </summary>
        public const string Diffuse = "RenderDiffuse";
        /// <summary>
        /// 
        /// </summary>
        public const string Colors = "RenderColors";
        /// <summary>
        /// 
        /// </summary>
        public const string Positions = "RenderPositions";
        /// <summary>
        /// 
        /// </summary>
        public const string Normals = "RenderNormals";
        /// <summary>
        /// 
        /// </summary>
        public const string PerturbedNormals = "RenderPerturbedNormals";
        /// <summary>
        /// 
        /// </summary>
        public const string Tangents = "RenderTangents";
        /// <summary>
        /// 
        /// </summary>
        public const string TexCoords = "RenderTexCoords";
        /// <summary>
        /// 
        /// </summary>
        public const string Lines = "RenderLines";
        /// <summary>
        /// 
        /// </summary>
        public const string Points = "RenderPoints";
        /// <summary>
        /// 
        /// </summary>
        public const string CubeMap = "RenderCubeMap";
        /// <summary>
        /// 
        /// </summary>
        public const string BillboardText = "RenderBillboard";
        /// <summary>
        /// 
        /// </summary>
        public const string BillboardInstancing = "RenderBillboardInstancing";
        /// <summary>
        /// 
        /// </summary>
        public const string InstancingBlinn = "RenderInstancingBlinn";
        /// <summary>
        /// 
        /// </summary>
        public const string BoneSkinBlinn = "RenderBoneSkinBlinn";
        /// <summary>
        /// 
        /// </summary>
        public const string ParticleStorm = "ParticleStorm";
        /// <summary>
        /// 
        /// </summary>
        public const string CrossSection = "RenderCrossSectionBlinn";
        /// <summary>
        /// 
        /// </summary>
        public const string ViewCube = "RenderViewCube";
        /// <summary>
        /// 
        /// </summary>
        public const string Skybox = "Skybox";
        /// <summary>
        /// 
        /// </summary>
        public const string PostEffectMeshBorderHighlight = "PostEffectMeshBorderHighlight";

        /// <summary>
        /// The post effect mesh x ray
        /// </summary>
        public const string PostEffectMeshXRay = "PostEffectMeshXRay";

        /// <summary>
        /// 
        /// </summary>
        public const string PostEffectMeshOutlineBlur = "PostEffectMeshOutlineBlur";

        /// <summary>
        /// 
        /// </summary>
        public const string PostEffectBloom = "PostEffectBloom";
#if !NETFX_CORE
        /// <summary>
        /// 
        /// </summary>
        public const string ScreenDuplication = "ScreenDup";
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public struct DefaultPassNames
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Default = "Default";
        /// <summary>
        /// 
        /// </summary>
        public const string MeshTriTessellation = "MeshTriTessellation";
        /// <summary>
        /// 
        /// </summary>
        public const string MeshQuadTessellation = "MeshQuadTessellation";
        /// <summary>
        /// 
        /// </summary>
        public const string MeshOutline = "RenderMeshOutline";

        /// <summary>
        /// 
        /// </summary>
        public const string EffectBlurVertical = "EffectBlurVertical";

        /// <summary>
        /// 
        /// </summary>
        public const string EffectBlurHorizontal = "EffectBlurHorizontal";

        /// <summary>
        /// 
        /// </summary>
        public const string EffectOutlineP1 = "RenderMeshOutlineP1";

        /// <summary>
        /// 
        /// </summary>
        public const string EffectMeshXRayP1 = "EffectMeshXRayP1";
        /// <summary>
        /// 
        /// </summary>
        public const string EffectMeshXRayP2 = "EffectMeshXRayP2";
        /// <summary>
        /// 
        /// </summary>
        public const string ShadowPass = "RenderShadow";
        /// <summary>
        /// 
        /// </summary>
        public const string Backface = "RenderBackface";
        /// <summary>
        /// 
        /// </summary>
        public const string ScreenQuad = "ScreenQuad";
        /// <summary>
        /// 
        /// </summary>
        public const string ScreenQuadCopy = "ScreenQuadCopy";
        /// <summary>
        /// The wireframe
        /// </summary>
        public const string Wireframe = "Wireframe";
    }
    /// <summary>
    /// 
    /// </summary>
    public struct DefaultParticlePassNames
    {
        /// <summary>
        /// The insert
        /// </summary>
        public const string Insert = "InsertParticle";//For compute shader
        /// <summary>
        /// The update
        /// </summary>
        public const string Update = "UpdateParticle";//For compute shader
        /// <summary>
        /// The default
        /// </summary>
        public const string Default = "Default";//For rendering
    }

    //public struct TessellationRenderTechniqueNames
    //{
    //    public const string PNTriangles = "RenderPNTriangs";
    //    public const string PNQuads = "RenderPNQuads";
    //}
    /// <summary>
    /// 
    /// </summary>
    public struct DeferredRenderTechniqueNames
    {
        /// <summary>
        /// The deferred
        /// </summary>
        public const string Deferred = "RenderDeferred";
        /// <summary>
        /// The g buffer
        /// </summary>
        public const string GBuffer = "RenderGBuffer";
        /// <summary>
        /// The deferred lighting
        /// </summary>
        public const string DeferredLighting = "RenderDeferredLighting";
        /// <summary>
        /// The screen space
        /// </summary>
        public const string ScreenSpace = "RenderScreenSpace";
    }
}
