using global::SharpDX.Direct3D;
using global::SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {
        using Render;
        using Utilities;
        public sealed class Sprite2DBufferModel : ReferenceCountDisposeObject, IGUID, IAttachableBufferModel
        {
            public PrimitiveTopology Topology { get; set; } = PrimitiveTopology.TriangleList;

            public IElementsBufferProxy[] VertexBuffer { get; } = new DynamicBufferProxy[1];

            public IEnumerable<int> VertexStructSize { get { return VertexBuffer.Select(x => x != null ? x.StructureSize : 0); } }

            public IElementsBufferProxy IndexBuffer { get; }

            public Guid GUID { get; } = Guid.NewGuid();

            public SpriteStruct[] Sprites { set; get; }
            public int SpriteCount;

            public int[] Indices { set; get; }
            public int IndexCount { set; get; }

            private readonly DynamicBufferProxy vertextBuffer;

            public Sprite2DBufferModel()
            {
                vertextBuffer = Collect(new DynamicBufferProxy(SpriteStruct.SizeInBytes, BindFlags.VertexBuffer));
                VertexBuffer[0] = vertextBuffer;
                IndexBuffer = Collect(new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer));
            }

            public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources deviceResources)
            {
                if(UpdateBuffers(context, deviceResources))
                {
                    context.SetVertexBuffers(0, new VertexBufferBinding(vertextBuffer.Buffer, vertextBuffer.StructureSize, vertextBuffer.Offset));
                    context.SetIndexBuffer(IndexBuffer.Buffer, global::SharpDX.DXGI.Format.R32_UInt, IndexBuffer.Offset);
                    return true;
                }
                return false;
            }

            public bool UpdateBuffers(DeviceContextProxy context, IDeviceResources deviceResources)
            {
                if(SpriteCount == 0 || IndexCount == 0 || Sprites == null || Indices == null || Sprites.Length < SpriteCount || Indices.Length < IndexCount)
                {
                    return false;
                }
                vertextBuffer.UploadDataToBuffer(context, Sprites, SpriteCount);
                IndexBuffer.UploadDataToBuffer(context, Indices, IndexCount);
                return true;
            }
        }
    }

}
