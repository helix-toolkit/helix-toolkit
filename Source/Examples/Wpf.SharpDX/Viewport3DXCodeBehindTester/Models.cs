using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;

namespace Viewport3DXCodeBehindTester;

public class Models
{
    private readonly IList<Geometry3D> models = new List<Geometry3D>();
    private readonly PhongMaterialCollection materials = new();
    private readonly Random rnd = new();

    public Models()
    {
        var builder = new MeshBuilder();
        builder.AddSphere(Vector3.Zero.ToVector(), 1);
        models.Add(builder.ToMesh().ToMeshGeometry3D());

        builder = new MeshBuilder();
        builder.AddBox(Vector3.Zero.ToVector(), 1, 1, 1);
        models.Add(builder.ToMesh().ToMeshGeometry3D());

        builder = new MeshBuilder();
        builder.AddDodecahedron(Vector3.Zero.ToVector(), new Vector3(1, 0, 0).ToVector(), new Vector3(0, 1, 0).ToVector(), 1);
        models.Add(builder.ToMesh().ToMeshGeometry3D());
    }

    public MeshGeometryModel3D GetModelRandom()
    {
        var idx = rnd.Next(0, models.Count);
        MeshGeometryModel3D model = new()
        {
            Geometry = models[idx],
            CullMode = SharpDX.Direct3D11.CullMode.Back
        };
        var scale = new System.Windows.Media.Media3D.ScaleTransform3D(rnd.NextDouble(1, 5), rnd.NextDouble(1, 5), rnd.NextDouble(1, 5));
        var translate = new System.Windows.Media.Media3D.TranslateTransform3D(rnd.NextDouble(-20, 20), rnd.NextDouble(-20, 20), rnd.NextDouble(-20, 20));
        var group = new System.Windows.Media.Media3D.Transform3DGroup();
        group.Children.Add(scale);
        group.Children.Add(translate);
        model.Transform = group;
        var material = materials[rnd.Next(0, materials.Count - 1)];
        model.Material = material;
        if (material.DiffuseColor.Alpha < 1)
        {
            model.IsTransparent = true;
        }
        return model;
    }

    public MeshNode GetSceneNodeRandom()
    {
        var idx = rnd.Next(0, models.Count);
        MeshNode model = new()
        {
            Geometry = models[idx],
            CullMode = SharpDX.Direct3D11.CullMode.Back
        };
        var scale = Matrix.Scaling((float)rnd.NextDouble(1, 5), (float)rnd.NextDouble(1, 5), (float)rnd.NextDouble(1, 5));
        var translate = Matrix.Translation((float)rnd.NextDouble(-20, 20), (float)rnd.NextDouble(-20, 20), (float)rnd.NextDouble(-20, 20));
        model.ModelMatrix = scale * translate;
        var material = materials[rnd.Next(0, materials.Count - 1)];
        model.Material = material;
        if (material.DiffuseColor.Alpha < 1)
        {
            model.IsTransparent = true;
        }
        return model;
    }
}
