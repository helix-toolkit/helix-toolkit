// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2021 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using HelixToolkit.Wpf.SharpDX;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Color = System.Windows.Media.Color;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using System.Collections.Generic;
using System.Diagnostics;
using HelixToolkit.SharpDX.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DynamicPointsAndLines
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        #region INotifyPropertyChanged Support
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string info = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));

        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (object.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                if (EffectsManager != null)
                {
                    var effectManager = EffectsManager as IDisposable;
                    Disposer.RemoveAndDispose(ref effectManager);
                }
                disposedValue = true;
                GC.SuppressFinalize(this);
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~MainViewModel()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public LineGeometry3D Lines { get; }
        public PointGeometry3D Points { get; }
        public Transform3D Lines1Transform { get; }
        public Transform3D Lines2Transform { get; }
        public Transform3D Points1Transform { get; }
        public Vector3D DirectionalLightDirection { get; }
        public Color DirectionalLightColor { get; }
        public Color AmbientLightColor { get; }
        public Stopwatch StopWatch { get; }

        public IEffectsManager EffectsManager { get; }
        public Camera Camera { get; }

        private int numberOfPoints;
        public int NumberOfPoints
        {
            get => numberOfPoints;
            set
            {
                StopWatch.Stop();

                SetValue(ref numberOfPoints, value);
                Lines.Indices = new IntCollection(numberOfPoints * 2);
                for (int i = 0; i < numberOfPoints * 2; i++)
                {
                    Lines.Indices.Add(i);
                }

                StopWatch.Start();
            }
        }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new PerspectiveCamera
            {
                Position = new Point3D(-90, 0, 95),
                LookDirection = new Vector3D(110, 0, -105),
                UpDirection = new Vector3D(0, 1, 0)
            };

            // setup lighting            
            AmbientLightColor = Colors.DimGray;
            DirectionalLightColor = Colors.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            // model trafos
            Lines1Transform = new TranslateTransform3D(0, 0, 45);
            Lines2Transform = new TranslateTransform3D(0, 0, -45);
            Points1Transform = new TranslateTransform3D(0, 0, 0);

            Lines = new LineGeometry3D { IsDynamic = true, Positions = new Vector3Collection() };
            Points = new PointGeometry3D { IsDynamic = true, Positions = new Vector3Collection() };

            StopWatch = new Stopwatch();
            StopWatch.Start();

            NumberOfPoints = 900;
        }

        public void UpdatePoints()
        {
            if (StopWatch.IsRunning)
            {
                Points.Positions.Clear();
                Points.Positions.AddRange(GeneratePoints(NumberOfPoints, StopWatch.ElapsedMilliseconds * 0.003));
                Lines.Positions.Clear();            
                Lines.Positions.AddRange(Points.Positions);
                Points.UpdateVertices();
                Lines.UpdateVertices();
            }
        }

        private static IEnumerable<Vector3> GeneratePoints(int n, double time)
        {
            const double R = 30;
            const double Q = 5;
            for (int i = 0; i < n; i++)
            {
                double t = Math.PI * 2 * i / (n - 1);
                double u = (t * 24) + (time * 5);
                var pt = new Vector3(
                    (float)(Math.Cos(t) * (R + (Q * Math.Cos(u)))),
                    (float)(Math.Sin(t) * (R + (Q * Math.Cos(u)))),
                    (float)(Q * Math.Sin(u)));
                yield return pt;
                if (i > 0 && i < n - 1)
                {
                    yield return pt;
                }
            }
        }
    }
}
