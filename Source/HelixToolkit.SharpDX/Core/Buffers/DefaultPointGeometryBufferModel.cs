﻿using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public class DefaultPointGeometryBufferModel : PointGeometryBufferModel<PointsVertex>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultPointGeometryBufferModel"/> class.
    /// </summary>
    public DefaultPointGeometryBufferModel() : base(PointsVertex.SizeInBytes) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultPointGeometryBufferModel"/> class.
    /// </summary>
    /// <param name="isDynamic"></param>
    public DefaultPointGeometryBufferModel(bool isDynamic) : base(PointsVertex.SizeInBytes, isDynamic) { }

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
            buffer.UploadDataToBuffer(context, emptyVerts, 0);
        }
    }


    protected override bool IsVertexBufferChanged(string propertyName, int vertexBufferIndex)
    {
        return base.IsVertexBufferChanged(propertyName, vertexBufferIndex) || propertyName.Equals(nameof(Geometry3D.Colors));
    }
    /// <summary>
    /// Called when [build vertex array].
    /// </summary>
    /// <param name="geometry">The geometry.</param>
    /// <returns></returns>
    private PointsVertex[] OnBuildVertexArray(Geometry3D geometry)
    {
        var positions = geometry.Positions;
        var vertexCount = geometry.Positions?.Count ?? 0;
        var array = ThreadBufferManager<PointsVertex>.GetBuffer(vertexCount);
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
