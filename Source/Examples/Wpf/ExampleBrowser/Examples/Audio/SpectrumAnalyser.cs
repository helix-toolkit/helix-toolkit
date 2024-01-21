using DependencyPropertyGenerator;
using HelixToolkit;
using HelixToolkit.Wpf;
using NAudio.Dsp;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Audio;

[DependencyProperty<Geometry3D>("Geometry", DefaultValueExpression = nameof(DefaultGeometry))]
[DependencyProperty<int>("FrequencyColumns", DefaultValue = 32)]
[DependencyProperty<int>("TimeColumns", DefaultValue = 16)]
[DependencyProperty<double>("Distance", DefaultValue = 1.0)]
[DependencyProperty<bool>("ShowIntensity", DefaultValue = true)]
[DependencyProperty<bool>("ScaleHeightOnly", DefaultValue = true)]
public sealed partial class SpectrumAnalyser : ModelVisual3D, ISpectrumAnalyser
{
    private static readonly Geometry3D DefaultGeometry = GetDefaultGeometry();

    private GeometryModel3D[,] Models;
    private ScaleTransform3D[,] ScaleTransforms;

#pragma warning disable CS8618
    public SpectrumAnalyser()
#pragma warning restore CS8618
    {
        UpdateModels();
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

    partial void OnGeometryChanged()
    {
        UpdateModels();
    }

    private static Geometry3D GetDefaultGeometry()
    {
        // The default geometry is a box
        var mb = new MeshBuilder(false, false);
        mb.AddBox(new System.Numerics.Vector3(0, 0, 0.5f), 0.8f, 0.8f, 1);
        return mb.ToMesh().ToWndMeshGeometry3D();
    }

    public void LoadModel(string path, bool transformYup, double scale)
    {
        var importer = new ModelImporter();
        var mod = importer.Load(path);

        if (mod == null || mod.Children.Count == 0)
            return;

        if (mod.Children[0] is not GeometryModel3D model)
            return;

        if (model.Geometry is not MeshGeometry3D mesh)
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
