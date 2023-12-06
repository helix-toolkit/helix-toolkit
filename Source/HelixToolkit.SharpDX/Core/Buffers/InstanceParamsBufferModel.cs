namespace HelixToolkit.SharpDX.Core;

public class InstanceParamsBufferModel<T> : ElementsBufferModel<T> where T : unmanaged
{
    public InstanceParamsBufferModel(int structSize) : base(structSize)
    {
    }
}
