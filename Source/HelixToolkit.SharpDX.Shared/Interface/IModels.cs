/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.IO;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;
    using global::SharpDX.Direct3D11;
    using System;
    using Utilities;

    public interface IBillboardText
    {
        BillboardType Type { get; }
        Stream Texture { get; }
        void DrawTexture();
        IList<BillboardVertex> BillboardVertices { get; }
        float Width { get; }
        float Height { get; }
    }
    [Flags]
    public enum BillboardType
    {
        SingleText = 1, MultipleText = 2, SingleImage = 4
    }

    public interface ILightsBufferProxy<T> where T : struct
    {
        int BufferSize { get; }
        T[] Lights { get; }
        Color4 AmbientLight { set; get; }
        void UploadToBuffer(IBufferProxy buffer, DeviceContext context);
    }
}
