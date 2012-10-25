namespace HelixToolkit.SharpDX.Wpf
{
    using global::SharpDX;

    /// <summary>
    /// Provides a base class for 
    /// </summary>
    public abstract class Model3D : Element3D
    {
        public Matrix Transform { get; set; }
    }
}