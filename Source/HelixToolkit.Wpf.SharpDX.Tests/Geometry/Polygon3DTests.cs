using NUnit.Framework;
using System.Numerics;

namespace HelixToolkit.Wpf.SharpDX.Tests.Geometry;

[TestFixture]
public class Polygon3DTests
{
    [Test]
    public void Polygon_GetNormal_NotThrows()
    {
        var points = new List<Vector3>()
            {
                new Vector3(-1.39943f, 0.328622f, 0.97968f),
                new Vector3(-1.39969f, 0.328622f, 0.99105f),
                new Vector3(-1.39963f, 0.328631f, 0.99105f),
                new Vector3(-1.39954f, 0.328631f, 0.98726f),
                new Vector3(-1.39937f, 0.328631f, 0.97968f),
            };

        var polygon = new Polygon3D(points.ConvertAll(t => t));
        Vector3 normal = polygon.GetNormal();
    }
}
