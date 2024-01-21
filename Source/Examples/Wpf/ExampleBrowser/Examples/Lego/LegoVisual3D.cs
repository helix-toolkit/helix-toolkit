using DependencyPropertyGenerator;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.Numerics;
using System.Windows.Media.Media3D;

namespace Lego;

/// <summary>
/// Traditional Lego bricks.
/// </summary>
[DependencyProperty<int>("Height", DefaultValue = 3, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<int>("Rows", DefaultValue = 2, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<int>("Columns", DefaultValue = 6, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<int>("Divisions", DefaultValue = 12)]
public sealed partial class LegoVisual3D : MeshElement3D
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
                var o = new Vector3((i + 0.5f) * (float)grid, (j + 0.5f) * (float)grid, (float)height);
                builder.AddCone(o, new Vector3(0, 0, 1), (float)(knobDiameter / 2), (float)(knobDiameter / 2), (float)knobHeight, false, true,
                                Divisions);
                builder.AddPipe(new Vector3(o.X, o.Y, o.Z - (float)wallThickness), new Vector3(o.X, o.Y, (float)wallThickness),
                                (float)knobDiameter, (float)outerDiameter, Divisions);
            }

        builder.AddBox(new Vector3((float)(Columns * 0.5 * grid), (float)(Rows * 0.5 * grid), (float)(height - wallThickness / 2)), (float)width, (float)length,
                      (float)wallThickness,
                      BoxFaces.All);
        builder.AddBox(new Vector3((float)(margin + wallThickness / 2), (float)(Rows * 0.5 * grid), (float)(height / 2 - wallThickness / 2)),
                       (float)wallThickness, (float)length, (float)(height - wallThickness),
                       BoxFaces.All ^ BoxFaces.Top);
        builder.AddBox(
            new Vector3((float)(Columns * grid - margin - wallThickness / 2), (float)(Rows * 0.5 * grid), (float)(height / 2 - wallThickness / 2)),
            (float)wallThickness, (float)length, (float)(height - wallThickness),
            BoxFaces.All ^ BoxFaces.Top);
        builder.AddBox(new Vector3((float)(Columns * 0.5 * grid), (float)(margin + wallThickness / 2), (float)(height / 2 - wallThickness / 2)),
                       (float)width, (float)wallThickness, (float)(height - wallThickness),
                       BoxFaces.All ^ BoxFaces.Top);
        builder.AddBox(
            new Vector3((float)(Columns * 0.5 * grid), (float)(Rows * grid - margin - wallThickness / 2), (float)(height / 2 - wallThickness / 2)),
            (float)width, (float)wallThickness, (float)(height - wallThickness),
            BoxFaces.All ^ BoxFaces.Top);

        return builder.ToMesh().ToWndMeshGeometry3D();
    }
}
