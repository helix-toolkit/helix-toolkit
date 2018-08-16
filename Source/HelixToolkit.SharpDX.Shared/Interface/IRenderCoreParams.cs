/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.IO;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public interface IGeometryRenderCore
    {
        /// <summary>
        /// Gets or sets the instance buffer.
        /// </summary>
        /// <value>
        /// The instance buffer.
        /// </value>
        IElementsBufferModel InstanceBuffer { set; get; }
        /// <summary>
        /// Gets or sets the geometry buffer.
        /// </summary>
        /// <value>
        /// The geometry buffer.
        /// </value>
        IAttachableBufferModel GeometryBuffer { set; get; }
        /// <summary>
        /// Gets or sets the raster description.
        /// </summary>
        /// <value>
        /// The raster description.
        /// </value>
        RasterizerStateDescription RasterDescription { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMaterialRenderParams
    {
        /// <summary>
        /// Gets or sets the material variables used for rendering.
        /// </summary>
        /// <value>
        /// The material variable.
        /// </value>
        MaterialVariable MaterialVariables { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMeshRenderParams : IInvertNormal, IMaterialRenderParams
    {
        bool RenderWireframe { set; get; }
        Color4 WireframeColor { set; get; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public interface IDynamicReflector
    {
        bool EnableReflector { set; get; }
        Vector3 Center { set; get; }
        int FaceSize { set; get; }
        float NearField { set; get; }
        float FarField { set; get; }
        bool IsLeftHanded { set; get; }
        void BindCubeMap(DeviceContextProxy deviceContext);
        void UnBindCubeMap(DeviceContextProxy deviceContext);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDynamicReflectable
    {
        IDynamicReflector DynamicReflector { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IInvertNormal
    {
        /// <summary>
        /// Gets or sets a value indicating whether [invert normal].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invert normal]; otherwise, <c>false</c>.
        /// </value>
        bool InvertNormal { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IBillboardRenderParams
    {
        /// <summary>
        /// Gets or sets a value indicating whether [fixed size].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixed size]; otherwise, <c>false</c>.
        /// </value>
        bool FixedSize { set; get; }
        /// <summary>
        /// Gets or sets the sampler description.
        /// </summary>
        /// <value>
        /// The sampler description.
        /// </value>
        SamplerStateDescription SamplerDescription { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ICrossSectionRenderParams
    {
        /// <summary>
        /// Cutting operation, intersects or substract
        /// </summary>
        CuttingOperation CuttingOperation { set; get; }
        /// <summary>
        /// Gets or sets the color of the section.
        /// </summary>
        /// <value>
        /// The color of the section.
        /// </value>
        Color4 SectionColor { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [plane1 enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [plane1 enabled]; otherwise, <c>false</c>.
        /// </value>
        bool Plane1Enabled { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [plane2 enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [plane2 enabled]; otherwise, <c>false</c>.
        /// </value>
        bool Plane2Enabled { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [plane3 enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [plane3 enabled]; otherwise, <c>false</c>.
        /// </value>
        bool Plane3Enabled { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [plane4 enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [plane4 enabled]; otherwise, <c>false</c>.
        /// </value>
        bool Plane4Enabled { set; get; }

        /// <summary>
        /// Defines the plane (Normal + d)
        /// </summary>
        Vector4 Plane1Params { set; get; }
        /// <summary>
        /// Gets or sets the plane2 parameters.(Normal + d)
        /// </summary>
        /// <value>
        /// The plane2 parameters.
        /// </value>
        Vector4 Plane2Params { set; get; }
        /// <summary>
        /// Gets or sets the plane3 parameters.(Normal + d)
        /// </summary>
        /// <value>
        /// The plane3 parameters.
        /// </value>
        Vector4 Plane3Params { set; get; }
        /// <summary>
        /// Gets or sets the plane4 parameters.(Normal + d)
        /// </summary>
        /// <value>
        /// The plane4 parameters.
        /// </value>
        Vector4 Plane4Params { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMeshOutlineParams
    {
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
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

    /// <summary>
    /// 
    /// </summary>
    public static class MeshTopologies
    {
        /// <summary>
        /// Gets the topologies.
        /// </summary>
        /// <value>
        /// The topologies.
        /// </value>
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
        /// <summary>
        /// Gets or sets the cube texture.
        /// </summary>
        /// <value>
        /// The cube texture.
        /// </value>
        Stream CubeTexture { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IThrowingShadow
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is throwing shadow.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is throwing shadow; otherwise, <c>false</c>.
        /// </value>
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
