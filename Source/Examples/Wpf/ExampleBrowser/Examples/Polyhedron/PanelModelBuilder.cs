using HelixToolkit;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Polyhedron;

public sealed class PanelModelBuilder
{
    public List<Panel> Panels { get; set; } = new();

    public sealed class Panel
    {
        public required Point3D[] Points { get; set; }

        public int TriangleIndex { get; set; }
    }

    public List<int> TriangleIndexToPanelIndex { get; set; } = new();

    public PanelModelBuilder()
    {
    }

    public void AddPanel(params Point3D[] points)
    {
        Panels.Add(new Panel { Points = points });
    }

    public void AddPanel(params double[] coords)
    {
        Point3D[] points = new Point3D[coords.Length / 3];
        for (int i = 0; i < coords.Length / 3; i++) points[i] = new Point3D(coords[i * 3], coords[i * 3 + 1], coords[i * 3 + 2]);
        AddPanel(points);
    }

    public Model3D ToModel3D()
    {
        var m = new Model3DGroup();

        TriangleIndexToPanelIndex = new List<int>();

        // Add the triangles
        var tm = new MeshBuilder(false, false);
        int panelIndex = 0;
        foreach (var p in Panels)
        {
            p.TriangleIndex = tm.Positions.Count;
            tm.AddTriangleFan(p.Points.ToVector3Collection()!);
            for (int i = 0; i < p.Points.Length - 2; i++)
                TriangleIndexToPanelIndex.Add(panelIndex);
            panelIndex++;
        }
        var panelsGeometry = tm.ToMesh();
        m.Children.Add(new GeometryModel3D(panelsGeometry.ToMeshGeometry3D(), Materials.Red)
        {
            BackMaterial = Materials.Blue
        });

        // Add the nodes
        var gm = new MeshBuilder();
        foreach (var p in panelsGeometry.Positions)
        {
            gm.AddSphere(p, 0.05f);
        }
        m.Children.Add(new GeometryModel3D(gm.ToMesh().ToMeshGeometry3D(), Materials.Gold));

        // Add the edges
        var em = new MeshBuilder();
        foreach (var p in Panels)
        {
            for (int i = 0; i < p.Points.Length; i += 1)
            {
                em.AddCylinder(p.Points[i].ToVector3(), p.Points[(i + 1) % p.Points.Length].ToVector3(), 0.05f, 10);
            }
        }
        m.Children.Add(new GeometryModel3D(em.ToMesh().ToMeshGeometry3D(), Materials.Gray));

        return m;
    }
}
