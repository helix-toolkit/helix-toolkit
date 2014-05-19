// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace ScatterPlotDemo
{
    using System.Linq;

    public class MainViewModel : INotifyPropertyChanged
    {
        public Point3D[] Data { get; set; }

        public double[] Values { get; set; }

        public Model3DGroup Lights
        {
            get
            {
                var group = new Model3DGroup();
                group.Children.Add(new AmbientLight(Colors.White));
                return group;
            }
        }

        public Brush SurfaceBrush
        {
            get
            {
                // return BrushHelper.CreateGradientBrush(Colors.White, Colors.Blue);
                return GradientBrushes.RainbowStripes;
                // return GradientBrushes.BlueWhiteRed;
            }
        }

        public MainViewModel()
        {
            UpdateModel();
        }

        private void UpdateModel()
        {
            Data = Enumerable.Range(0, 7 * 7 * 7).Select(i => new Point3D(i % 7, (i % 49) / 7, i / 49)).ToArray();

            var rnd = new Random();
            this.Values = Data.Select(d => rnd.NextDouble()).ToArray();

            RaisePropertyChanged("Data");
            RaisePropertyChanged("Values");
            RaisePropertyChanged("SurfaceBrush");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}