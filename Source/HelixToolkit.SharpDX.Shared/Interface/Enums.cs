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
    public enum RenderType : ushort
    {
        None, Light, PreProc, Normal, Particle, PostProc, ScreenSpaced, Others
    }

    public enum MSAALevel : ushort
    {
        Disable = 0, Maximum = 1, Two = 2, Four = 4, Eight = 8
    }

    public enum LightType : ushort
    {
        Ambient = 0, Directional = 1, Point = 2, Spot = 3, ThreePoint = 4, None = 5
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PointFigure : ushort
    {
        Rect,
        Ellipse,
        Cross,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MeshTopologyEnum : ushort
    {
        PNTriangles,
        PNQuads
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ShaderStage : ushort
    {
        None = 0,
        Vertex = 1,
        Hull = 1 << 2,
        Domain = 1 << 3,
        Geometry = 1 << 4,
        Pixel = 1 << 5,
        Compute = 1 << 6
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum StateType : ushort
    {
        None = 0,
        RasterState = 1,
        DepthStencilState = 1 << 2,
        BlendState = 1 << 3
    }

    [Flags]
    public enum RenderDetail : ushort
    {
        None = 0,
        FPS = 1,
        Statistics = 2,
        TriangleInfo = 4,
        Camera = 8,
    }

    public struct EnumHelper
    {
        public static bool HasFlag(ShaderStage option, ShaderStage flag)
        {
            return (option & flag) != 0;
        }

        public static bool HasFlag(StateType option, StateType flag)
        {
            return (option & flag) != 0;
        }

        public static bool HasFlag(RenderDetail option, RenderDetail flag)
        {
            return (option & flag) != 0;
        }
    }
}
