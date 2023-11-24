using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
/// <typeparam name="VertexStruct">The type of the ertex structure.</typeparam>
public abstract class BillboardBufferModel<VertexStruct> : GeometryBufferModel, IBillboardBufferModel where VertexStruct : unmanaged
{
    private static readonly VertexStruct[] emptyVerts = Array.Empty<VertexStruct>();

    /// <summary>
    /// Use the shared texture resource proxy
    /// </summary>
    private ShaderResourceViewProxy? textureView;
    /// <summary>
    /// Gets the texture view.
    /// </summary>
    /// <value>
    /// The texture view.
    /// </value>
    public ShaderResourceViewProxy? TextureView
    {
        get
        {
            return textureView;
        }
    }
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public BillboardType Type
    {
        private set; get;
    }

    private TextureModel? texture;
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
    protected override void OnCreateIndexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, Geometry3D? geometry, IDeviceResources? deviceResources)
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
    protected override void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D? geometry, IDeviceResources? deviceResources)
    {
        if (geometry is IBillboardText billboardGeometry && deviceResources is not null)
        {
            billboardGeometry.DrawTexture(deviceResources);
            if (billboardGeometry.BillboardVertices != null && billboardGeometry.BillboardVertices.Count > 0)
            {
                Type = billboardGeometry.Type;
                buffer.UploadDataToBuffer(context, billboardGeometry.BillboardVertices, billboardGeometry.BillboardVertices.Count, 0, geometry.PreDefinedVertexCount);
                if (texture != billboardGeometry.Texture)
                {
                    texture = billboardGeometry.Texture;
                    var newView = texture == null ?
                        null : deviceResources?.MaterialTextureManager?.Register(texture);
                    RemoveAndDispose(ref textureView);
                    textureView = newView;
                }
            }
            else
            {
                RemoveAndDispose(ref textureView);
                texture = null;
                buffer.UploadDataToBuffer(context, emptyVerts, 0);
            }
        }
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref textureView);
        base.OnDispose(disposeManagedResources);
    }
}
