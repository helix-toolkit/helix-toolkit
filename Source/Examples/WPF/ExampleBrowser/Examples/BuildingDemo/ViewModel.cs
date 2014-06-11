namespace BuildingDemo
{
    using System.Windows.Media.Media3D;

    using PropertyTools;

    public class ViewModel : Observable
    {
        private object selectedObject;

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
            this.SelectedObject = visual;
        }
    }
}