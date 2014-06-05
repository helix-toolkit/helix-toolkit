namespace MvvmCameraDemo.ViewModels
{
    using Caliburn.Micro;
    using System.Windows.Media.Media3D;

    public class ShellViewModel : Screen
    {
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    RaisePropertyChangedEventImmediately("Title");
                }
            }
        }


        public ViewportViewModel Viewport1 { get; set; }
        public ViewportViewModel Viewport2 { get; set; }

        PerspectiveCamera camera;

        public ShellViewModel()
        {
            Title = "Hello Caliburn.Micro";
            Viewport1 = new ViewportViewModel();
            Viewport2 = new ViewportViewModel();

            camera = new PerspectiveCamera()
            {
                Position = new Point3D(0, -10, 0),
                LookDirection = new Vector3D(0, 10, 0),
                UpDirection = new Vector3D(0, 0, 1),
                FieldOfView = 60,
            };

            Viewport1.Camera = camera;
            Viewport2.Camera = camera;
        }
    }
}
