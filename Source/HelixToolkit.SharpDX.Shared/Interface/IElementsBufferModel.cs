using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public interface IElementsBufferModel : IVertexExtraBufferModel
    {
        bool HasElements { get; }

        void ResetHasElementsVariable();
    }

    public interface IElementsBufferModel<T> : IElementsBufferModel
    {
        IList<T> Elements { get; set; }
    }
}