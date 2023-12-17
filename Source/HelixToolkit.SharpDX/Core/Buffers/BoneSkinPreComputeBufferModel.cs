using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public sealed class BoneSkinPreComputeBufferModel : DisposeObject, IAttachableBufferModel, IBoneSkinPreComputehBufferModel
{
    public PrimitiveTopology Topology
    {
        get => meshBuffer?.Topology ?? PrimitiveTopology.Undefined;
        set
        {
            if (meshBuffer is not null)
            {
                meshBuffer.Topology = value;
            }
        }
    }

    public IElementsBufferProxy?[] VertexBuffer { private set; get; } = Array.Empty<IElementsBufferProxy?>();

    public IEnumerable<int> VertexStructSize
    {
        get
        {
            return VertexBuffer.Select(x => x != null ? x.StructureSize : 0);
        }
    }

    public IElementsBufferProxy? IndexBuffer => meshBuffer?.IndexBuffer;

    public bool CanPreCompute => meshBuffer?.BoneIdBuffer is not null && meshBuffer.BoneIdBuffer.ElementCount != 0;

    public Guid GUID { get; } = new Guid();
    private bool vertexBufferUpdate = true;
    private bool stagingBufferValid = false;
    private IBoneSkinMeshBufferModel? meshBuffer;
    private IElementsBufferProxy? skinnedVertexBuffer;
    private IElementsBufferProxy? originalVertexBuffer;
    private IElementsBufferProxy? skinnedVertexStagingBuffer;

    private VertexBufferBinding[] skinnedOutputBindings = Array.Empty<VertexBufferBinding>();
    private VertexBufferBinding[] vertexBufferBindings = Array.Empty<VertexBufferBinding>();

    /// <summary>
    /// Initializes a new instance of the <see cref="BoneSkinPreComputeBufferModel"/> class.
    /// </summary>
    /// <param name="meshBuffer">The mesh buffer.</param>
    /// <param name="structSize">Size of the structure.</param>
    public BoneSkinPreComputeBufferModel(IBoneSkinMeshBufferModel meshBuffer, int structSize)
    {
        this.meshBuffer = meshBuffer;
        this.meshBuffer.VertexBufferUpdated += MeshBuffer_OnVertexBufferUpdated;
        this.meshBuffer.BoneIdBufferUpdated += MeshBuffer_OnBoneIdBufferUpdated;
        skinnedVertexBuffer = new ImmutableBufferProxy(structSize,
            BindFlags.VertexBuffer | BindFlags.StreamOutput,
            ResourceOptionFlags.None, ResourceUsage.Default);
        skinnedVertexStagingBuffer = new ImmutableBufferProxy(structSize,
            BindFlags.None, CpuAccessFlags.Read, ResourceOptionFlags.BufferStructured,
            ResourceUsage.Staging);
    }

    private void MeshBuffer_OnBoneIdBufferUpdated(object? sender, EventArgs e)
    {
        if (originalVertexBuffer != null && meshBuffer?.BoneIdBuffer is not null)
        {
            skinnedOutputBindings = new VertexBufferBinding[]
            {
                        new VertexBufferBinding(originalVertexBuffer.Buffer, originalVertexBuffer.StructureSize, originalVertexBuffer.Offset),
                        new VertexBufferBinding(meshBuffer.BoneIdBuffer.Buffer, meshBuffer.BoneIdBuffer.StructureSize, meshBuffer.BoneIdBuffer.Offset)
            };
        }
    }

    private void MeshBuffer_OnVertexBufferUpdated(object? sender, EventArgs e)
    {
        vertexBufferUpdate = true;
        stagingBufferValid = false;
    }

    /// <summary>
    /// Binds the skinned vertex buffer to output.
    /// </summary>
    /// <param name="context">The context.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BindSkinnedVertexBufferToOutput(DeviceContextProxy context)
    {
        context.SetVertexBuffers(0, skinnedOutputBindings);
        context.SetIndexBuffer(null, Format.Unknown, 0);
        context.SetStreamOutputTarget(skinnedVertexBuffer?.Buffer, skinnedVertexBuffer?.Offset ?? 0);
        stagingBufferValid = false;
    }

    /// <summary>
    /// Uns the bind skinned vertex buffer to gs stream output.
    /// </summary>
    /// <param name="context">The context.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnBindSkinnedVertexBufferToOutput(DeviceContextProxy context)
    {
        context.SetStreamOutputTarget(null);
    }

    /// <summary>
    /// Attaches the buffers.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="vertexBufferStartSlot">The vertex buffer start slot.</param>
    /// <param name="deviceResources">The device resources.</param>
    /// <returns></returns>
    public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources? deviceResources)
    {
        UpdateBuffers(context, deviceResources);
        if (VertexBuffer.Length > 0)
        {
            if (VertexBuffer.Length == vertexBufferBindings.Length)
            {
                context.SetVertexBuffers(vertexBufferStartSlot, vertexBufferBindings);
                vertexBufferStartSlot += VertexBuffer.Length;
            }
            else
            {
                return false;
            }
        }
        if (IndexBuffer != null)
        {
            context.SetIndexBuffer(IndexBuffer.Buffer, Format.R32_UInt, IndexBuffer.Offset);
        }
        else
        {
            context.SetIndexBuffer(null, Format.Unknown, 0);
        }
        context.PrimitiveTopology = Topology;
        return true;
    }

    /// <summary>
    /// Updates the buffers.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="deviceResources">The device resources.</param>
    /// <returns></returns>
    public bool UpdateBuffers(DeviceContextProxy context, IDeviceResources? deviceResources)
    {
        if (meshBuffer is null)
        {
            return false;
        }

        var updated = false;
        if (meshBuffer.UpdateBuffers(context, deviceResources) || vertexBufferUpdate)
        {
            if (skinnedVertexBuffer is null)
            {
                return false;
            }

            lock (skinnedVertexBuffer)
            {
                if (vertexBufferUpdate)
                {
                    if (meshBuffer.VertexBuffer.Length > 0)
                    {
                        VertexBuffer = meshBuffer.VertexBuffer.ToArray();
                        originalVertexBuffer = VertexBuffer[0];
                        if (skinnedVertexBuffer.Buffer == null || skinnedVertexBuffer.ElementCount != originalVertexBuffer!.ElementCount)
                        {
                            var array = new float[originalVertexBuffer!.ElementCount * originalVertexBuffer.StructureSize];
                            skinnedVertexBuffer.UploadDataToBuffer(context, array, originalVertexBuffer.ElementCount);
                            context.CopyResource(originalVertexBuffer.Buffer!, skinnedVertexBuffer.Buffer!);
                        }
                        VertexBuffer[0] = skinnedVertexBuffer;
                        vertexBufferBindings = VertexBuffer.Select(x => x != null ? new VertexBufferBinding(x.Buffer, x.StructureSize, x.Offset) : new VertexBufferBinding()).ToArray();
                        skinnedOutputBindings = new VertexBufferBinding[]
                        {
                                    new VertexBufferBinding(originalVertexBuffer.Buffer, originalVertexBuffer.StructureSize, originalVertexBuffer.Offset),
                                    new VertexBufferBinding(meshBuffer.BoneIdBuffer!.Buffer, meshBuffer.BoneIdBuffer.StructureSize, meshBuffer.BoneIdBuffer.Offset)
                        };
                    }
                    else
                    {
                        VertexBuffer = Array.Empty<IElementsBufferProxy>();
                        vertexBufferBindings = Array.Empty<VertexBufferBinding>();
                    }
                    vertexBufferUpdate = false;
                    updated = true;
                    stagingBufferValid = false;
                }
            }
        }
        return updated;
    }

    public void ResetSkinnedVertexBuffer(DeviceContextProxy context)
    {
        if (originalVertexBuffer?.Buffer is not null && skinnedVertexBuffer?.Buffer != null && skinnedVertexBuffer.ElementCount == originalVertexBuffer.ElementCount)
        {
            context.CopyResource(originalVertexBuffer.Buffer, skinnedVertexBuffer.Buffer);
        }
    }
    /// <summary>
    /// Copies the skinned to array.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="array">The array.</param>
    /// <returns>Number of vertex has been copied.</returns>
    public int CopySkinnedToArray(DeviceContextProxy context, Vector3[] array)
    {
        if (skinnedVertexBuffer?.Buffer == null)
        {
            return 0;
        }
        if (skinnedVertexStagingBuffer?.Buffer == null || skinnedVertexStagingBuffer.ElementCount != skinnedVertexBuffer.ElementCount)
        {
            skinnedVertexStagingBuffer?.CreateBuffer(context, skinnedVertexBuffer.ElementCount);
            stagingBufferValid = false;
        }
        if (skinnedVertexStagingBuffer?.Buffer != null)
        if (skinnedVertexStagingBuffer?.Buffer != null)
        {
            var size = Math.Min(array.Length, skinnedVertexStagingBuffer.ElementCount);
            if (!stagingBufferValid)
            {
                context.CopyResource(skinnedVertexBuffer.Buffer, skinnedVertexStagingBuffer.Buffer);
                stagingBufferValid = true;
            }
            var box = context.MapSubresource(skinnedVertexStagingBuffer.Buffer, MapMode.Read,
                global::SharpDX.Direct3D11.MapFlags.None);
            if (box is not null)
            {
                unsafe
                {
                    var p = (byte*)box.Value.DataPointer;
                    for (var i = 0; i < size; ++i)
                    {
                        array[i] = *(Vector3*)p;
                        p += skinnedVertexStagingBuffer.StructureSize;
                    }
                }
                context.UnmapSubresource(skinnedVertexStagingBuffer.Buffer, 0);
            }
            return size;
        }
        return 0;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        if (meshBuffer is not null)
        {
            meshBuffer.BoneIdBufferUpdated -= MeshBuffer_OnBoneIdBufferUpdated;
            meshBuffer.VertexBufferUpdated -= MeshBuffer_OnVertexBufferUpdated;
        }

        RemoveAndDispose(ref meshBuffer);
        RemoveAndDispose(ref skinnedVertexBuffer);
        RemoveAndDispose(ref skinnedVertexStagingBuffer);
        base.OnDispose(disposeManagedResources);
    }
}
