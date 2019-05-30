/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Core;
    using Core2D;
    using Model.Scene;
    using Model.Scene2D;
    using System.Collections.Generic;
    using Shaders;
    using System.Runtime.CompilerServices;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Used for static function overloading
    /// </summary>
    public struct VertexShaderType { }
    public struct HullShaderType { }
    public struct DomainShaderType { }
    public struct GeometryShaderType { }
    public struct PixelShaderType { }
    public struct ComputeShaderType { }
    /// <summary>
    /// 
    /// </summary>
    public static class Constants
    {
        public const int MaxLights = 8;
        /// <summary>
        /// Number of shader stages
        /// </summary>
        public const int NumShaderStages = 6;

        /// <summary>
        /// Stages that can bind textures
        /// </summary>
        public const ShaderStage CanBindTextureStages = ShaderStage.Vertex | ShaderStage.Pixel | ShaderStage.Domain | ShaderStage.Compute;

        /// <summary>
        /// 
        /// </summary>
        public const int VertexIdx = 0, HullIdx = 1, DomainIdx = 2, GeometryIdx = 3, PixelIdx = 4, ComputeIdx = 5;
        /// <summary>
        /// Convert shader stage into 0~5 stage numbers
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndex(this ShaderStage type)
        {
            switch (type)
            {
                case ShaderStage.Vertex:
                    return VertexIdx;
                case ShaderStage.Hull:
                    return HullIdx;
                case ShaderStage.Domain:
                    return DomainIdx;
                case ShaderStage.Geometry:
                    return GeometryIdx;
                case ShaderStage.Pixel:
                    return PixelIdx;
                case ShaderStage.Compute:
                    return ComputeIdx;
                default:
                    return -1;
            }
        }
        /// <summary>
        /// To the shader stage.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ShaderStage ToShaderStage(this int index)
        {
            switch (index)
            {
                case VertexIdx:
                    return ShaderStage.Vertex;
                case DomainIdx:
                    return ShaderStage.Domain;
                case HullIdx:
                    return ShaderStage.Hull;
                case GeometryIdx:
                    return ShaderStage.Geometry;
                case PixelIdx:
                    return ShaderStage.Pixel;
                case ComputeIdx:
                    return ShaderStage.Compute;
                default:
                    return ShaderStage.None;
            }
        }
        /// <summary>
        /// Gets the null shader.
        /// </summary>
        /// <param name="stage">The stage.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ShaderBase GetNullShader(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    return VertexShader.NullVertexShader;
                case ShaderStage.Domain:
                    return DomainShader.NullDomainShader;
                case ShaderStage.Hull:
                    return HullShader.NullHullShader;
                case ShaderStage.Geometry:
                    return GeometryShader.NullGeometryShader;
                case ShaderStage.Pixel:
                    return PixelShader.NullPixelShader;
                case ShaderStage.Compute:
                    return ComputeShader.NullComputeShader;
                default:
                    return null;
            }
        }

        public static readonly char[] Separators = { ';', ' ', ',' };
        public static readonly FastList<KeyValuePair<int, SceneNode>> EmptyRenderablePair = new FastList<KeyValuePair<int, SceneNode>>();
        public static readonly FastList<SceneNode> EmptyRenderable = new FastList<SceneNode>();
        public static readonly List<RenderCore> EmptyCore = new List<RenderCore>();
        public static readonly ObservableCollection<SceneNode> EmptyRenderableArray = new ObservableCollection<SceneNode>();
        public static readonly ReadOnlyObservableCollection<SceneNode> EmptyReadOnlyRenderableArray;
        public static readonly ObservableCollection<SceneNode2D> EmptyRenderable2D = new ObservableCollection<SceneNode2D>();
        public static readonly ReadOnlyObservableCollection<SceneNode2D> EmptyReadOnlyRenderable2DArray;
        public static readonly IList<RenderCore2D> EmptyCore2D = new RenderCore2D[0];

        static Constants()
        {
            EmptyReadOnlyRenderableArray = new ReadOnlyObservableCollection<SceneNode>(EmptyRenderableArray);
            EmptyReadOnlyRenderable2DArray = new ReadOnlyObservableCollection<SceneNode2D>(EmptyRenderable2D);
        }
    }
}
