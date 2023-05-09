// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpDirectionViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a ViewModel for the Main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace UpDirectionDemo
{
    public class UpDirectionViewModel : INotifyPropertyChanged
    {
        Vector3D _upModel = new Vector3D(0, 1, 0);
        public Vector3D UpModel
        {
            get => _upModel;
            set
            {
                _upModel = value;

            }
        }
        public double X
        {
            get { return _upModel.X; }
            set
            {
                _upModel.X = value;
                RaisePropertyChanged(nameof(X));
                RaisePropertyChanged(nameof(UpModel));

            }
        }
        public double Y
        {

            get { return _upModel.Y; }
            set
            {
                _upModel.Y = value;
                RaisePropertyChanged(nameof(Y));
                RaisePropertyChanged(nameof(UpModel));

            }
        }
        public double Z
        {
            get { return _upModel.Z; }
            set
            {
                _upModel.Z = value;
                RaisePropertyChanged(nameof(Z));
                RaisePropertyChanged(nameof(UpModel));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
