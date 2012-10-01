namespace HelixToolkit.SharpDX
{
    using global::SharpDX;

    public class PerspectiveCamera : ProjectionCamera
    {
        public double FieldOfView { get; set; }

        public PerspectiveCamera()
        {
            this.FieldOfView = 45;
        }

        public override Matrix CreateProjectionMatrix(double aspectRatio)
        {
            if (this.CreateLeftHandSystem)
            {
                return Matrix.PerspectiveFovLH(
                    (float)this.FieldOfView, (float)aspectRatio, (float)this.NearPlaneDistance, (float)this.FarPlaneDistance);
            }

            return Matrix.PerspectiveFovRH(
                (float)this.FieldOfView, (float)aspectRatio, (float)this.NearPlaneDistance, (float)this.FarPlaneDistance);
        }
    }
}