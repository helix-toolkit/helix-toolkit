namespace BuildingDemo
{
    using System.ComponentModel;

    using PropertyTools.DataAnnotations;

    public class SiloVisualDecorator
    {
        private readonly SiloVisual3D silo;

        public SiloVisualDecorator(SiloVisual3D silo)
        {
            this.silo = silo;
        }

        [Category("Silo/tank properties")]
        [Slidable(0, 100)]
        [FormatString("0.00")]
        public double Diameter { get { return this.silo.Diameter; } set { this.silo.Diameter = value; } }

        [Slidable(0, 100)]
        [FormatString("0.00")]
        public double Height { get { return this.silo.Height; } set { this.silo.Height = value; } }

        [Slidable(0, 20)]
        [FormatString("0.00")]
        public double DomeHeight { get { return this.silo.DomeHeight; } set { this.silo.DomeHeight = value; } }

        [Slidable(0, 50)]
        [FormatString("0.00")]
        public double DomeDiameter { get { return this.silo.DomeDiameter; } set { this.silo.DomeDiameter = value; } }
    }
}