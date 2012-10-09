namespace HelixToolkit.SharpDX.Wpf
{
    using global::SharpDX;

    public class RenderContext
    {
        internal Matrix worldMatrix;

        internal Matrix viewMatrix;

        internal Matrix projectionMatrix;

        // lights?

        public RenderContext(Camera camera, DPFCanvas canvas, Matrix worldMatrix)
        {
            this.worldMatrix = worldMatrix;
            this.viewMatrix = camera.CreateViewMatrix();
            var aspectRatio = canvas.ActualWidth / canvas.ActualHeight;
            this.projectionMatrix = camera.CreateProjectionMatrix(aspectRatio);
        }
    }
}