namespace BuildingDemo
{
    using System.ComponentModel;

    using PropertyTools.DataAnnotations;

    public class HouseVisualDecorator
    {
        private readonly HouseVisual3D house;

        public HouseVisualDecorator(HouseVisual3D house)
        {
            this.house = house;
        }

        [Category("House properties")]
        [Slidable(0, 60)]
        public double Width { get { return this.house.Width; } set { this.house.Width = value; } }

        [Slidable(0, 60)]
        public double Length { get { return this.house.Length; } set { this.house.Length = value; } }

        [Slidable(0, 4)]
        [FormatString("0.00")]
        public double StoryHeight { get { return this.house.StoryHeight; } set { this.house.StoryHeight = value; } }

        [Slidable(1, 8)]
        public int Stories { get { return this.house.Stories; } set { this.house.Stories = value; } }

        [Slidable(0, 60)]
        public double RoofAngle { get { return this.house.RoofAngle; } set { this.house.RoofAngle = value; } }

        [Slidable(0, 2)]
        [FormatString("0.00")]
        public double RoofThickness { get { return this.house.RoofThickness; } set { this.house.RoofThickness = value; } }

        [Slidable(0, 1)]
        [FormatString("0.00")]
        public double FloorThickness { get { return this.house.FloorThickness; } set { this.house.FloorThickness = value; } }
    }
}