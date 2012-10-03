using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using System.Diagnostics;

namespace MvvmCameraDemo.ViewModels
{
    public class ViewportViewModel : Screen
    {
        PerspectiveCamera _camera;
        public PerspectiveCamera Camera
        {
            get { return _camera; }
            set
            {
                _camera = value;
                NotifyOfPropertyChange(() => this.Camera);
            }
        }


        public ViewportViewModel()
        {

        }
    }
}
