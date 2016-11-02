// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolarSystem3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;

namespace SolarsystemDemo
{
    public class SolarSystem3D : ModelVisual3D, INotifyPropertyChanged
    {
        public double DistanceScale { get; set; }
        public double DiameterScale { get; set; }

        double _days = 0;
        DateTime Time0;

        public double Days
        {
            get { return _days; }
            set { _days = value; OnPropertyChanged("Days"); OnPropertyChanged("Time"); TimeChanged(); }
        }

        public DateTime Time
        {
            get { return Time0.AddDays(_days); }
            set { Days = (value - Time0).TotalDays; }
        }

        private void TimeChanged()
        {
            UpdateModel();
        }

        public SolarSystem3D()
        {
            Time0 = DateTime.Now;
        }

        public void InitModel()
        {
            foreach (Planet3D p in Children)
                p.InitModel(this);
        }

        public void UpdateModel()
        {
            foreach (Planet3D p in Children)
                p.UpdateModel();
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