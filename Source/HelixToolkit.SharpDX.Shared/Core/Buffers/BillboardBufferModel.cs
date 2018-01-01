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
    using global::SharpDX.DXGI;
    using Shaders;
    using System;
    using Utilities;
    public class BillboardBufferModel<VertexStruct> : GeometryBufferModel, IBillboardBufferModel where VertexStruct : struct
    {
        public delegate VertexStruct[] BuildVertexArrayHandler(IBillboardText geometry);
        /// <summary>
        /// Create VertexStruct[] from geometry position etc.
        /// </summary>
        public BuildVertexArrayHandler OnBuildVertexArray;

        private ShaderResourceView textureView;
        public ShaderResourceView TextureView { get { return textureView; } }

        public BillboardType Type { private set; get; }

        public BillboardBufferModel(int structSize)
            : base(PrimitiveTopology.PointList, new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer), null)
        {
        }

        protected override void OnCreateIndexBuffer(DeviceContext context, IElementsBufferProxy buffer, Geometry3D geometry)
        {

        }

        protected override void OnCreateVertexBuffer(DeviceContext context, IElementsBufferProxy buffer, Geometry3D geometry)
        {
            RemoveAndDispose(ref textureView);
            var billboardGeometry = geometry as IBillboardText;
            
            if (billboardGeometry != null && billboardGeometry.BillboardVertices != null)
            {
                Type = billboardGeometry.Type;              
                var data = OnBuildVertexArray(billboardGeometry);
                buffer.UploadDataToBuffer(context, data, billboardGeometry.BillboardVertices.Count);
              
                if (billboardGeometry.Texture != null)
                {
                    textureView = Collect(global::SharpDX.Toolkit.Graphics.Texture.Load(context.Device, billboardGeometry.Texture));
                }
            }
            else
            {
                buffer.Dispose();
            }
        }

        protected override bool OnAttachBuffer(DeviceContext context, InputLayout vertexLayout, int vertexBufferSlot)
        {
            context.InputAssembler.PrimitiveTopology = Topology;
            context.InputAssembler.InputLayout = vertexLayout;
            if (VertexBuffer != null)
            {
                context.InputAssembler.SetVertexBuffers(vertexBufferSlot, new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, VertexBuffer.Offset));
            }
            else
            {
                context.InputAssembler.SetIndexBuffer(null, Format.Unknown, 0);
            }
            return true;
        }
    }
}
