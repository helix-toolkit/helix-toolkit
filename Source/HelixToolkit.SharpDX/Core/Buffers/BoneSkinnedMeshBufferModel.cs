using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public sealed class BoneSkinnedMeshBufferModel : DefaultMeshGeometryBufferModel, IBoneSkinMeshBufferModel
{
    public event EventHandler? BoneIdBufferUpdated;
    private IElementsBufferProxy? boneIdBuffer;
    public IElementsBufferProxy? BoneIdBuffer => boneIdBuffer;
    private bool boneIdChanged = true;

    public BoneSkinnedMeshBufferModel()
    {
        boneIdBuffer = new ImmutableBufferProxy(BoneIds.SizeInBytes, BindFlags.VertexBuffer, ResourceOptionFlags.None);
    }

    protected override bool IsVertexBufferChanged(string propertyName, int bufferIndex)
    {
        if (propertyName.Equals(nameof(BoneSkinnedMeshGeometry3D.VertexBoneIds)))
        {
            boneIdChanged = true;
            return false;
        }
        else
        {
            return base.IsVertexBufferChanged(propertyName, bufferIndex);
        }
    }

    public override bool UpdateBuffers(DeviceContextProxy context, IDeviceResources? deviceResources)
    {
        if (boneIdChanged && BoneIdBuffer is not null)
        {
            lock (BoneIdBuffer)
            {
                if (boneIdChanged)
                {
                    if (Geometry is BoneSkinnedMeshGeometry3D boneMesh
                        && boneMesh.VertexBoneIds != null && boneMesh.VertexBoneIds.Count == boneMesh.Positions?.Count)
                    {
                        BoneIdBuffer.UploadDataToBuffer(context, boneMesh.VertexBoneIds, boneMesh.VertexBoneIds.Count);
                    }
                    else
                    {
                        BoneIdBuffer.UploadDataToBuffer(context, Array.Empty<BoneIds>(), 0);
                    }
                    boneIdChanged = false;
                    BoneIdBufferUpdated?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        return base.UpdateBuffers(context, deviceResources);
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref boneIdBuffer);
        base.OnDispose(disposeManagedResources);
    }
}
