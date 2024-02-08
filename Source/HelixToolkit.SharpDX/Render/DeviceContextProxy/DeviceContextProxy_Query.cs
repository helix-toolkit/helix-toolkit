using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Render;

public partial class DeviceContextProxy
{
    public Query CreateQuery(in QueryDescription description)
    {
        return new Query(device, description);
    }

    public Predicate CreatePredicate(in QueryDescription description)
    {
        return new Predicate(device, description);
    }

    public void Begin(Asynchronous query)
    {
        deviceContext?.Begin(query);
    }

    public void End(Asynchronous query)
    {
        deviceContext?.End(query);
    }

    public bool GetData<T>(Asynchronous query, out T dataOut) where T : unmanaged
    {
        dataOut = default;
        return deviceContext?.GetData<T>(query, out dataOut) ?? false;
    }

    public bool IsDataAvailable(Asynchronous query)
    {
        return deviceContext?.IsDataAvailable(query) ?? false;
    }
}
