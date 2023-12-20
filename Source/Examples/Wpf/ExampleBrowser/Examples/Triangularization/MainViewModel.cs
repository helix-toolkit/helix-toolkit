using HelixToolkit;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Triangularization;

/// <summary>
/// Provides a ViewModel for the Main window.
/// </summary>
public sealed class MainViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
    {
        // Create a model group
        Model3DGroup modelGroup = new();

        // Create a mesh builder and add a box to it
        MeshBuilder meshBuilder = new(false, false);
        CreateSimpleTriangulatedMesh(meshBuilder);

        // Create a mesh from the builder (and freeze it)
        MeshGeometry3D mesh = meshBuilder.ToMesh().ToMeshGeometry3D(true);

        // Create some materials
        Material blueMaterial = MaterialHelper.CreateMaterial(Colors.Blue);
        Material insideMaterial = MaterialHelper.CreateMaterial(Colors.Yellow);

        modelGroup.Children.Add(new GeometryModel3D
        {
            Geometry = mesh,
            Material = blueMaterial,
            BackMaterial = insideMaterial
        });

        Model = modelGroup;
    }

    private static void CreateSimpleTriangulatedMesh(MeshBuilder meshBuilder)
    {
        List<Point> triangleInPositiveOrientation = new()
            {
                new Point(0, 0),
                new Point(1, 0),
                new Point(0, 1)
            };
        //it's up to the user how to map 2D points to 3D, in this case we simply add a z-coordinate to the points:
        IEnumerable<Point3D> triangleInXYPlane = triangleInPositiveOrientation.Select(p => new Point3D(p.X, p.Y, 0));
        foreach (Point3D point in triangleInXYPlane)
            meshBuilder.Positions.Add(point.ToVector3());

        List<int>? indices = SweepLinePolygonTriangulator.Triangulate(triangleInPositiveOrientation.Select(t => t.ToVector()).ToList());

        if (indices is not null)
        {
            foreach (int index in indices)
            {
                meshBuilder.TriangleIndices.Add(index);
            }
        }

        //Create a triangle with the reversed orientation, this should thus also be visible from the other side:
        List<Point> triangleInNegativeOrientation = new()
            {
                new Point(0, 0),
                new Point(0, 1),
                new Point(1, 0)
            };
        triangleInXYPlane = triangleInPositiveOrientation.Select(p => new Point3D(p.X, p.Y, 1));
        foreach (Point3D point in triangleInXYPlane)
            meshBuilder.Positions.Add(point.ToVector3());

        indices = SweepLinePolygonTriangulator.Triangulate(triangleInNegativeOrientation.Select(t => t.ToVector()).ToList());

        if (indices is not null)
        {
            foreach (int index in indices)
            {
                //add 3 as we have already added a triangle before, making the starting index of this triangle 3 higher:
                meshBuilder.TriangleIndices.Add(index + 3);
            }
        }
    }

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    /// <value>The model.</value>
    public Model3D Model { get; set; }
}
