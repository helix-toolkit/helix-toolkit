namespace HelixToolkit.SharpDX.Core.Buffers;

public class VertexBoneIdBufferModel<T> : ElementsBufferModel<T> where T : unmanaged
{
    public VertexBoneIdBufferModel(int structSize) : base(structSize) { }
}
