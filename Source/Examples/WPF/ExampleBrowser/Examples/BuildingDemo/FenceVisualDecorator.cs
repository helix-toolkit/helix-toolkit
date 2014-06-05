namespace BuildingDemo
{
    using System.ComponentModel;

    using PropertyTools.DataAnnotations;

    public class FenceVisualDecorator
    {
        private readonly FenceVisual3D fence;

        public FenceVisualDecorator(FenceVisual3D fence)
        {
            this.fence = fence;
        }

        [Category("Fence properties")]
        [Slidable(0.01, 0.2)]
        [FormatString("0.00")]
        public double Diameter { get { return this.fence.Diameter; } set { this.fence.Diameter = value; } }

        [Slidable(0, 2)]
        [FormatString("0.00")]
        public double Height { get { return this.fence.Height; } set { this.fence.Height = value; } }

        [Slidable(0.01, 0.2)]
        [FormatString("0.00")]
        public double MeshSize { get { return this.fence.MeshSize; } set { this.fence.MeshSize = value; } }

        [Slidable(0.2, 10)]
        [FormatString("0.00")]
        public double PoleDistance { get { return this.fence.PoleDistance; } set { this.fence.PoleDistance = value; } }
    }
}