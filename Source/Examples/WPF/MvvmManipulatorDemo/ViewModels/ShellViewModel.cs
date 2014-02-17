namespace MvvmManipulatorDemo.ViewModels
{
    using Caliburn.Micro;

    public class ShellViewModel : PropertyChangedBase
    {

        public ShellViewModel()
        {

        }

        private double translateValue;
        public double TranslateValue
        {
            get { return translateValue; }
            set
            {
                if (translateValue != value)
                {
                    translateValue = value;
                    NotifyOfPropertyChange(() => this.TranslateValue);

                }
            }
        }

        private double rotateValue;
        public double RotateValue
        {
            get { return rotateValue; }
            set
            {
                if (rotateValue != value)
                {
                    rotateValue = value;
                    NotifyOfPropertyChange(() => this.RotateValue);
                }
            }
        }
    }
}