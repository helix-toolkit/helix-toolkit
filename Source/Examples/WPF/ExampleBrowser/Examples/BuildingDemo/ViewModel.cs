namespace BuildingDemo
{
    using System.Windows.Media.Media3D;

    using PropertyTools;

    public class ViewModel : Observable
    {
        private Visual3D selectedVisual;

        private object selectedObject;

        public Visual3D SelectedVisual
        {
            get
            {
                return this.selectedVisual;
            }

            set
            {
                this.SetValue(ref this.selectedVisual, value, () => this.SelectedVisual);
            }
        }

        public object SelectedObject
        {
            get
            {
                return this.selectedObject;
            }

            set
            {
                this.SetValue(ref this.selectedObject, value, () => this.SelectedObject);
            }
        }

        public void Select(Visual3D visual)
        {
            this.SelectedVisual = visual;
            if (visual != null)
            {
                this.SelectedObject = this.CreateDecorator(visual);
            }
            else
            {
                this.SelectedObject = null;
            }
        }

        private object CreateDecorator(Visual3D visual)
        {
            var house = visual as HouseVisual3D;
            if (house != null)
            {
                return new HouseVisualDecorator(house);
            }

            var silo = visual as SiloVisual3D;
            if (silo != null)
            {
                return new SiloVisualDecorator(silo);
            }

            var chimney = visual as ChimneyVisual3D;
            if (chimney != null)
            {
                return new ChimneyVisualDecorator(chimney);
            }

            var fence = visual as FenceVisual3D;
            if (fence != null)
            {
                return new FenceVisualDecorator(fence);
            }

            return null;
        }
    }
}