// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewportViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MvvmCameraDemo
{
    using System.Windows.Media.Media3D;

    public class ViewportViewModel : Observable
    {
        PerspectiveCamera _camera;
        public PerspectiveCamera Camera
        {
            get { return _camera; }
            set
            {
                _camera = value;
                this.OnPropertyChanged();
            }
        }
    }
}