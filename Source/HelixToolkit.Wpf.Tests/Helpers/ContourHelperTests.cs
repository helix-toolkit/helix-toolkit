namespace HelixToolkit.Wpf.Tests
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    using NUnit.Framework;

    [TestFixture]
    public class ContourHelperTests
    {
        private static Point3D origin = new Point3D();
        private static Vector3D normal = new Vector3D(0, 0, 1);

        private Point3D[] newPositions;
        private Vector3D[] newNormals;
        private Point[] newTextureCoordinates;
        private int[] triangleIndices;

        [Test]
        public void OnePointAboveAndTwoPointsBelow()
        {
            this.Contour(MeshFromTriangle(new Point3D(2, 0, 1), new Point3D(2, 0, -1), new Point3D(0, 0, -1)));
            Assert.That(newPositions.Length, Is.EqualTo(2));
            Assert.That(newPositions[0], Is.EqualTo(new Point3D(2, 0, 0)));
            Assert.That(newPositions[1], Is.EqualTo(new Point3D(1, 0, 0)));
            Assert.That(triangleIndices.Length, Is.EqualTo(3));
            Assert.That(triangleIndices, Is.EquivalentTo(new[] { 0, 3, 4 }));
        }

        [Test]
        public void OnePointBelowAndTwoPointsAbove()
        {
            this.Contour(MeshFromTriangle(new Point3D(2, 0, -1), new Point3D(2, 0, 1), new Point3D(0, 0, 1)));
            Assert.That(newPositions.Length, Is.EqualTo(2));
            Assert.That(newPositions[0], Is.EqualTo(new Point3D(1, 0, 0)));
            Assert.That(newPositions[1], Is.EqualTo(new Point3D(2, 0, 0)));
            Assert.That(triangleIndices.Length, Is.EqualTo(6));
            Assert.That(triangleIndices, Is.EquivalentTo(new[] { 1, 2, 3, 3, 4, 1 }));
        }

        [Test]
        [Ignore("#55")]
        public void OnePointOnCuttingPlaneAndOthersOnOppositeSides()
        {
            this.Contour(MeshFromTriangle(new Point3D(0, 0, 0), new Point3D(1, 0, -1), new Point3D(1, 0, 1)));
            Assert.That(newPositions.Length, Is.EqualTo(1));
            Assert.That(newPositions[0], Is.EqualTo(new Point3D(1, 0, 0)));
            Assert.That(triangleIndices, Is.EquivalentTo(new[] { 0, 3, 2 }));
        }

        [Test]
        public void TwoPointsOnCuttingPlaneAndOneOnPositiveSide()
        {
            this.Contour(MeshFromTriangle(new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 0, 1)));
            Assert.That(newPositions.Length, Is.EqualTo(0));
            Assert.That(triangleIndices.Length, Is.EqualTo(3));
            Assert.That(triangleIndices, Is.EquivalentTo(new[] { 0, 1, 2 }));
        }

        [Test]
        public void TwoPointsOnCuttingPlaneAndOneOnNegativeSide()
        {
            this.Contour(MeshFromTriangle(new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 0, -1)));
            Assert.That(newPositions.Length, Is.EqualTo(0));
            Assert.That(triangleIndices.Length, Is.EqualTo(0));
        }

        private void Contour(MeshGeometry3D mesh)
        {
            var ch = new ContourHelper(origin, normal, mesh);
            ch.ContourFacet(0, 1, 2, out newPositions, out newNormals, out newTextureCoordinates, out triangleIndices);
        }

        private static MeshGeometry3D MeshFromTriangle(Point3D p1, Point3D p2, Point3D p3)
        {
            var mb = new MeshBuilder(false, false);
            mb.AddTriangle(p1, p2, p3);
            return mb.ToMesh();
        }
    }
}