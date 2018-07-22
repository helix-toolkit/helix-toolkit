/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX.Direct3D;
using global::SharpDX.Direct3D11;
using global::SharpDX.DXGI;
using System;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="VertexStruct">The type of the ertex structure.</typeparam>
    public abstract class BillboardBufferModel<VertexStruct> : GeometryBufferModel, IBillboardBufferModel where VertexStruct : struct
    {
        private static readonly VertexStruct[] emptyVerts = new VertexStruct[0];

        /// <summary>
        /// Use the shared texture resource proxy
        /// </summary>
        private ShaderResourceViewProxy textureView;
        /// <summary>
        /// Gets the texture view.
        /// </summary>
        /// <value>
        /// The texture view.
        /// </value>
        public ShaderResourceViewProxy TextureView { get { return textureView; } }
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
        /// <param name="dynamic"></param>
        public BillboardBufferModel(int structSize, bool dynamic = false)
            : base(PrimitiveTopology.PointList,
                  dynamic ? new DynamicBufferProxy(structSize, BindFlags.VertexBuffer) : new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer) as IElementsBufferProxy,
                  null)
        {
        }
        /// <summary>
        /// Called when [create index buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        protected override void OnCreateIndexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, Geometry3D geometry, IDeviceResources deviceResources)
        {

        }
        /// <summary>
        /// Called when [create vertex buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        /// <param name="bufferIndex"></param>
        protected override void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D geometry, IDeviceResources deviceResources)
        {           
            var billboardGeometry = geometry as IBillboardText;
            billboardGeometry.DrawTexture(deviceResources);
            if (billboardGeometry != null && billboardGeometry.BillboardVertices != null && billboardGeometry.BillboardVertices.Count > 0)
            {
                Type = billboardGeometry.Type;              
                buffer.UploadDataToBuffer(context, billboardGeometry.BillboardVertices, billboardGeometry.BillboardVertices.Count, 0, geometry.PreDefinedVertexCount);
                RemoveAndDispose(ref textureView);
                if (billboardGeometry.Texture != null)
                {
                    textureView = Collect(deviceResources.MaterialTextureManager.Register(billboardGeometry.Texture));
                }
            }
            else
            {
                textureView = null;
                buffer.UploadDataToBuffer(context, emptyVerts, 0);
            }
        }

        ///// <summary>
        ///// Called when [attach buffer].
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <param name="vertexLayout">The vertex layout.</param>
        ///// <param name="vertexBufferStartSlot">The vertex buffer start slot. Returns next available bind slot</param>
        ///// <returns></returns>
        //protected override bool OnAttachBuffer(DeviceContextProxy context, InputLayout vertexLayout, ref int vertexBufferStartSlot)
        //{
        //    context.PrimitiveTopology = Topology;
        //    context.InputLayout = vertexLayout;
        //    if (VertexBuffer.Length > 0)
        //    {
        //        context.SetVertexBuffers(vertexBufferStartSlot, VertexBuffer.Select(x=> new VertexBufferBinding(x.Buffer, x.StructureSize, x.Offset)).ToArray());
        //        vertexBufferStartSlot += VertexBuffer.Length;
        //    }
        //    else
        //    {
        //        context.SetIndexBuffer(null, Format.Unknown, 0);
        //    }
        //    return true;
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class DefaultBillboardBufferModel : BillboardBufferModel<BillboardVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBillboardBufferModel"/> class.
        /// </summary>
        public DefaultBillboardBufferModel() : base(BillboardVertex.SizeInBytes) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class DynamicBillboardBufferModel : BillboardBufferModel<BillboardVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicBillboardBufferModel"/> class.
        /// </summary>
        public DynamicBillboardBufferModel() : base(BillboardVertex.SizeInBytes, true) { }
    }
}
