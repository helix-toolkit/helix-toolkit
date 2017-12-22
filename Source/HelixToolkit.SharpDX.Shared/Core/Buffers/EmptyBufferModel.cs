/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using System;
    using Utilities;

    public sealed class EmptyGeometryBufferModel : IGeometryBufferModel
    {
        public Geometry3D Geometry
        {
            set;get;
        }

        public Guid GUID
        {
            get;
        } = Guid.NewGuid();

        public IBufferProxy IndexBuffer
        {
            get
            {
                return null;
            }
        }

        public PrimitiveTopology Topology
        {
            get
            {
                return PrimitiveTopology.Undefined;
            }
            set { }
        }

        public IBufferProxy VertexBuffer
        {
            get
            {
                return null;
            }
        }

        public event EventHandler<bool> InvalidateRenderer;

        public void Attach()
        {

        }

        public bool AttachBuffers(DeviceContext context, InputLayout vertexLayout, int vertexBufferSlot)
        {
            return true;
        }

        public void Detach()
        {

        }
    }
}
