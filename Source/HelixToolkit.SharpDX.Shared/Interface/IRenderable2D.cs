/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct2D1;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public interface ITransform2D
    {
        Matrix3x2 Transform { set; get; }
    }
    public interface IRenderable2D : IDisposable
    {
        RectangleF Bound { set; get; }
        //Matrix3x2 Transform { set; get; }
        bool IsRendering { set; get; }
        void Attach(IRenderHost target);
        void Detach();
        void Render(IRenderContext2D context);
        bool IsMouseOver { set; get; }
    }
}
