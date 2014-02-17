// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace SurfaceDemo
{
    public enum ViewMode
    {
        Normal,
        Stereo,
        Anaglyph
    } ;

    public class ViewModel : INotifyPropertyChanged
    {
        Brush _Brush;
        public Brush Brush
        {
            get { return _Brush; }
            set
            {
                _Brush = value;
                OnPropertyChanged("Brush");
            }
        }

        int _MeshSizeU = 140;
        public int MeshSizeU
        {
            get { return _MeshSizeU; }
            set
            {
                _MeshSizeU = value;
                OnPropertyChanged("MeshSizeU");
            }
        }

        int _MeshSizeV = 140;
        public int MeshSizeV
        {
            get { return _MeshSizeV; }
            set
            {
                _MeshSizeV = value;
                OnPropertyChanged("MeshSizeV");
            }
        }

        double _ParameterW = 1;
        public double ParameterW
        {
            get { return _ParameterW; }
            set
            {
                _ParameterW = value;
                OnPropertyChanged("ParameterW");
            }
        }

        double _StereoBase = 0.05;
        public double StereoBase
        {
            get { return _StereoBase; }
            set
            {
                _StereoBase = value;
                OnPropertyChanged("StereoBase");
            }
        }

        ViewMode _ViewMode = SurfaceDemo.ViewMode.Normal;
        public ViewMode ViewMode
        {
            get { return _ViewMode; }
            set
            {
                _ViewMode = value;
                OnPropertyChanged("ViewMode");
            }
        }

        string _ModelTitle;
        public string ModelTitle
        {
            get { return _ModelTitle; }
            set
            {
                _ModelTitle = value;
                OnPropertyChanged("ModelTitle");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}