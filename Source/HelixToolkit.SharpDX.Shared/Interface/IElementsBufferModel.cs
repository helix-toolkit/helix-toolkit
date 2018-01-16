/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Utilities;
    public interface IElementsBufferModel : IVertexExtraBufferModel
    {
        event EventHandler<bool> OnElementChanged;
        bool HasElements { get; }
        void DisposeAndClear();
    }

    public interface IElementsBufferModel<T> : IElementsBufferModel
    {
        IList<T> Elements { get; set; }
    }
}