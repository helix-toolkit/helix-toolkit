namespace HelixToolkit.SharpDX
{
    using global::SharpDX;

    public class OrthographicCamera : ProjectionCamera
    {
        public double Width { get; set; }

        public override Matrix CreateProjectionMatrix(double aspectRatio)
        {
            if (this.CreateLeftHandSystem)
            {
                return Matrix.OrthoLH((float)this.Width, (float)(this.Width * aspectRatio), (float)this.NearPlaneDistance, (float)this.FarPlaneDistance);
            }

            return Matrix.OrthoRH((float)this.Width, (float)(this.Width * aspectRatio), (float)this.NearPlaneDistance, (float)this.FarPlaneDistance);
        }
    }
}