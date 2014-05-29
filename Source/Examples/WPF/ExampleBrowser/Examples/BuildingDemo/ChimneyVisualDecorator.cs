namespace BuildingDemo
{
    using System.ComponentModel;

    using PropertyTools.DataAnnotations;

    public class ChimneyVisualDecorator
    {
        private readonly ChimneyVisual3D chimney;

        public ChimneyVisualDecorator(ChimneyVisual3D chimney)
        {
            this.chimney = chimney;
        }

        [Category("Chimney properties")]
        [Slidable(0, 20)]
        [FormatString("0.00")]
        public double BaseDiameter { get { return this.chimney.BaseDiameter; } set { this.chimney.BaseDiameter = value; } }

        [Slidable(0, 20)]
        [FormatString("0.00")]
        public double TopDiameter { get { return this.chimney.TopDiameter; } set { this.chimney.TopDiameter = value; } }

        [Slidable(0, 100)]
        [FormatString("0.00")]
        public double Height { get { return this.chimney.Height; } set { this.chimney.Height = value; } }

        [Slidable(1, 20)]
        [FormatString("0.00")]
        public int Bands { get { return this.chimney.Bands; } set { this.chimney.Bands = value; } }
    }
}