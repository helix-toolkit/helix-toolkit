using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Helpers.ScreenSpace;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class LineGeometryBuilderTests
{
    [Test]
    public void CreatePositions_CoincidingProjectedPoints_ShouldNotReturnNaN()
    {
        Point3DCollection? result = null;
        CrossThreadTestRunner.RunInSTA(
            () =>
            {
                var vp = new Viewport3D();
                var visual = new ModelVisual3D();
                vp.Children.Add(visual);

                var b = new LineGeometryBuilder(visual);
                b.UpdateTransforms();
                var c = (ProjectionCamera)vp.Camera;
                result = b.CreatePositions(new[] { c.Position + c.LookDirection, c.Position + (c.LookDirection * 2) });
            });

        for (int i = 0; i < result?.Count; i++)
        {
            Assert.That(
                !double.IsNaN(result[i].X) && !double.IsNaN(result[i].Y) && !double.IsNaN(result[i].Z),
                "Point " + i + " is invalid");
        }
    }

    [Test]
    public void CreatePositions_CoincidingPoints_ShouldNotReturnNaN()
    {
        Point3DCollection? result = null;
        CrossThreadTestRunner.RunInSTA(
            () =>
            {
                var vp = new Viewport3D();
                var visual = new ModelVisual3D();
                vp.Children.Add(visual);

                var b = new LineGeometryBuilder(visual);
                b.UpdateTransforms();
                result = b.CreatePositions(new[] { new Point3D(0, 0, 0), new Point3D(0, 0, 0) });
            });

        for (int i = 0; i < result?.Count; i++)
        {
            Assert.That(
                !double.IsNaN(result[i].X) && !double.IsNaN(result[i].Y) && !double.IsNaN(result[i].Z),
                "Point " + i + " is invalid");
        }
    }
}
