using DependencyPropertyGenerator;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Numerics;

namespace ScatterPlot;

[DependencyProperty<Point3D[]>("Points", OnChanged = nameof(UpdateModel))]
[DependencyProperty<double[]>("Values", OnChanged = nameof(UpdateModel))]
[DependencyProperty<Brush>("SurfaceBrush", OnChanged = nameof(UpdateModel))]
public sealed partial class ScatterPlotVisual3D : ModelVisual3D
{
    private readonly ModelVisual3D visualChild;

    public ScatterPlotVisual3D()
    {
        IntervalX = 1;
        IntervalY = 1;
        IntervalZ = 1;
        FontSize = 0.06;
        SphereSize = 0.09;
        LineThickness = 0.01;

        visualChild = new ModelVisual3D();
        Children.Add(visualChild);
    }

    // todo: make Dependency properties
    public double IntervalX { get; set; }
    public double IntervalY { get; set; }
    public double IntervalZ { get; set; }
    public double FontSize { get; set; }
    public double SphereSize { get; set; }
    public double LineThickness { get; set; }

    private void UpdateModel()
    {
        visualChild.Content = CreateModel();
    }

    private Model3D CreateModel()
    {
        var plotModel = new Model3DGroup();
        if (Points == null || Values == null) return plotModel;

        double minX = Points.Min(p => p.X);
        double maxX = Points.Max(p => p.X);
        double minY = Points.Min(p => p.Y);
        double maxY = Points.Max(p => p.Y);
        double minZ = Points.Min(p => p.Z);
        double maxZ = Points.Max(p => p.Z);
        double minValue = Values.Min();
        double maxValue = Values.Max();

        var valueRange = maxValue - minValue;

        var scatterMeshBuilder = new MeshBuilder(true, true);

        var oldTCCount = 0;
        for (var i = 0; i < Points.Length; ++i)
        {
            scatterMeshBuilder.AddSphere(Points[i].ToVector3(), (float)SphereSize, 4, 4);

            var u = (Values[i] - minValue) / valueRange;

            var newTCCount = scatterMeshBuilder.TextureCoordinates!.Count;
            for (var j = oldTCCount; j < newTCCount; ++j)
            {
                scatterMeshBuilder.TextureCoordinates[j] = new Vector2((float)u, (float)u);
            }
            oldTCCount = newTCCount;
        }

        var scatterModel = new GeometryModel3D(scatterMeshBuilder.ToMesh().ToWndMeshGeometry3D(),
                                               MaterialHelper.CreateMaterial(SurfaceBrush, null, null, 1, 0));
        scatterModel.BackMaterial = scatterModel.Material;

        // create bounding box with axes indications
        var axesMeshBuilder = new MeshBuilder();
        for (double x = minX; x <= maxX; x += IntervalX)
        {
            GeometryModel3D label = TextCreator.CreateTextLabelModel3D(x.ToString(), Brushes.Black, true, FontSize,
                                                                       new Point3D(x, minY - FontSize * 2.5, minZ),
                                                                       new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
            plotModel.Children.Add(label);
        }

        {
            GeometryModel3D label = TextCreator.CreateTextLabelModel3D("X-axis", Brushes.Black, true, FontSize,
                                                                       new Point3D((minX + maxX) * 0.5,
                                                                                   minY - FontSize * 6, minZ),
                                                                       new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
            plotModel.Children.Add(label);
        }

        for (double y = minY; y <= maxY; y += IntervalY)
        {
            GeometryModel3D label = TextCreator.CreateTextLabelModel3D(y.ToString(), Brushes.Black, true, FontSize,
                                                                       new Point3D(minX - FontSize * 3, y, minZ),
                                                                       new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
            plotModel.Children.Add(label);
        }
        {
            GeometryModel3D label = TextCreator.CreateTextLabelModel3D("Y-axis", Brushes.Black, true, FontSize,
                                                                       new Point3D(minX - FontSize * 10,
                                                                                   (minY + maxY) * 0.5, minZ),
                                                                       new Vector3D(0, 1, 0), new Vector3D(-1, 0, 0));
            plotModel.Children.Add(label);
        }
        double z0 = (int)(minZ / IntervalZ) * IntervalZ;
        for (double z = z0; z <= maxZ + double.Epsilon; z += IntervalZ)
        {
            GeometryModel3D label = TextCreator.CreateTextLabelModel3D(z.ToString(), Brushes.Black, true, FontSize,
                                                                       new Point3D(minX - FontSize * 3, maxY, z),
                                                                       new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
            plotModel.Children.Add(label);
        }
        {
            GeometryModel3D label = TextCreator.CreateTextLabelModel3D("Z-axis", Brushes.Black, true, FontSize,
                                                                       new Point3D(minX - FontSize * 10, maxY,
                                                                                   (minZ + maxZ) * 0.5),
                                                                       new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
            plotModel.Children.Add(label);
        }

        var bb = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);
        axesMeshBuilder.AddBoundingBox(bb.ToBoundingBox(), (float)LineThickness);

        var axesModel = new GeometryModel3D(axesMeshBuilder.ToMesh().ToWndMeshGeometry3D(), Materials.Black);

        plotModel.Children.Add(scatterModel);
        plotModel.Children.Add(axesModel);

        return plotModel;
    }
}
