// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContourHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Contour helper.
    /// </summary>
    /// <remarks>
    /// See http://paulbourke.net/papers/conrec/ for further information.
    /// </remarks>
    internal class ContourHelper
    {
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
        /// The a.
        /// </summary>
        private readonly double a;

        /// <summary>
        /// The b.
        /// </summary>
        private readonly double b;

        /// <summary>
        /// The c.
        /// </summary>
        private readonly double c;

        /// <summary>
        /// The d.
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

        private readonly bool hasTextureCoordinates;

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
        /// The orginal mesh.
        /// </param>
        public ContourHelper(Point3D planeOrigin, Vector3D planeNormal, MeshGeometry3D originalMesh, bool hasTextureCoordinates = false)
        {
            this.originalMesh = originalMesh;
            this.hasTextureCoordinates = hasTextureCoordinates;
            positionCount = originalMesh.Positions.Count;
            // Determine the equation of the plane as
            // Ax + By + Cz + D = 0
            var l = Math.Sqrt(planeNormal.X * planeNormal.X + planeNormal.Y * planeNormal.Y + planeNormal.Z * planeNormal.Z);
            a = planeNormal.X / l;
            b = planeNormal.Y / l;
            c = planeNormal.Z / l;
            d = -(planeNormal.X * planeOrigin.X + planeNormal.Y * planeOrigin.Y + planeNormal.Z * planeOrigin.Z);
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
        public void ContourFacet(int index0, int index1, int index2,
            out Point3D[] positions, out Point[] textureCoordinates,
            out int[] triangleIndices)
        {
            this.SetData(index0, index1, index2);

            var facetResult = this.GetContourFacet();
            switch (facetResult)
            {
                case ContourFacetResult.ZeroOnly:
                    triangleIndices = new [] { index0, positionCount++, positionCount++ };
                    break;
                case ContourFacetResult.OneAndTwo:
                    triangleIndices = new[] { index1, index2, positionCount, positionCount++, positionCount++, index1 };
                    break;
                case ContourFacetResult.OneOnly:
                    triangleIndices = new [] { index1, positionCount++, positionCount++ };
                    break;
                case ContourFacetResult.ZeroAndTwo:
                    triangleIndices = new[] { index2, index0, positionCount, positionCount++, positionCount++, index2 };
                    break;
                case ContourFacetResult.TwoOnly:
                    triangleIndices = new [] { index2, positionCount++, positionCount++ };
                    break;
                case ContourFacetResult.ZeroAndOne:
                    triangleIndices = new [] { index0, index1, positionCount, positionCount++, positionCount++, index0 };
                    break;
                case ContourFacetResult.All:
                    positions = new Point3D[0];
                    textureCoordinates = new Point[0];
                    triangleIndices = new[] { index0, index1, index2 };
                    return;
                case ContourFacetResult.None:
                default:
                    positions = new Point3D[0];
                    textureCoordinates = new Point[0];
                    triangleIndices = new int[0];
                    return;
            }
            var facetIndices = ResultIndices[facetResult];
            positions = new []
            {
                CreateNewPosition(facetIndices[0,0], facetIndices[0, 1]),
                CreateNewPosition(facetIndices[1,0], facetIndices[1, 1])
            };
            if (hasTextureCoordinates)
            {
                textureCoordinates = new []
                {
                    CreateNewTexture(facetIndices[0, 0], facetIndices[0, 1]),
                    CreateNewTexture(facetIndices[1, 0], facetIndices[1, 1])
                };
            }
            else
            {
                textureCoordinates = new Point[0];
            }
        }

        private ContourFacetResult GetContourFacet()
        {
            if (IsSideAlone(0))
            {
                return sides[0] > 0 ? ContourFacetResult.ZeroOnly : ContourFacetResult.OneAndTwo;
            }
            if (IsSideAlone(1))
            {
                return sides[1] > 0 ? ContourFacetResult.OneOnly : ContourFacetResult.ZeroAndTwo;
            }
            if (IsSideAlone(2))
            {
                return sides[2] > 0 ? ContourFacetResult.TwoOnly : ContourFacetResult.ZeroAndOne;
            }
            if (AllSidesBelowContour())
            {
                return ContourFacetResult.All;
            }
            return ContourFacetResult.None;
        }

        private void SetData(int index0, int index1, int index2)
        {
            indices[0] = index0;
            indices[1] = index1;
            indices[2] = index2;

            points[0] = originalMesh.Positions[index0];
            points[1] = originalMesh.Positions[index1];
            points[2] = originalMesh.Positions[index2];

            if (hasTextureCoordinates)
            {
                textures[0] = originalMesh.TextureCoordinates[index0];
                textures[1] = originalMesh.TextureCoordinates[index1];
                textures[2] = originalMesh.TextureCoordinates[index2];
            }

            sides[0] = a * points[0].X + b * points[0].Y + c * points[0].Z + d;
            sides[1] = a * points[1].X + b * points[1].Y + c * points[1].Z + d;
            sides[2] = a * points[2].X + b * points[2].Y + c * points[2].Z + d;
        }

        private Point3D CreateNewPosition(int index0, int index1)
        {
            var firstPoint = points[index0];
            var secondPoint = points[index1];
            var firstSide = sides[index0];
            var secondSide = sides[index1];
            return new Point3D(
                CalculatePoint(firstPoint.X, secondPoint.X, firstSide, secondSide),
                CalculatePoint(firstPoint.Y, secondPoint.Y, firstSide, secondSide),
                CalculatePoint(firstPoint.Z, secondPoint.Z, firstSide, secondSide));
        }

        private Point CreateNewTexture(int index0, int index1)
        {
            var firstTexture = textures[index0];
            var secondTexture = textures[index1];
            var firstSide = sides[index0];
            var secondSide = sides[index1];
            return new Point(
                CalculatePoint(firstTexture.X, secondTexture.X, firstSide, secondSide),
                CalculatePoint(firstTexture.Y, secondTexture.Y, firstSide, secondSide));
        }

        private static double CalculatePoint(double firstPoint, double secondPoint, double firstSide, double secondSide)
        {
            return firstPoint - (firstSide * (secondPoint - firstPoint) / (secondSide - firstSide));
        }

        private bool IsSideAlone(int index)
        {
            Func<int, int> getNext = i => i + 1 > 2 ? 0 : i + 1;

            var firstSideIndex = getNext(index);
            var secondSideIndex = getNext(firstSideIndex);
            return sides[index] * sides[firstSideIndex] < 0
                && sides[index] * sides[secondSideIndex] < 0;
        }

        private bool AllSidesBelowContour()
        {
            return sides[0] >= 0
                && sides[1] >= 0
                && sides[2] >= 0;
        }
    }
}