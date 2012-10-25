namespace HelixToolkit.SharpDX.Wpf
{
    using global::SharpDX;

    public class DiffuseMaterial : Material
    {
        public Color Color { get; set; }

        public DiffuseMaterial()
        {
            this.Color = Color.Blue;
        }
    }
}