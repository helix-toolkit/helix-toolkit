namespace HelixToolkit.SharpDX.Wpf
{
    using Color = global::SharpDX.Color;

    public abstract class Light : Element3D
    {
        public Color Color { get; set; }

        public Light()
        {
            Color = Color.White;
        }
    }
}