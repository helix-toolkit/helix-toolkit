// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PanelModelBuilder.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PolyhedronDemo
{
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    public class PanelModelBuilder
    {
        public List<Panel> Panels { get; set; }

        public class Panel
        {
            public Point3D[] Points { get; set; }
            public int TriangleIndex { get; set; }
        }

        public List<int> TriangleIndexToPanelIndex { get; set; }

        public PanelModelBuilder()
        {
            Panels = new List<Panel>();
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
                tm.AddTriangleFan(p.Points);
                for (int i = 0; i < p.Points.Length - 2; i++) TriangleIndexToPanelIndex.Add(panelIndex);
                panelIndex++;
            }
            var panelsGeometry = tm.ToMesh();
            m.Children.Add(new GeometryModel3D(panelsGeometry, Materials.Red) { BackMaterial = Materials.Blue });

            // Add the nodes
            var gm = new MeshBuilder();
            foreach (var p in panelsGeometry.Positions)
            {
                gm.AddSphere(p, 0.05);
            }
            m.Children.Add(new GeometryModel3D(gm.ToMesh(), Materials.Gold));

            // Add the edges
            var em = new MeshBuilder();
            foreach (var p in Panels)
            {
                for (int i = 0; i < p.Points.Length; i += 1)
                {
                    em.AddCylinder(p.Points[i], p.Points[(i + 1) % p.Points.Length], 0.05, 10);
                }
            }
            m.Children.Add(new GeometryModel3D(em.ToMesh(), Materials.Gray));

            return m;
        }
    }
}