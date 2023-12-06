using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public class DefaultMeshGeometryBufferModel : MeshGeometryBufferModel<DefaultVertex>
{
    private static readonly Vector2[] emptyTextureArray = Array.Empty<Vector2>();
    private static readonly Vector4[] emptyColorArray = Array.Empty<Vector4>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMeshGeometryBufferModel"/> class.
    /// </summary>
    public DefaultMeshGeometryBufferModel()
        : base(PrimitiveTopology.TriangleList,
              new[]
              {
                          new ImmutableBufferProxy(DefaultVertex.SizeInBytes, BindFlags.VertexBuffer),
                          new ImmutableBufferProxy(Vector2.SizeInBytes, BindFlags.VertexBuffer),
                          new ImmutableBufferProxy(Vector4.SizeInBytes, BindFlags.VertexBuffer)
              } as IElementsBufferProxy[])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMeshGeometryBufferModel"/> class.
    /// </summary>
    /// <param name="buffers">The buffers.</param>
    /// <param name="isDynamic"></param>
    public DefaultMeshGeometryBufferModel(IElementsBufferProxy[] buffers, bool isDynamic)
        : base(PrimitiveTopology.TriangleList, buffers, isDynamic)
    {
    }
    /// <summary>
    /// Determines whether [is vertex buffer changed] [the specified property name].
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="bufferIndex"></param>
    /// <returns>
    ///   <c>true</c> if [is vertex buffer changed] [the specified property name]; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsVertexBufferChanged(string propertyName, int bufferIndex)
    {
        switch (bufferIndex)
        {
            case 0:
                return base.IsVertexBufferChanged(propertyName, bufferIndex);
            case 1:
                return propertyName.Equals(nameof(MeshGeometry3D.TextureCoordinates));
            case 2:
                return propertyName.Equals(nameof(MeshGeometry3D.Colors));
            default:
                return false;
        }
    }
    /// <summary>
    /// Called when [create vertex buffer].
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="bufferIndex">Index of the buffer.</param>
    /// <param name="geometry">The geometry.</param>
    /// <param name="deviceResources">The device resources.</param>
    protected override void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D? geometry, IDeviceResources? deviceResources)
    {
        if (geometry is MeshGeometry3D mesh)
        {
            switch (bufferIndex)
            {
                case 0:
                    // -- set geometry if given
                    if (geometry.Positions != null && geometry.Positions.Count > 0)
                    {
                        // --- get geometry
                        var data = BuildVertexArray(mesh);
                        buffer.UploadDataToBuffer(context, data, geometry.Positions.Count, 0, geometry.PreDefinedVertexCount);
                    }
                    else
                    {
                        //buffer.DisposeAndClear();
                        buffer.UploadDataToBuffer(context, emptyVerts, 0);
                    }
                    break;
                case 1:
                    if (mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count > 0)
                    {
                        buffer.UploadDataToBuffer(context, mesh.TextureCoordinates, mesh.TextureCoordinates.Count, 0, geometry.PreDefinedVertexCount);
                    }
                    else
                    {
                        buffer.UploadDataToBuffer(context, emptyTextureArray, 0);
                    }
                    break;
                case 2:
                    if (geometry.Colors != null && geometry.Colors.Count > 0)
                    {
                        buffer.UploadDataToBuffer(context, geometry.Colors, geometry.Colors.Count, 0, geometry.PreDefinedVertexCount);
                    }
                    else
                    {
                        buffer.UploadDataToBuffer(context, emptyColorArray, 0);
                    }
                    break;
            }
        }
    }
    /// <summary>
    /// Builds the vertex array.
    /// </summary>
    /// <param name="geometry">The geometry.</param>
    /// <returns></returns>
    private DefaultVertex[] BuildVertexArray(MeshGeometry3D geometry)
    {
        //var geometry = this.geometryInternal as MeshGeometry3D;
        var positions = geometry.Positions!.GetEnumerator();
        var vertexCount = geometry.Positions.Count;

        var normals = geometry.Normals != null ? geometry.Normals.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
        var tangents = geometry.Tangents != null ? geometry.Tangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
        var bitangents = geometry.BiTangents != null ? geometry.BiTangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();

        var array = ThreadBufferManager<DefaultVertex>.GetBuffer(vertexCount);
        for (var i = 0; i < vertexCount; i++)
        {
            positions.MoveNext();
            normals.MoveNext();
            tangents.MoveNext();
            bitangents.MoveNext();
            array[i].Position = new Vector4(positions.Current, 1f);
            array[i].Normal = normals.Current;
            array[i].Tangent = tangents.Current;
            array[i].BiTangent = bitangents.Current;
        }
        normals.Dispose();
        tangents.Dispose();
        bitangents.Dispose();
        positions.Dispose();
        return array;
    }
}
