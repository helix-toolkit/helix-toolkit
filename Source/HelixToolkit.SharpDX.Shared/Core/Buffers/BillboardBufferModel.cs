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
    using System.IO;
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

        private Stream textureStream;
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
            if(geometry is IBillboardText billboardGeometry)
            {
                billboardGeometry.DrawTexture(deviceResources);
                if (billboardGeometry.BillboardVertices != null && billboardGeometry.BillboardVertices.Count > 0)
                {
                    Type = billboardGeometry.Type;              
                    buffer.UploadDataToBuffer(context, billboardGeometry.BillboardVertices, billboardGeometry.BillboardVertices.Count, 0, geometry.PreDefinedVertexCount);
                    if(textureStream != billboardGeometry.Texture)
                    {
                        RemoveAndDispose(ref textureView);
                        textureStream = billboardGeometry.Texture;
                        if (textureStream != null)
                        {
                            textureView = Collect(deviceResources.MaterialTextureManager.Register(textureStream));
                        }
                    }
                }
                else
                {
                    RemoveAndDispose(ref textureView);
                    textureStream = null;
                    buffer.UploadDataToBuffer(context, emptyVerts, 0);
                }
            }
        }
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
