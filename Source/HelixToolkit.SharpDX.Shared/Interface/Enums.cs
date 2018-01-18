/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
using System;

namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public enum MSAALevel
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
    public enum PointFigure
    {
        Rect,
        Ellipse,
        Cross,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MeshTopologyEnum
    {
        PNTriangles,
        PNQuads
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ShaderStage
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
    public enum StateType
    {
        None = 0,
        RasterState = 1,
        DepthStencilState = 1 << 2,
        BlendState = 1 << 3
    }
}
