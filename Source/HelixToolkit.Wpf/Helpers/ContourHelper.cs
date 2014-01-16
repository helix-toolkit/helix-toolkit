// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContourHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides functionality to calculate a contour slice through a 3 vertex facet.
    /// </summary>
    /// <remarks>
    /// See <a href="http://paulbourke.net/papers/conrec/">CONREC</a> for further information.
    /// </remarks>
    internal class ContourHelper
    {
        /// <summary>
        /// Provides the indices for the various <see cref="ContourFacetResult"/> cases.
        /// </summary>
        private static readonly IDictionary<ContourFacetResult, int[,]> ResultIndices
            = new Dictionary<ContourFacetResult, int[,]>
        {
            { ContourFacetResult.ZeroOnly, new[,] { { 0, 1 }, { 0, 2 } } },
            { ContourFacetResult.OneAndTwo, new[,] { { 0, 2 }, { 0, 1 } } },
            { ContourFacetResult.OneOnly, new[,] { { 1, 2 }, { 1, 0 } } },
            { ContourFacetResult.ZeroAndTwo, new[,] { { 1, 0 }, { 1, 2 } } },
            { ContourFacetResult.TwoOnly, new[,] { { 2, 0 }, { 2, 1 } } },
            { ContourFacetResult.ZeroAndOne, new[,] { { 2, 1 }, { 2, 0 } } },
        };

        /// <summary>
        /// The original mesh.
        /// </summary>
        private readonly MeshGeometry3D originalMesh;

        /// <summary>
        /// The parameter 'a' of the plane equation.
        /// </summary>
        private readonly double a;

        /// <summary>
        /// The parameter 'b' of the plane equation.
        /// </summary>
        private readonly double b;

        /// <summary>
        /// The parameter 'c' of the plane equation.
        /// </summary>
        private readonly double c;

        /// <summary>
        /// The parameter 'd' of the plane equation.
        /// </summary>
        private readonly double d;

        /// <summary>
        /// The sides.
        /// </summary>
        private readonly double[] sides = new double[3];

        /// <summary>
        /// The indices.
        /// </summary>
        private readonly int[] indices = new int[3];

        /// <summary>
        /// The points.
        /// </summary>
        private readonly Point3D[] points = new Point3D[3];

        /// <summary>
        /// The textures.
        /// </summary>
        private readonly Point[] textures = new Point[3];

        /// <summary>
        /// The position count.
        /// </summary>
        private int positionCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContourHelper"/> class.
        /// </summary>
        /// <param name="planeOrigin">
        /// The plane origin.
        /// </param>
        /// <param name="planeNormal">
        /// The plane normal.
        /// </param>
        /// <param name="originalMesh">
        /// The original mesh.
        /// </param>
        public ContourHelper(Point3D planeOrigin, Vector3D planeNormal, MeshGeometry3D originalMesh)
        {
            this.originalMesh = originalMesh;
            this.positionCount = originalMesh.Positions.Count;

            // Determine the equation of the plane as
            // ax + by + cz + d = 0
            var l = Math.Sqrt((planeNormal.X * planeNormal.X) + (planeNormal.Y * planeNormal.Y) + (planeNormal.Z * planeNormal.Z));
            this.a = planeNormal.X / l;
            this.b = planeNormal.Y / l;
            this.c = planeNormal.Z / l;
            this.d = -((planeNormal.X * planeOrigin.X) + (planeNormal.Y * planeOrigin.Y) + (planeNormal.Z * planeOrigin.Z));
        }

        /// <summary>
        /// The contour facet result.
        /// </summary>
        private enum ContourFacetResult
        {
            /// <summary>
            /// All of the points fall above the contour plane.
            /// </summary>
            None,

            /// <summary>
            /// Only the 0th point falls below the contour plane.
            /// </summary>
            ZeroOnly,

            /// <summary>
            /// The 1st and 2nd points fall below the contour plane.
            /// </summary>
            OneAndTwo,

            /// <summary>
            /// Only the 1st point falls below the contour plane.
            /// </summary>
            OneOnly,

            /// <summary>
            /// The 0th and 2nd points fall below the contour plane.
            /// </summary>
            ZeroAndTwo,

            /// <summary>
            /// Only the second point falls below the contour plane.
            /// </summary>
            TwoOnly,

            /// <summary>
            /// The 0th and 1st points fall below the contour plane.
            /// </summary>
            ZeroAndOne,

            /// <summary>
            /// All of the points fall below the contour plane.
            /// </summary>
            All
        }

        /// <summary>
        /// Gets a value indicating whether the source <see cref="MeshGeometry3D"/> has texture coordinates.
        /// </summary>
        /// <value>
        /// <c>true</c> if the mesh has texture coordinates; otherwise, <c>false</c>.
        /// </value>
        private bool HasTextureCoordinates
        {
            get
            {
                return this.originalMesh.TextureCoordinates != null && this.originalMesh.TextureCoordinates.Any();
            }
        }

        /// <summary>
        /// Create a contour slice through a 3 vertex facet.
        /// </summary>
        /// <param name="index0">
        /// The 0th point index.
        /// </param>
        /// <param name="index1">
        /// The 1st point index.
        /// </param>
        /// <param name="index2">
        /// The 2nd point index.
        /// </param>
        /// <param name="positions">
        /// Any new positions that are created, when the contour plane slices through the vertex.
        /// </param>
        /// <param name="textureCoordinates">
        /// Any new texture coordinates that are created, when the contour plane slices through the vertex.
        /// </param>
        /// <param name="triangleIndices">
        /// All triangle indices that are created, when 1 or more points fall below the contour plane.
        /// </param>
        public void ContourFacet(
            int index0,
            int index1,
            int index2,
            out List<Point3D> positions,
            out List<Point> textureCoordinates,
            out List<int> triangleIndices)
        {
            this.SetData(index0, index1, index2);

            positions = new List<Point3D>();
            textureCoordinates = new List<Point>();
            triangleIndices = new List<int>();

            var facetResult = this.GetContourFacet();
            switch (facetResult)
            {
                case ContourFacetResult.All:
                    triangleIndices = new List<int> { index0, index1, index2 };
                    return;
                case ContourFacetResult.None:
                    return;
                case ContourFacetResult.ZeroOnly:
                    triangleIndices = new List<int> { index0, this.positionCount++, this.positionCount++ };
                    break;
                case ContourFacetResult.OneAndTwo:
                    triangleIndices = new List<int> { index1, index2, this.positionCount, this.positionCount++, this.positionCount++, index1 };
                    break;
                case ContourFacetResult.OneOnly:
                    triangleIndices = new List<int> { index1, this.positionCount++, this.positionCount++ };
                    break;
                case ContourFacetResult.ZeroAndTwo:
                    triangleIndices = new List<int> { index2, index0, this.positionCount, this.positionCount++, this.positionCount++, index2 };
                    break;
                case ContourFacetResult.TwoOnly:
                    triangleIndices = new List<int> { index2, this.positionCount++, this.positionCount++ };
                    break;
                case ContourFacetResult.ZeroAndOne:
                    triangleIndices = new List<int> { index0, index1, this.positionCount, this.positionCount++, this.positionCount++, index0 };
                    break;
            }

            var facetIndices = ResultIndices[facetResult];
            positions = new List<Point3D>
            {
                this.CreateNewPosition(facetIndices[0, 0], facetIndices[0, 1]),
                this.CreateNewPosition(facetIndices[1, 0], facetIndices[1, 1])
            };

            if (this.HasTextureCoordinates)
            {
                textureCoordinates = new List<Point>
                {
                    this.CreateNewTexture(facetIndices[0, 0], facetIndices[0, 1]),
                    this.CreateNewTexture(facetIndices[1, 0], facetIndices[1, 1])
                };
            }
        }

        /// <summary>
        /// Gets the <see cref="ContourFacetResult"/> for the current facet.
        /// </summary>
        /// <returns>a facet result.</returns>
        private ContourFacetResult GetContourFacet()
        {
            if (this.IsSideAlone(0))
            {
                return this.sides[0] > 0 ? ContourFacetResult.ZeroOnly : ContourFacetResult.OneAndTwo;
            }

            if (this.IsSideAlone(1))
            {
                return this.sides[1] > 0 ? ContourFacetResult.OneOnly : ContourFacetResult.ZeroAndTwo;
            }

            if (this.IsSideAlone(2))
            {
                return this.sides[2] > 0 ? ContourFacetResult.TwoOnly : ContourFacetResult.ZeroAndOne;
            }

            if (this.AllSidesBelowContour())
            {
                return ContourFacetResult.All;
            }

            return ContourFacetResult.None;
        }

        /// <summary>
        /// Initializes the facet data and calculates the <see cref="sides"/> values from the specified triangle indices. 
        /// </summary>
        /// <param name="index0">The first triangle index of the facet.</param>
        /// <param name="index1">The second triangle index of the facet.</param>
        /// <param name="index2">The third triangle index of the facet.</param>
        private void SetData(int index0, int index1, int index2)
        {
            this.indices[0] = index0;
            this.indices[1] = index1;
            this.indices[2] = index2;

            this.points[0] = this.originalMesh.Positions[index0];
            this.points[1] = this.originalMesh.Positions[index1];
            this.points[2] = this.originalMesh.Positions[index2];

            if (this.originalMesh.TextureCoordinates != null && this.originalMesh.TextureCoordinates.Any())
            {
                this.textures[0] = this.originalMesh.TextureCoordinates[index0];
                this.textures[1] = this.originalMesh.TextureCoordinates[index1];
                this.textures[2] = this.originalMesh.TextureCoordinates[index2];
            }

            this.sides[0] = (this.a * this.points[0].X) + (this.b * this.points[0].Y) + (this.c * this.points[0].Z) + this.d;
            this.sides[1] = (this.a * this.points[1].X) + (this.b * this.points[1].Y) + (this.c * this.points[1].Z) + this.d;
            this.sides[2] = (this.a * this.points[2].X) + (this.b * this.points[2].Y) + (this.c * this.points[2].Z) + this.d;
        }

        /// <summary>
        /// Calculates the position at the plane intersection for the side specified by two triangle indices.
        /// </summary>
        /// <param name="index0">The first index.</param>
        /// <param name="index1">The second index.</param>
        /// <returns>The interpolated position.</returns>
        private Point3D CreateNewPosition(int index0, int index1)
        {
            var firstPoint = this.points[index0];
            var secondPoint = this.points[index1];
            var firstSide = this.sides[index0];
            var secondSide = this.sides[index1];
            return new Point3D(
                firstPoint.X - (firstSide * (secondPoint.X - firstPoint.X) / (secondSide - firstSide)),
                firstPoint.Y - (firstSide * (secondPoint.Y - firstPoint.Y) / (secondSide - firstSide)),
                firstPoint.Z - (firstSide * (secondPoint.Z - firstPoint.Z) / (secondSide - firstSide)));
        }

        /// <summary>
        /// Calculates the texture coordinate at the plane intersection for the side specified by two triangle indices.
        /// </summary>
        /// <param name="index0">The first index.</param>
        /// <param name="index1">The second index.</param>
        /// <returns>The interpolated texture coordinate.</returns>
        private Point CreateNewTexture(int index0, int index1)
        {
            if (this.originalMesh.TextureCoordinates == null)
            {
                return new Point(0, 0);
            }

            var firstTexture = this.textures[index0];
            var secondTexture = this.textures[index1];
            var firstSide = this.sides[index0];
            var secondSide = this.sides[index1];
            return new Point(
                firstTexture.X - (firstSide * (secondTexture.X - firstTexture.X) / (secondSide - firstSide)),
                firstTexture.Y - (firstSide * (secondTexture.Y - firstTexture.Y) / (secondSide - firstSide)));
        }

        /// <summary>
        /// Determines whether the vertex at the specified index is at the opposite side of the other two vertices.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if the vertex is on its own side.</returns>
        private bool IsSideAlone(int index)
        {
            Func<int, int> getNext = i => i + 1 > 2 ? 0 : i + 1;

            var firstSideIndex = getNext(index);
            var secondSideIndex = getNext(firstSideIndex);
            return this.sides[index] * this.sides[firstSideIndex] < 0
                && this.sides[index] * this.sides[secondSideIndex] < 0;
        }

        /// <summary>
        /// Determines whether all sides of the facet are below the contour.
        /// </summary>
        /// <returns><c>true</c> if all sides are below the contour.</returns>
        private bool AllSidesBelowContour()
        {
            return this.sides[0] >= 0
                && this.sides[1] >= 0
                && this.sides[2] >= 0;
        }
    }
}