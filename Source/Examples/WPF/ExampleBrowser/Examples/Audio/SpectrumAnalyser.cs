// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpectrumAnalyser.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using NAudio.Dsp;
using NAudioWpfDemo;

namespace AudioDemo
{
    public class SpectrumAnalyser : ModelVisual3D, ISpectrumAnalyser
    {
        private static readonly Geometry3D DefaultGeometry = GetDefaultGeometry();

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry3D), typeof(SpectrumAnalyser),
                                        new UIPropertyMetadata(DefaultGeometry, GeometryChanged));

        public static readonly DependencyProperty FrequencyColumnsProperty =
            DependencyProperty.Register("FrequencyColumns", typeof(int), typeof(SpectrumAnalyser), new UIPropertyMetadata(32));

        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("Distance", typeof(double), typeof(SpectrumAnalyser),
                                        new UIPropertyMetadata(1.0));

        public static readonly DependencyProperty ShowIntensityProperty =
            DependencyProperty.Register("ShowIntensity", typeof(bool), typeof(SpectrumAnalyser),
                                        new UIPropertyMetadata(true));

        public static readonly DependencyProperty ScaleHeightOnlyProperty =
            DependencyProperty.Register("ScaleHeightOnly", typeof(bool), typeof(SpectrumAnalyser),
                                        new UIPropertyMetadata(true));
        public int TimeColumns
        {
            get { return (int)GetValue(TimeColumnsProperty); }
            set { SetValue(TimeColumnsProperty, value); }
        }

        public static readonly DependencyProperty TimeColumnsProperty =
            DependencyProperty.Register("TimeColumns", typeof(int), typeof(SpectrumAnalyser), new UIPropertyMetadata(16));

        private GeometryModel3D[,] Models;
        private ScaleTransform3D[,] ScaleTransforms;

        public SpectrumAnalyser()
        {
            UpdateModels();
        }

        public Geometry3D Geometry
        {
            get { return (Geometry3D)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        public int FrequencyColumns
        {
            get { return (int)GetValue(FrequencyColumnsProperty); }
            set { SetValue(FrequencyColumnsProperty, value); }
        }

        public double Distance
        {
            get { return (double)GetValue(DistanceProperty); }
            set { SetValue(DistanceProperty, value); }
        }

        public bool ShowIntensity
        {
            get { return (bool)GetValue(ShowIntensityProperty); }
            set { SetValue(ShowIntensityProperty, value); }
        }

        public bool ScaleHeightOnly
        {
            get { return (bool)GetValue(ScaleHeightOnlyProperty); }
            set { SetValue(ScaleHeightOnlyProperty, value); }
        }

        private int updateCount = 0;

        public void Update(Complex[] fftResults)
        {
            var y = new double[FrequencyColumns];

            // Use only the lowest 1/8 of the spectrum
            int resultLength = fftResults.Length / 8;

            double yScale = 1.0 * FrequencyColumns / resultLength;
            yScale *= ShowIntensity ? 100 : 0.05;

            for (int i = 0; i < resultLength; i++)
            {
                double intensity = Math.Sqrt(fftResults[i].X * fftResults[i].X + fftResults[i].Y * fftResults[i].Y);
                double decibels = 10 * Math.Log10(fftResults[i].X * fftResults[i].X + fftResults[i].Y * fftResults[i].Y);
                var j = (int)((FrequencyColumns - 1) * (double)i / (resultLength - 1));
                y[j] += ShowIntensity ? intensity : 100 + decibels;
            }

            // Move to the next time columns every 20th sample
            updateCount++;
            if (updateCount % 20 == 0)
            {
                for (int j = TimeColumns - 1; j > 0; j--)
                    for (int i = 0; i < FrequencyColumns; i++)
                        ScaleTransforms[i, j].ScaleZ = ScaleTransforms[i, j - 1].ScaleZ;
            }

            for (int i = 0; i < FrequencyColumns; i++)
            {
                double s = y[i] * yScale;
                if (!ScaleHeightOnly)
                {
                    ScaleTransforms[i, 0].ScaleX = s;
                    ScaleTransforms[i, 0].ScaleY = s;
                }
                ScaleTransforms[i, 0].ScaleZ = s;
            }
        }

        private static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SpectrumAnalyser)d).UpdateModels();
        }

        private static Geometry3D GetDefaultGeometry()
        {
            // The default geometry is a box
            var mb = new MeshBuilder(false, false);
            mb.AddBox(new Point3D(0, 0, 0.5), 0.8, 0.8, 1);
            return mb.ToMesh();
        }

        public void LoadModel(string path, bool transformYup, double scale)
        {
            var importer = new ModelImporter();
            var mod = importer.Load(path);

            if (mod == null || mod.Children.Count == 0)
                return;

            var model = (mod.Children[0] as GeometryModel3D);
            if (model == null)
                return;

            var mesh = model.Geometry as MeshGeometry3D;
            if (mesh == null)
                return;

            var transform = new Transform3DGroup();
            if (transformYup)
                transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90)));
            transform.Children.Add(new ScaleTransform3D(scale, scale, scale));

            for (int i = 0; i < mesh.Positions.Count; i++)
                mesh.Positions[i] = transform.Transform(mesh.Positions[i]);

            ScaleHeightOnly = false;
            Geometry = mesh;
        }

        private void UpdateModels()
        {
            var group = new Model3DGroup();
            Models = new GeometryModel3D[FrequencyColumns, TimeColumns];
            ScaleTransforms = new ScaleTransform3D[FrequencyColumns, TimeColumns];

            for (int j = 0; j < TimeColumns; j++)
            {
                for (int i = 0; i < FrequencyColumns; i++)
                {
                    Material material = MaterialHelper.CreateMaterial(ColorHelper.HsvToColor(0.6 * i / (FrequencyColumns - 1), 1, 1));
                    ScaleTransforms[i, j] = new ScaleTransform3D(1, 1, 1);

                    var translation = new TranslateTransform3D((i - (FrequencyColumns - 1) * 0.5) * Distance, -j * Distance, 0);
                    var tg = new Transform3DGroup();
                    tg.Children.Add(ScaleTransforms[i, j]);
                    tg.Children.Add(translation);
                    Models[i, j] = new GeometryModel3D(Geometry, material) { Transform = tg };
                    group.Children.Add(Models[i, j]);
                }
            }

            Content = group;
        }
    }
}