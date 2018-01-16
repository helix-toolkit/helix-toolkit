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
        /// Convert shader stage into 0~5 stage numbers
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int Convert(ShaderStage type)
        {
            switch (type)
            {
                case ShaderStage.Vertex:
                    return 0;
                case ShaderStage.Hull:
                    return 1;
                case ShaderStage.Domain:
                    return 2;
                case ShaderStage.Geometry:
                    return 3;
                case ShaderStage.Pixel:
                    return 4;
                case ShaderStage.Compute:
                    return 5;
                default:
                    return -1;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ShaderStage
    {
        None = 0,
        Vertex = 1,
        Hull = 1<<2,
        Domain = 1<<3,
        Geometry = 1<<4,
        Pixel = 1<<5,
        Compute = 1<<6
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum StateType
    {
        None = 0,
        RasterState = 1,
        DepthStencilState = 1<<2,
        BlendState = 1<<3
    }
}
