// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MvvmCameraDemo
{
    using System.Windows.Media.Media3D;

    public class ShellViewModel : Observable
    {
        private string title;
        public string Title
        {
            get { return this.title; }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.OnPropertyChanged();
                }
            }
        }


        public ViewportViewModel Viewport1 { get; set; }
        public ViewportViewModel Viewport2 { get; set; }

        public ShellViewModel()
        {
            this.Title = "MvvmCameraDemo";
            this.Viewport1 = new ViewportViewModel();
            this.Viewport2 = new ViewportViewModel();

            var camera = new PerspectiveCamera()
            {
                Position = new Point3D(0, -10, 0),
                LookDirection = new Vector3D(0, 10, 0),
                UpDirection = new Vector3D(0, 0, 1),
                FieldOfView = 60,
            };

            this.Viewport1.Camera = camera;
            this.Viewport2.Camera = camera;
        }
    }
}