using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf
{
    internal class Closest3DPointHitTester
    {
        private readonly Viewport3D mViewPort3D;

        /// <summary>
        /// Create a new CloseObjectHitTest that returns a 3D point closest to the clicked point on the camera.
        /// </summary>
        /// <param name="viewPort3D"></param>
        public Closest3DPointHitTester(Viewport3D viewPort3D)
        {
            mViewPort3D = viewPort3D;
        }


        /// <summary>
        /// Returns the 3D point that the user most likely clicked on.
        /// Returns the closest intersection with a mesh, if possible.
        /// If useClosestMeshIfPossible is true it will otherwise returns the mesh vertex that is closest to the clicked 2D screen coordinate, if possible.
        /// Returns the unprojected position otherwise.
        /// Also returns the 2D point corresponding to the returned 3D point.
        /// </summary>
        /// <param name="position"> The point in screen coordinates to calculate the closest vertices for. </param>
        /// <param name="useClosestMeshIfPossible"> When true the closest vertex point to the clicked position will be used when no mesh is hit at the exact mouse cursor area.</param>
        public NearestPointInCamera CalculateMouseDownNearestPoint(Point position, bool useClosestMeshIfPossible)
        {
            if (mViewPort3D.FindNearest(position, out Point3D nearestPoint, out _, out _))
            {
                return new NearestPointInCamera(position, nearestPoint);
            }

            if (useClosestMeshIfPossible)
            {
                ClosestVertexResult closestResult =
                    new Closest3DPointHitTester(mViewPort3D).ResultOfClosestPointHit2D(position);
                if (closestResult != null)
                {
                    return new NearestPointInCamera(closestResult.ClosestPointIn2D, closestResult.ClosestPoint);
                }
            }

            Point3D? unprojectedPosition = mViewPort3D.UnProject(position);
            return new NearestPointInCamera(position, unprojectedPosition);
        }

        public ClosestVertexResult ResultOfClosestPointHit2D(Point position)
        {
            List<ClosestVertexResult> closestHits =
                FindClosestHits(position).OrderBy(x => x.DistanceToPoint2D).ToList();
            if (closestHits.Count != 0)
                return closestHits[0];
            return null;
        }

        /// <summary>
        /// For each mesh object in the viewport, this method calculates the vertex that when mapped to 2D is closest to the given 2D point on the screen
        /// </summary>
        /// <param name="pointToHitTest"> The point in screen coordinates to calculate the closest vertices for. </param>
        /// <returns> A ClosestVertexResult containing the distance to the 2D point and the closest 3D vertex point for each geometry/model </returns>

        public IEnumerable<ClosestVertexResult> FindClosestHits(Point pointToHitTest)
        {
            ProjectionCamera camera = mViewPort3D.Camera as ProjectionCamera;

            if (camera == null)
            {
                throw new InvalidOperationException("No projection camera defined. Cannot find rectangle hits.");
            }

            List<ClosestVertexResult> results = new List<ClosestVertexResult>();
            mViewPort3D.Children.Traverse<GeometryModel3D>(
                (model, transform) =>
                {
                    MeshGeometry3D geometry = model.Geometry as MeshGeometry3D;
                    if (geometry == null || geometry.Positions == null || geometry.TriangleIndices == null)
                    {
                        return;
                    }

                    // transform the positions of the mesh to screen coordinates
                    Point3D[] point3Ds = geometry.Positions.Select(transform.Transform).ToArray();
                    Point[] point2Ds = geometry.Positions.Select(transform.Transform)
                        .Select(mViewPort3D.Point3DtoPoint2D).ToArray();

                    double minSquaredDistanceToPoint = double.PositiveInfinity;

                    Point3D closestPoint = point3Ds[0];
                    Point closestPointIn2D = point2Ds[0];
                    // evaluate each triangle
                    for (int i = 0; i < point2Ds.Length; i++)
                    {
                        Point point = point2Ds[i];
                        double xDiff = point.X - pointToHitTest.X;
                        double yDiff = point.Y - pointToHitTest.Y;
                        double squaredDistance = xDiff * xDiff + yDiff * yDiff;
                        if (squaredDistance < minSquaredDistanceToPoint)
                        {
                            minSquaredDistanceToPoint = squaredDistance;
                            closestPoint = point3Ds[i];
                            closestPointIn2D = point;
                        }
                    }

                    ClosestVertexResult hitTestResult = new ClosestVertexResult(model, geometry, closestPoint,
                        closestPointIn2D, Math.Sqrt(minSquaredDistanceToPoint));
                    results.Add(hitTestResult);

                });

            return results;
        }
    }
}