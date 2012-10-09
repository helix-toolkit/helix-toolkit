namespace HelixToolkit.SharpDX
{
    using global::SharpDX.Direct3D10;    

    public interface IRenderHost
    {
        Device Device { get; }        
    }
}