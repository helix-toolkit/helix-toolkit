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
        bool HasElements { get; }
    }

    public interface IElementsBufferModel<T> : IElementsBufferModel
    {
        IList<T> Elements { get; set; }
    }
}