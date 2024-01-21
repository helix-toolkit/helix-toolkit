using DependencyPropertyGenerator;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Media.Media3D;

namespace WpfDemo;

/// <summary>
/// Traditional Lego bricks.
/// </summary>
[DependencyProperty<int>("Divisions", DefaultValue = 12)]
[DependencyProperty<int>("Height", DefaultValue = 3, OnChanged = nameof(OnChanged))]
[DependencyProperty<int>("Rows", DefaultValue = 2, OnChanged = nameof(OnChanged))]
[DependencyProperty<int>("Columns", DefaultValue = 6, OnChanged = nameof(OnChanged))]
public partial class LegoVisual3D : MeshElement3D
{
    private const double grid = 0.008;
    private const double margin = 0.0001;
    private const double wallThickness = 0.001;
    private const double plateThickness = 0.0032;
    private const double brickThickness = 0.0096;
    private const double knobHeight = 0.0018;
    private const double knobDiameter = 0.0048;
    private const double outerDiameter = 0.00651;
    private const double axleDiameter = 0.00475;
    private const double holeDiameter = 0.00485;

    protected void OnChanged()
    {
        OnGeometryChanged();
    }

    // http://www.robertcailliau.eu/Lego/Dimensions/zMeasurements-en.xhtml
    public static double GridUnit
    {
        get { return grid; }
    }
    public static double HeightUnit
    {
        get { return plateThickness; }
    }

    protected override MeshGeometry3D Tessellate()
    {
        double width = Columns * grid - margin * 2;
        double length = Rows * grid - margin * 2;
        double height = Height * plateThickness;
        var builder = new MeshBuilder(true, true);

        for (int i = 0; i < Columns; i++)
            for (int j = 0; j < Rows; j++)
            {
                var o = new Point3D((i + 0.5) * grid, (j + 0.5) * grid, height);
                builder.AddCone(o.ToVector3(), new Vector3D(0, 0, 1).ToVector3(), (float)knobDiameter / 2, (float)knobDiameter / 2, (float)knobHeight, false, true,
                                Divisions);
                builder.AddPipe(new Point3D(o.X, o.Y, o.Z - wallThickness).ToVector3(), new Point3D(o.X, o.Y, wallThickness).ToVector3(),
                                (float)knobDiameter, (float)outerDiameter, Divisions);
            }

        builder.AddBox(new Point3D(Columns * 0.5 * grid, Rows * 0.5 * grid, height - wallThickness / 2).ToVector3(), (float)width, (float)length,
                      (float)wallThickness,
                      BoxFaces.All);
        builder.AddBox(new Point3D(margin + wallThickness / 2, Rows * 0.5 * grid, height / 2 - wallThickness / 2).ToVector3(),
                       (float)wallThickness, (float)length, (float)(height - wallThickness),
                       BoxFaces.All ^ BoxFaces.Top);
        builder.AddBox(
            new Point3D(Columns * grid - margin - wallThickness / 2, Rows * 0.5 * grid, height / 2 - wallThickness / 2).ToVector3(),
            (float)wallThickness, (float)length, (float)(height - wallThickness),
            BoxFaces.All ^ BoxFaces.Top);
        builder.AddBox(new Point3D(Columns * 0.5 * grid, margin + wallThickness / 2, height / 2 - wallThickness / 2).ToVector3(),
                       (float)width, (float)wallThickness, (float)(height - wallThickness),
                       BoxFaces.All ^ BoxFaces.Top);
        builder.AddBox(
            new Point3D(Columns * 0.5 * grid, Rows * grid - margin - wallThickness / 2, height / 2 - wallThickness / 2).ToVector3(),
            (float)width, (float)wallThickness, (float)(height - wallThickness),
            BoxFaces.All ^ BoxFaces.Top);

        return builder.ToMesh().ToWndMeshGeometry3D();
    }
}
