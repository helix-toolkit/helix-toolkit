/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderCore : IGUID
    {
        event EventHandler<bool> OnInvalidateRenderer;

        Matrix ModelMatrix { set; get; }
        IRenderTechnique EffectTechnique { get; }
        Device Device { get; }
        /// <summary>
        /// If render core is attached or not
        /// </summary>
        bool IsAttached { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="technique"></param>
        void Attach(IRenderTechnique technique);
        /// <summary>
        /// 
        /// </summary>
        void Detach();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Render(IRenderContext context);
        /// <summary>
        /// Unsubscribe all OnInvalidateRenderer event handler;
        /// </summary>
        void ResetInvalidateHandler();
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IGeometryRenderCore : IThrowingShadow
    {
        InputLayout VertexLayout { get; }
        IElementsBufferModel InstanceBuffer { set; get; }
        IGeometryBufferModel GeometryBuffer { set; get; }
        RasterizerStateDescription RasterDescription { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IMaterialRenderParams
    {
        IMaterial Material { set; get; }
        bool RenderDiffuseMap { set; get; }
        bool RenderDiffuseAlphaMap { set; get; }
        bool RenderNormalMap { set; get; }
        bool RenderDisplacementMap { set; get; }
        bool RenderShadowMap { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IInvertNormal
    {
        bool InvertNormal { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IBillboardRenderParams
    {
        bool FixedSize { set; get; }
        SamplerStateDescription SamplerDescription { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IBoneSkinRenderParams
    {
        IElementsBufferModel VertexBoneIdBuffer { set; get; }
        BoneMatricesStruct BoneMatrices { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface ICrossSectionRenderParams
    {
        Color4 SectionColor { set; get; }

        bool Plane1Enabled { set; get; }
        bool Plane2Enabled { set; get; }
        bool Plane3Enabled { set; get; }
        bool Plane4Enabled { set; get; }

        /// <summary>
        /// Defines the plane (Normal + d)
        /// </summary>
        Vector4 Plane1Params { set; get; }
        Vector4 Plane2Params { set; get; }
        Vector4 Plane3Params { set; get; }
        Vector4 Plane4Params { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMeshOutlineParams
    {
        Color4 Color { set; get; }
        /// <summary>
        /// Enable outline
        /// </summary>
        bool OutlineEnabled { set; get; }

        /// <summary>
        /// Draw original mesh
        /// </summary>
        bool DrawMesh { set; get; }

        /// <summary>
        /// Draw outline order
        /// </summary>
        bool DrawOutlineBeforeMesh { set; get; }

        /// <summary>
        /// Outline fading
        /// </summary>
        float OutlineFadingFactor { set; get; }
    }

    public enum MeshTopologyEnum
    {
        PNTriangles,
        PNQuads
    }
    public static class MeshTopologies
    {
        public static IEnumerable<MeshTopologyEnum> Topologies
        {
            get
            {
                yield return MeshTopologyEnum.PNTriangles;
                yield return MeshTopologyEnum.PNQuads;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IPatchRenderParams
    {
        /// <summary>
        /// 
        /// </summary>
        float MinTessellationDistance { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float MaxTessellationDistance { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float MinTessellationFactor { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float MaxTessellationFactor { set; get; }
        /// <summary>
        /// 
        /// </summary>
        MeshTopologyEnum MeshType { set; get; }
        /// <summary>
        /// 
        /// </summary>
        bool EnableTessellation { set; get; }
    }

    public enum PointFigure
    {
        Rect,
        Ellipse,
        Cross,
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IPointRenderParams
    {
        /// <summary>
        /// 
        /// </summary>
        Color4 PointColor { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float Width { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float Height { set; get; }
        /// <summary>
        /// 
        /// </summary>
        PointFigure Figure { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float FigureRatio { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IShadowMapRenderParams
    {
        /// <summary>
        /// 
        /// </summary>
        int Width { set; get; }
        /// <summary>
        /// 
        /// </summary>
        int Height { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float Bias { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float Intensity { set; get; }
        /// <summary>
        /// 
        /// </summary>
        Matrix LightViewProjectMatrix { set; get; }
        /// <summary>
        /// Update shadow map every N frames
        /// </summary>
        int UpdateFrequency { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface ISkyboxRenderParams
    {
        Stream CubeTexture { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IThrowingShadow
    {
        bool IsThrowingShadow { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface ILineRenderParams
    {
        /// <summary>
        /// 
        /// </summary>
        float Thickness { set; get; }

        /// <summary>
        /// 
        /// </summary>
        float Smoothness { set; get; }
        /// <summary>
        /// Final Line Color = LineColor * PerVertexLineColor
        /// </summary>
        Color4 LineColor { set; get; }
    }
}
