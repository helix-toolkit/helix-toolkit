using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace DataTemplateDemo
{
    public class ModelElement : Element, INotifyPropertyChanged
    {
        private bool isVisible = true;

        public Model3D Model { get; set; }
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (IsVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
