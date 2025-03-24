using System.Windows.Media.Media3D;

using CommunityToolkit.Diagnostics;
using SRTM;

using HelixToolkit.Geometry;

namespace HelixToolkit.Wpf;

/// <summary>
/// Represents a SRTM terrain model.
/// </summary>
/// <remarks>
/// Supports the following terrain file types
/// .hgt
/// .hgt.zip
///  <para>
/// Read .hgt files from disk, keeps the model data and creates the Model3D.
/// The .hgt.zip format is a gzip compressed version of the .hgt format.
///  </para>
///  <para>
///  The source of the reading logic for this class is taken from the following git repository:
///  https://github.com/itinero/srtm
///  </para>
/// </remarks>
public class HgtTerrainModel : ITerrainModel
{
    private ISRTMDataCell? _cell;

    /// <summary>
    /// Gets or sets the bottom.
    /// </summary>
    /// <value>The bottom.</value>
    public double Bottom { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>The height.</value>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>The left.</value>
    public double Left { get; set; }

    /// <summary>
    /// Gets or sets the maximum Z.
    /// </summary>
    /// <value>The maximum Z.</value>
    public double MaximumZ { get; set; }

    /// <summary>
    /// Gets or sets the minimum Z.
    /// </summary>
    /// <value>The minimum Z.</value>
    public double MinimumZ { get; set; }

    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public Point3D Offset { get; set; }

    /// <summary>
    /// Gets or sets the right.
    /// </summary>
    /// <value>The right.</value>
    public double Right { get; set; }

    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>The texture.</value>
    public TerrainTexture? Texture { get; set; }

    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>The top.</value>
    public double Top { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    public int Width { get; set; }

    /// <summary>
    /// Gets the lod.
    /// </summary>
    public int Lod => 1;

    /// <inheritdoc cref="ITerrainModel.CreateModel(int)"/>
    public GeometryModel3D? CreateModel(int lod)
    {
        if (_cell == null) return null;

        var pointsPerCell = _cell.PointsPerCell;
        var pts = new List<Point3D>(pointsPerCell * pointsPerCell);
        var precisionInMeters = _cell.PrecisionInMeters / lod;

        double mx = (this.Left + this.Right) / 2 * precisionInMeters;
        double my = (this.Top + this.Bottom) / 2 * precisionInMeters;
        double mz = 0; // we don't need an offset for Z as we want to render it with real heights

        this.Offset = new Point3D(mx, my, mz);

        double previousZ = 0;
        for (int row = 0; row < pointsPerCell; row++)
        {
            for (int col = 0; col < pointsPerCell; col++)
            {
                double x = row * precisionInMeters;
                double y = col * precisionInMeters;
                double? z = _cell.GetElevation(row * _cell.PointsPerCell * 2 + col * 2);
                if (z == null)
                    z = previousZ; // just in case
                if (z > 9000)
                    z = previousZ;

                z = z / lod;

                x -= this.Offset.X;
                y -= this.Offset.Y;
                z -= this.Offset.Z;
                pts.Add(new Point3D(x, y, z.Value));
                previousZ = z.Value;
            }
        }

        var mb = new MeshBuilder(false, false);
        mb.AddRectangularMesh(pts.ToVector3Collection()!, pointsPerCell);
        var mesh = mb.ToMesh().ToWndMeshGeometry3D();

        var material = Materials.Green;

        if (Texture != null)
        {
            Texture.Calculate(this, mesh);
            material = Texture.Material;
            mesh.TextureCoordinates = Texture.TextureCoordinates;
        }

        return new GeometryModel3D
        {
            Geometry = mesh,
            Material = material,
            BackMaterial = material
        };
    }

    /// <summary>
    /// https://www.usgs.gov/publications/shuttle-radar-topography-mission-srtm
    /// </summary>
    /// <param name="source">
    /// The source file.
    /// </param>
    public void Load(string source)
    {
        Guard.IsNotNull(source);

        _cell = new SRTMDataCell(source);
        DefineModelProperties(_cell);
    }

    /// <summary>
    /// https://www.usgs.gov/publications/shuttle-radar-topography-mission-srtm
    /// This method is intended to load SRTM cell.
    /// </summary>
    /// <param name="cell">
    /// The source cell.
    /// </param>
    public void Load(ISRTMDataCell cell)
    {
        Guard.IsNotNull(cell);
        
        _cell = cell;
        DefineModelProperties(_cell);
    }

    private void DefineModelProperties(ISRTMDataCell cell)
    {
        this.Width = cell.PointsPerCell;
        this.Height = cell.PointsPerCell;
        this.Left = this.Width / 2;
        this.Right = this.Width / 2 + 1;
        this.Bottom = this.Height / 2;
        this.Top = this.Height / 2 + 1;

        this.MinimumZ = double.MaxValue;
        this.MaximumZ = double.MinValue;

        for (int row = 0; row < cell.PointsPerCell; row++)
        {
            for (int col = 0; col < cell.PointsPerCell; col++)
            {
                var z = cell.GetElevation(row * cell.PointsPerCell * 2 + col * 2);
                if (z == null)
                    continue;
                if (z > 9000)
                    continue;

                if (z < this.MinimumZ)
                {
                    this.MinimumZ = z.Value;
                }

                if (z > this.MaximumZ)
                {
                    this.MaximumZ = z.Value;
                }
            }
        }
    }
}
