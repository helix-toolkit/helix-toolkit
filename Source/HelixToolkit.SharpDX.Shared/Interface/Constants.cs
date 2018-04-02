/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;
    using Core2D;
    using Model.Scene;
    using Model.Scene2D;
    using System.Collections.Generic;

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

        public static readonly char[] Separators = { ';', ' ', ',' };

        public static readonly List<SceneNode> EmptyRenderable = new List<SceneNode>();
        public static readonly List<RenderCore> EmptyCore = new List<RenderCore>();
        public static readonly IList<SceneNode> EmptyRenderableArray = new SceneNode[0];
        public static readonly IList<SceneNode2D> EmptyRenderable2D = new SceneNode2D[0];
        public static readonly IList<RenderCore2D> EmptyCore2D = new RenderCore2D[0];
    }
}
