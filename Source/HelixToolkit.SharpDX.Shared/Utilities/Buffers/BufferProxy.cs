/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SDX11 = SharpDX.Direct3D11;
using SharpDX.Direct3D11;
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBufferProxy
    {
        /// <summary>
        /// Raw Buffer
        /// </summary>
        SDX11.Buffer Buffer { get; }
        /// <summary>
        /// Element Size
        /// </summary>
        int StructureSize { get; }
        /// <summary>
        /// Element count
        /// </summary>
        int ElementCount { get; }
        /// <summary>
        /// Buffer offset in bytes
        /// </summary>
        int Offset { set; get; }
        /// <summary>
        /// Buffer binding flag
        /// </summary>
        BindFlags BindFlags { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class BufferProxyBase : DisposeObject, IBufferProxy
    {
        /// <summary>
        /// 
        /// </summary>
        protected SDX11.Buffer buffer;
        /// <summary>
        /// <see cref="IBufferProxy.StructureSize"/> 
        /// </summary>
        public int StructureSize { get; private set; }
        /// <summary>
        ///  <see cref="IBufferProxy.ElementCount"/> 
        /// </summary>
        public int ElementCount { get; protected set; } = 0;
        /// <summary>
        /// Buffer data offset in bytes.
        /// <see cref="IBufferProxy.Offset"/> 
        /// </summary>
        public int Offset { get; set; } = 0;
        /// <summary>
        ///  <see cref="IBufferProxy.Buffer"/> 
        /// </summary>
        public SDX11.Buffer Buffer { get { return buffer; } }
        /// <summary>
        ///  <see cref="IBufferProxy.BindFlags"/> 
        /// </summary>
        public BindFlags BindFlags { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="structureSize"></param>
        /// <param name="bindFlags"></param>
        public BufferProxyBase(int structureSize, BindFlags bindFlags)
        {
            StructureSize = structureSize;
            BindFlags = bindFlags;
        }

        public override void DisposeAndClear()
        {
            buffer = null;
            ElementCount = 0;
            base.DisposeAndClear();
        }
    }
}
