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
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public Point3D Offset { get; set; }

    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>The texture.</value>
    public TerrainTexture? Texture { get; set; }

    /// <summary>
    /// Gets the lod.
    /// </summary>
    public int Lod => 1;

    /// <inheritdoc cref="ITerrainModel.CreateModel(int)"/>
    public GeometryModel3D? CreateModel(int lod)
    {
        if (_cell == null) return null;

        var horizontalPointsPerCell = _cell.PointsPerCell;
        var verticalPointsPerCell = _cell.VerticalPointsPerCell;
        var pts = new List<Point3D>(horizontalPointsPerCell * verticalPointsPerCell);

        double mx = _cell.PointsPerCell / 2 * _cell.PointWidthInMeters;
        double my = _cell.VerticalPointsPerCell / 2 * _cell.PointHeightInMeters;
        double mz = 0; // we don't need an offset for Z as we want to render it with real heights

        this.Offset = new Point3D(mx, my, mz);

        double previousZ = 0;
        for (int row = 0; row < verticalPointsPerCell; row++) // vertical
        {
            for (int col = 0; col < horizontalPointsPerCell; col++) // horizontal
            {
                double x = col * _cell.PointWidthInMeters;
                double y = row * _cell.PointHeightInMeters;
                double? z = _cell.GetElevation(row * _cell.PointsPerCell * 2 + col * 2);
                if (z == null)
                    z = previousZ; // just in case
                if (z > 9000)
                    z = previousZ;

                z = z / lod;

                x -= this.Offset.X; // x is moving from left to right -> from "-" to "+"
                y = this.Offset.Y - y; // y is moving from top to bottom -> from "+" to "-"
                z -= this.Offset.Z;
                pts.Add(new Point3D(x, y, z.Value));
                previousZ = z.Value;
            }
        }

        var mb = new MeshBuilder(false, false);
        mb.AddRectangularMesh(pts.ToVector3Collection()!, horizontalPointsPerCell);
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
    }
}
