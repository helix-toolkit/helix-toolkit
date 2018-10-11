// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
 namespace Workitem10048
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Extensions;

    public class MainViewModel : BaseViewModel
    {
        private static readonly Point3D NoHit = new Point3D(double.NaN, double.NaN, double.NaN);

        private Point3D pointHit = NoHit;

        public Point3D PointHit
        {
            get
            {
                return this.pointHit;
            }

            set
            {
                if (this.pointHit != value)
                {
                    this.pointHit = value;
                    this.OnPropertyChanged(nameof(this.PointHit));
                }
            }
        }

        public MainViewModel()
        {
            // titles
            this.Title = "Simple Demo (Workitem 10048 and 10052)";
            this.SubTitle = "Select lines with left mouse button.\nRotate or zoom around a point on a line if the cursor is above one.";

            EffectsManager = new DefaultEffectsManager();
        }

        public void OnMouseDown3D(object sender, RoutedEventArgs e)
        {
            this.PointHit = (e as MouseDown3DEventArgs)?.HitTestResult?.PointHit.ToPoint3D() ?? NoHit;
        }
    }
}