using global::SharpDX;

namespace HelixToolkit.Wpf.SharpDX
{
    public static class BoundingSphereExtensions
    {

        public static global::SharpDX.BoundingSphere TransformBoundingSphere(this global::SharpDX.BoundingSphere b, Matrix m)
        {
            var center = b.Center;
            var edge = b.Center + Vector3.Right * b.Radius;

            var worldCenter = Vector3.Transform(center, m);
            var worldEdge = Vector3.Transform(edge, m);

            return new global::SharpDX.BoundingSphere(worldCenter.ToXYZ(), (worldEdge - worldCenter).Length());
        }
    }
}
