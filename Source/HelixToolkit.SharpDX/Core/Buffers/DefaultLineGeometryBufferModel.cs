using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public class DefaultLineGeometryBufferModel : LineGeometryBufferModel<LinesVertex>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultLineGeometryBufferModel"/> class.
    /// </summary>
    public DefaultLineGeometryBufferModel() : base(LinesVertex.SizeInBytes) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultLineGeometryBufferModel"/> class.
    /// </summary>
    /// <param name="isDynamic"></param>
    public DefaultLineGeometryBufferModel(bool isDynamic) : base(LinesVertex.SizeInBytes, isDynamic) { }

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
        // -- set geometry if given
        if (geometry != null && geometry.Positions != null && geometry.Positions.Count > 0)
        {
            // --- get geometry
            var data = OnBuildVertexArray(geometry);
            buffer.UploadDataToBuffer(context, data, geometry.Positions.Count, 0, geometry.PreDefinedVertexCount);
        }
        else
        {
            //buffer.DisposeAndClear();
            buffer.UploadDataToBuffer(context, emptyVertices, 0);
        }
    }

    protected override bool IsVertexBufferChanged(string propertyName, int vertexBufferIndex)
    {
        return base.IsVertexBufferChanged(propertyName, vertexBufferIndex) || propertyName.Equals(nameof(Geometry3D.Colors));
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
        if (geometry != null && geometry.Indices != null && geometry.Indices.Count > 0)
        {
            buffer.UploadDataToBuffer(context, geometry.Indices, geometry.Indices.Count, 0, geometry.PreDefinedIndexCount);
        }
        else
        {
            buffer.UploadDataToBuffer(context, emptyIndices, 0);
        }
    }

    /// <summary>
    /// Called when [build vertex array].
    /// </summary>
    /// <param name="geometry">The geometry.</param>
    /// <returns></returns>
    private LinesVertex[] OnBuildVertexArray(Geometry3D geometry)
    {
        var positions = geometry.Positions;
        var vertexCount = geometry.Positions?.Count ?? 0;
        var array = ThreadBufferManager<LinesVertex>.GetBuffer(vertexCount);
        var colors = geometry.Colors != null ? geometry.Colors.GetEnumerator() : Enumerable.Repeat(Color4.White, vertexCount).GetEnumerator();

        for (var i = 0; i < vertexCount; i++)
        {
            colors.MoveNext();
            array[i].Position = new Vector4(positions![i], 1f);
            array[i].Color = colors.Current;
        }
        colors.Dispose();
        return array;
    }
}
