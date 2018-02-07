/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Linq;
using global::SharpDX.Direct3D;
using global::SharpDX.Direct3D11;
using global::SharpDX.DXGI;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="VertexStruct">The type of the ertex structure.</typeparam>
    public abstract class BillboardBufferModel<VertexStruct> : GeometryBufferModel, IBillboardBufferModel where VertexStruct : struct
    {
        /// <summary>
        /// Called when [build vertex array].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected abstract VertexStruct[] OnBuildVertexArray(IBillboardText geometry, IDeviceResources deviceResources);

        private ShaderResourceView textureView;
        /// <summary>
        /// Gets the texture view.
        /// </summary>
        /// <value>
        /// The texture view.
        /// </value>
        public ShaderResourceView TextureView { get { return textureView; } }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public BillboardType Type { private set; get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="structSize">Size of the structure.</param>
        public BillboardBufferModel(int structSize)
            : base(PrimitiveTopology.PointList, new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer), null)
        {
        }
        /// <summary>
        /// Called when [create index buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        protected override void OnCreateIndexBuffer(DeviceContext context, IElementsBufferProxy buffer, Geometry3D geometry, IDeviceResources deviceResources)
        {

        }
        /// <summary>
        /// Called when [create vertex buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        protected override void OnCreateVertexBuffer(DeviceContext context, IElementsBufferProxy buffer, Geometry3D geometry, IDeviceResources deviceResources)
        {
            RemoveAndDispose(ref textureView);
            var billboardGeometry = geometry as IBillboardText;
            
            if (billboardGeometry != null && billboardGeometry.BillboardVertices != null)
            {
                Type = billboardGeometry.Type;              
                var data = OnBuildVertexArray(billboardGeometry, deviceResources);
                buffer.UploadDataToBuffer(context, data, billboardGeometry.BillboardVertices.Count);
              
                if (billboardGeometry.Texture != null)
                {
                    textureView = Collect(global::SharpDX.Toolkit.Graphics.Texture.Load(context.Device, billboardGeometry.Texture));
                }
            }
            else
            {
                buffer.DisposeAndClear();
            }
        }
        /// <summary>
        /// Called when [attach buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="vertexLayout">The vertex layout.</param>
        /// <param name="vertexBufferSlot">The vertex buffer slot.</param>
        /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    public sealed class DefaultBillboardBufferModel : BillboardBufferModel<BillboardVertex>
    {
        [ThreadStatic]
        private static BillboardVertex[] vertexArrayBuffer;
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBillboardBufferModel"/> class.
        /// </summary>
        public DefaultBillboardBufferModel() : base(BillboardVertex.SizeInBytes) { }

        /// <summary>
        /// Called when [build vertex array].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected override BillboardVertex[] OnBuildVertexArray(IBillboardText geometry, IDeviceResources deviceResources)
        {
            // Gather all of the textInfo offsets.
            // These should be equal in number to the positions.
            geometry.DrawTexture(deviceResources);

            var vertexCount = geometry.BillboardVertices.Count;
            var array = vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new BillboardVertex[vertexCount];

            vertexArrayBuffer = array;

            for (var i = 0; i < vertexCount; i++)
            {
                array[i] = geometry.BillboardVertices[i];
            }

            return array;
        }
    }
}
