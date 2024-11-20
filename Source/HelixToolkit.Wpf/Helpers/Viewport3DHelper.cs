using CommunityToolkit.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides extension methods for <see cref="Viewport3D"/>.
/// </summary>
/// <remarks>
/// See "3D programming for Windows" (Charles Petzold book) and <a hef="http://www.ericsink.com/wpf3d/index.html">Twelve Days of WPF 3D</a>.
/// </remarks>
public static class Viewport3DHelper
{
    /// <summary>
    /// Copies the specified viewport to the clipboard.
    /// </summary>
    /// <param name="view">The viewport.</param>
    /// <param name="m">The oversampling multiplier.</param>
    public static void Copy(this Viewport3D view, int m = 1)
    {
        Clipboard.SetImage(RenderBitmap(view, Brushes.White, m));
    }

    /// <summary>
    /// Copies the specified viewport to the clipboard.
    /// </summary>
    /// <param name="view">The viewport.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="background">The background.</param>
    /// <param name="m">The oversampling multiplier.</param>
    public static void Copy(this Viewport3D view, double width, double height, Brush background, int m = 1)
    {
        Clipboard.SetImage(RenderBitmap(view, width, height, background, m));
    }

    /// <summary>
    /// Copies the viewport as <code>xaml</code> to the clipboard.
    /// </summary>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    public static void CopyXaml(this Viewport3D viewport)
    {
        Clipboard.SetText(XamlWriter.Save(viewport));
    }

    /// <summary>
    /// Exports the specified viewport.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="background">The background brush.</param>
    public static void Export(this Viewport3D viewport, string fileName, Brush? background = null)
    {
        string ext = System.IO.Path.GetExtension(fileName);
        if (ext != null)
        {
            ext = ext.ToLower();
        }

        switch (ext)
        {
            case ".jpg":
            case ".png":
                SaveBitmap(viewport, fileName, background, 2);
                break;
            case ".xaml":
                ExportXaml(viewport, fileName);
                break;
            case ".xml":
                ExportKerkythea(viewport, fileName, background);
                break;
            case ".obj":
                ExportObj(viewport, fileName);
                break;
            case ".x3d":
                ExportX3D(viewport, fileName);
                break;
            case ".dae":
                ExportCollada(viewport, fileName);
                break;
            case ".stl":
                ExportStl(viewport, fileName);
                break;
            default:
                throw new HelixToolkitException("Not supported file format.");
        }
    }

    /// <summary>
    /// Exports the specified viewport.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="stereoBase">The stereo base.</param>
    /// <param name="background">The background brush.</param>
    /// <exception cref="HelixToolkitException">Not supported file format.</exception>
    public static void ExportStereo(this Viewport3D viewport, string fileName, double stereoBase, Brush? background = null)
    {
        string ext = System.IO.Path.GetExtension(fileName);
        if (ext != null)
        {
            ext = ext.ToLower();
        }

        switch (ext)
        {
            case ".jpg":
            case ".png":
                SaveStereoBitmap(viewport, fileName, stereoBase, background, 2);
                break;
            case ".mpo":
                throw new HelixToolkitException("MPO is not yet supported.");
            default:
                throw new HelixToolkitException("Not supported file format.");
        }
    }

    /// <summary>
    /// Finds the hits for the specified position.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="position">The position.</param>
    /// <param name="filterCallback">The method that represents the hit test filter callback value./></param>
    /// <returns> List of hits, sorted with the nearest hit first.</returns>
    public static IList<PointHitResult> FindHits(this Viewport3D viewport, Point position, HitTestFilterCallback? filterCallback = null)
    {
        if (viewport is null)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(viewport));
        }
        if (viewport.Camera is null)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(viewport.Camera));
        }

        List<PointHitResult> result = new List<PointHitResult>();
        Point3D cameraPosition = new Point3D();
        if (viewport.Camera is ProjectionCamera projectionCamera)
        {
            cameraPosition = projectionCamera.Position;
        }
        else if (viewport.Camera is MatrixCamera matrixCamera)
        {
            Matrix3D inversedViewMatrix = matrixCamera.ViewMatrix.Inverse();
            cameraPosition = new Point3D(inversedViewMatrix.OffsetX, inversedViewMatrix.OffsetY, inversedViewMatrix.OffsetZ);
        }
        var pointHitTestParamrters = new PointHitTestParameters(position);
        VisualTreeHelper.HitTest(viewport, filterCallback, DefaultHitTestResultBehaviorCallback, pointHitTestParamrters);
        result.Sort((x, y) => x.Distance.CompareTo(y.Distance));
        return result;

        HitTestResultBehavior DefaultHitTestResultBehaviorCallback(HitTestResult hit)
        {
            if (hit is RayMeshGeometry3DHitTestResult rayHit)
            {
                if (rayHit.MeshHit is not null)
                {
                    Point3D position = GetGlobalHitPosition(rayHit, viewport);
                    Vector3D normalHit = GetNormalHit(rayHit) ?? new Vector3D(0, 0, 1);
                    double distance = (cameraPosition - position).Length;
                    PointHitResult hitResult = new PointHitResult(rayHit, distance, position, normalHit);
                    result.Add(hitResult);
                }
            }
            return HitTestResultBehavior.Continue;
        }
    }

    /// <summary>
    /// Finds the hits for the specified rectangle.
    /// </summary>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="rectangle">
    /// The rectangle.
    /// </param>
    /// <param name="mode">
    /// The mode of selection.
    /// </param>
    /// <returns>
    /// The list of the hits.
    /// </returns>
    public static IEnumerable<RectangleHitResult> FindHits(this Viewport3D viewport, Rect rectangle, SelectionHitMode mode)
    {
        if (viewport is null)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(viewport));
        }
        if (viewport.Camera is null)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(viewport.Camera));
        }

        if (rectangle.Size.Equals(default)
           || HelixToolkit.Maths.MathUtil.IsZero((float)rectangle.Width) && HelixToolkit.Maths.MathUtil.IsZero((float)rectangle.Height))
        {
            IList<PointHitResult> hitResults = FindHits(viewport, rectangle.BottomLeft);
            return hitResults.Select(x => new RectangleHitResult(x.Model, x.Visual));
        }

        var results = new List<RectangleHitResult>();
        viewport.Children.Traverse<GeometryModel3D>(
            (model, visual, transform) =>
            {
                if (model.Geometry is not MeshGeometry3D geometry
                || model.Bounds.IsEmpty
                || geometry.Bounds.IsEmpty
                || geometry.Positions == null
                || geometry.Positions.Count == 0
                || geometry.TriangleIndices == null
                || geometry.TriangleIndices.Count == 0)
                {
                    return;
                }
                // ref https://learn.microsoft.com/en-us/archive/blogs/wpf3d/transforming-bounds
                GeneralTransform3DTo2D transform3DTo2D = visual.TransformToAncestor(viewport);
                // If null, the transform isn't possible at all
                if (transform3DTo2D is null) return;
                Rect visualBound = transform3DTo2D.TransformBounds(VisualTreeHelper.GetDescendantBounds(visual));

                if (rectangle.Contains(visualBound)) // object is both inside and touch
                {
                    results.Add(new RectangleHitResult(model, visual));
                }
                else if (rectangle.IntersectsWith(visualBound))
                {
                    // transform the positions of the mesh to screen coordinates
                    Point[] meshPoints;

                    var point2Ds = TransformPoints3dTo2d(viewport, transform, geometry.Positions);

                    int[] distinctTriangleIndices = geometry.TriangleIndices.Distinct().ToArray();
                    if (geometry.Positions.Count == distinctTriangleIndices.Length)//MeshGeometry3D maybe (high probability) is origin (have not edited like is cutted...)
                    {
                        /* Get bound points by Transform Rect3D of MeshGeometry3D.Bounds to Rect 2d through 8 corners point
                         *
                         *              Z | Up   
                         *                |
                         *        p4______|_________p7
                         *        /|      |        /|
                         *       / |      |       / |
                         *      /  |      |      /  |
                         *  p5 |----------------|p6 |
                         *     |   |      |     |   |
                         *     |   |    O +-----|---------- Y Left
                         *     | p0|_____/______|___|p3
                         *     |  /     /       |   /
                         *     | /     /        |  /
                         *     |/     /         | /
                         *  p1 |_____/__________|/p2
                         *          /
                         *       X / Front
                         */

                        Rect3D rect3d = geometry.Bounds;
                        Point3D p0 = rect3d.Location;
                        Point3D p1 = new Point3D(p0.X + rect3d.SizeX, p0.Y, p0.Z);
                        Point3D p2 = new Point3D(p0.X + rect3d.SizeX, p0.Y + rect3d.SizeY, p0.Z);
                        Point3D p3 = new Point3D(p0.X, p0.Y + rect3d.SizeY, p0.Z);

                        Point3D p4 = new Point3D(p0.X, p0.Y, p0.Z + rect3d.SizeZ);
                        Point3D p5 = new Point3D(p1.X, p1.Y, p1.Z + rect3d.SizeZ);
                        Point3D p6 = new Point3D(p2.X, p2.Y, p2.Z + rect3d.SizeZ);
                        Point3D p7 = new Point3D(p3.X, p3.Y, p3.Z + rect3d.SizeZ);

                        Point3D[] point3dCorners = new Point3D[8] { p0, p1, p2, p3, p4, p5, p6, p7 };
                        meshPoints = TransformPoints3dTo2d(viewport, transform, point3dCorners);
                    }
                    else //MeshGeometry3D maybe (high probability) is changed from origin model3D (like is cutted...)
                    {
                        //Get all point in actual mesh position 
                        int length = distinctTriangleIndices.Length;
                        meshPoints = new Point[length];
                        for (var i = 0; i < length; i++)
                        {
                            int triangleIndice = distinctTriangleIndices[i];
                            meshPoints[i] = point2Ds[triangleIndice];
                        }
                    }
                    Rect geometryBound = GetBound2d(meshPoints);

                    if (rectangle.Contains(geometryBound))// object is both inside and touch
                    {
                        results.Add(new RectangleHitResult(model, visual));
                    }
                    else if (mode == SelectionHitMode.Touch && rectangle.IntersectsWith(geometryBound))
                    {
                        // evaluate each triangle
                        int triangleIndiceCount = geometry.TriangleIndices.Count;
                        for (var i = 0; i < triangleIndiceCount; i += 3)
                        {
                            var p1 = point2Ds[geometry.TriangleIndices[i]];
                            var p2 = point2Ds[geometry.TriangleIndices[i + 1]];
                            var p3 = point2Ds[geometry.TriangleIndices[i + 2]];
                            var triangle = new Triangle(p1, p2, p3);
                            // check if triangle touched/inside with rectangle
                            if (triangle.IsCompletelyInside(rectangle)
                                     || triangle.IntersectsWith(rectangle)
                                     || triangle.IsRectCompletelyInside(rectangle))
                            {
                                results.Add(new RectangleHitResult(model, visual));
                                break;
                            }
                        }
                    }
                }
            });
        return results;
        Point[] TransformPoints3dTo2d(Viewport3D viewport3d, Transform3D transform3d, IList<Point3D> point3ds)
        {
            int length = point3ds.Count;
            Point[] point2ds = new Point[length];
            for (int i = 0; i < length; i++)
            {
                Point3D transformedPoint3d = transform3d.Transform(point3ds[i]);
                Point point2d = viewport3d.Point3DtoPoint2D(transformedPoint3d);
                point2ds[i] = point2d;
            }
            return point2ds;
        }
        Rect GetBound2d(Point[] point2ds)
        {
            double minX = point2ds[0].X;
            double minY = point2ds[0].Y;
            double maxX = point2ds[0].X;
            double maxY = point2ds[0].Y;
            for (int i = 1; i < point2ds.Length; i++)
            {
                ref Point pi = ref point2ds[i];
                if (pi.X < minX)
                    minX = pi.X;
                else if (pi.X > maxX)
                    maxX = pi.X;
                if (pi.Y < minY)
                    minY = pi.Y;
                else if (pi.Y > maxY)
                    maxY = pi.Y;
            }
            return new Rect(new Point(minX, minY), new Point(maxX, maxY));
        }

    }

    /// <summary>
    /// Finds the nearest visual, hit point and its normal at the specified position.
    /// </summary>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="position">
    /// The position.
    /// </param>
    /// <param name="point">
    /// The 3D hit point.
    /// </param>
    /// <param name="normal">
    /// The normal of the mesh at the hit point.
    /// </param>
    /// <param name="visual">
    /// The hit visual.
    /// </param>
    /// <returns>
    /// <c>true</c> if a visual was found at the specified position.
    /// </returns>
    public static bool FindNearest(this Viewport3D viewport, Point position, out Point3D point, out Vector3D normal, out DependencyObject? visual)
    {
        if (viewport.Camera is not ProjectionCamera camera)
        {
            point = new Point3D();
            normal = new Vector3D();
            visual = null;
            return false;
        }

        var htp = new PointHitTestParameters(position);

        double minimumDistance = double.MaxValue;
        var nearestPoint = new Point3D();
        var nearestNormal = new Vector3D();
        DependencyObject? nearestObject = null;

        VisualTreeHelper.HitTest(
            viewport,
            null,
            delegate (HitTestResult hit)
            {
                if (hit is RayMeshGeometry3DHitTestResult rayHit)
                {
                    var mesh = rayHit.MeshHit;
                    if (mesh != null)
                    {
                        var p1 = mesh.Positions[rayHit.VertexIndex1];
                        var p2 = mesh.Positions[rayHit.VertexIndex2];
                        var p3 = mesh.Positions[rayHit.VertexIndex3];
                        double x = (p1.X * rayHit.VertexWeight1) + (p2.X * rayHit.VertexWeight2) + (p3.X * rayHit.VertexWeight3);
                        double y = (p1.Y * rayHit.VertexWeight1) + (p2.Y * rayHit.VertexWeight2) + (p3.Y * rayHit.VertexWeight3);
                        double z = (p1.Z * rayHit.VertexWeight1) + (p2.Z * rayHit.VertexWeight2) + (p3.Z * rayHit.VertexWeight3);

                        // point in local coordinates
                        var p = new Point3D(x, y, z);

                        // transform to global coordinates

                        // first transform the Model3D hierarchy
                        var t2 = rayHit.VisualHit.GetTransformTo(rayHit.ModelHit);
                        if (t2 != null)
                        {
                            p = t2.Transform(p);
                        }

                        // then transform the Visual3D hierarchy up to the Viewport3D ancestor
                        var t = GetTransform(viewport, rayHit.VisualHit);
                        if (t != null)
                        {
                            p = t.Transform(p);
                        }

                        double distance = (camera.Position - p).LengthSquared;
                        if (distance < minimumDistance)
                        {
                            minimumDistance = distance;
                            nearestPoint = p;
                            nearestNormal = Vector3D.CrossProduct(p2 - p1, p3 - p1);
                            nearestObject = hit.VisualHit;
                        }
                    }
                }

                return HitTestResultBehavior.Continue;
            },
            htp);

        point = nearestPoint;
        visual = nearestObject;
        normal = nearestNormal;

        if (minimumDistance >= double.MaxValue)
        {
            return false;
        }

        normal.Normalize();
        return true;
    }

    /// <summary>
    /// Finds the coordinates of the nearest point at the specified position.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="position">The position.</param>
    /// <returns>The nearest point, or null if no point was found.</returns>
    public static Point3D? FindNearestPoint(this Viewport3D viewport, Point position)
    {
        if (FindNearest(viewport, position, out Point3D p, out Vector3D n, out DependencyObject? obj))
        {
            return p;
        }

        return null;
    }

    /// <summary>
    /// Finds the Visual3D that is nearest the specified position.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="position">The position.</param>
    /// <returns>
    /// The nearest visual, or <c>null</c> if no visual was found.
    /// </returns>
    public static Visual3D? FindNearestVisual(this Viewport3D viewport, Point position)
    {
        if (FindNearest(viewport, position, out Point3D p, out Vector3D n, out DependencyObject? obj))
        {
            return obj as Visual3D;
        }

        return null;
    }

    /// <summary>
    /// Gets the camera transform.
    /// </summary>
    /// <param name="viewport3DVisual">The viewport visual.</param>
    /// <returns>The camera transform.</returns>
    public static Matrix3D GetCameraTransform(this Viewport3DVisual viewport3DVisual)
    {
        return viewport3DVisual.Camera.GetTotalTransform(viewport3DVisual.Viewport.Size.Width / viewport3DVisual.Viewport.Size.Height);
    }

    /// <summary>
    /// Gets the camera transform (viewport and projection).
    /// </summary>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <returns>
    /// A <see cref="Matrix3D"/>.
    /// </returns>
    public static Matrix3D GetCameraTransform(this Viewport3D viewport)
    {
        return viewport.Camera.GetTotalTransform(viewport.ActualWidth / viewport.ActualHeight);
    }

    /// <summary>
    /// Gets all lights.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <returns>A sequence of <see cref="Light"/> objects.</returns>
    public static IEnumerable<Light?> GetLights(this Viewport3D viewport)
    {
        var models = SearchFor<Light>(viewport.Children);
        return models.Select(m => m as Light);
    }

    /// <summary>
    /// Gets the ray at the specified position in 2D (screen coordinates).
    /// </summary>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="position">
    /// A 2D point.
    /// </param>
    /// <returns>
    /// A <see cref="Ray3D"/>.
    /// </returns>
    public static Ray3D? GetRay(this Viewport3D viewport, Point position)
    {
        if (!Point2DtoPoint3D(viewport, position, out Point3D point1, out Point3D point2))
        {
            return null;
        }
        var dir = point2 - point1;
        dir.Normalize();
        return new Ray3D { Origin = point1, Direction = dir };
    }

    /// <summary>
    /// Gets the total transform (camera and viewport).
    /// </summary>
    /// <param name="viewport3DVisual">The viewport visual.</param>
    /// <returns>The total transform.</returns>
    public static Matrix3D GetTotalTransform(this Viewport3DVisual viewport3DVisual)
    {
        var m = GetCameraTransform(viewport3DVisual);
        m.Append(GetViewportTransform(viewport3DVisual));
        return m;
    }

    /// <summary>
    /// Gets the total transform (camera and viewport).
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <returns>The total transform.</returns>
    public static Matrix3D GetTotalTransform(this Viewport3D viewport)
    {
        var transform = GetCameraTransform(viewport);
        transform.Append(GetViewportTransform(viewport));
        return transform;
    }

    /// <summary>
    /// Gets the total transform of the specified visual.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="visual">The visual.</param>
    /// <returns>The transform.</returns>
    public static GeneralTransform3D? GetTransform(this Viewport3D viewport, Visual3D visual)
    {
        if (visual == null)
        {
            return null;
        }

        foreach (var ancestor in viewport.Children)
        {
            if (visual.IsDescendantOf(ancestor))
            {
                var g = new GeneralTransform3DGroup();

                // this includes the visual.Transform
                var ta = visual.TransformToAncestor(ancestor);
                if (ta != null)
                {
                    g.Children.Add(ta);
                }

                // add the transform of the top-level ancestor
                g.Children.Add(ancestor.Transform);

                return g;
            }
        }

        return visual.Transform;
    }

    /// <summary>
    /// Gets the view matrix.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <returns>A <see cref="Matrix3D"/>.</returns>
    public static Matrix3D GetViewMatrix(this Viewport3D viewport)
    {
        return viewport.Camera.GetViewMatrix();
    }

    /// <summary>
    /// Gets the projection matrix.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <returns>A <see cref="Matrix3D"/>.</returns>
    public static Matrix3D GetProjectionMatrix(this Viewport3D viewport)
    {
        return viewport.Camera.GetProjectionMatrix(viewport.ActualWidth / viewport.ActualHeight);
    }

    /// <summary>
    /// Gets the viewport transform.
    /// </summary>
    /// <param name="viewport3DVisual">The viewport3DVisual.</param>
    /// <returns>The transform.</returns>
    public static Matrix3D GetViewportTransform(this Viewport3DVisual viewport3DVisual)
    {
        return new Matrix3D(
            viewport3DVisual.Viewport.Width / 2,
            0,
            0,
            0,
            0,
            -viewport3DVisual.Viewport.Height / 2,
            0,
            0,
            0,
            0,
            1,
            0,
            viewport3DVisual.Viewport.X + (viewport3DVisual.Viewport.Width / 2),
            viewport3DVisual.Viewport.Y + (viewport3DVisual.Viewport.Height / 2),
            0,
            1);
    }

    /// <summary>
    /// Gets the viewport transform.
    /// </summary>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <returns>The transform.</returns>
    public static Matrix3D GetViewportTransform(this Viewport3D viewport)
    {
        return new Matrix3D(
            viewport.ActualWidth / 2,
            0,
            0,
            0,
            0,
            -viewport.ActualHeight / 2,
            0,
            0,
            0,
            0,
            1,
            0,
            viewport.ActualWidth / 2,
            viewport.ActualHeight / 2,
            0,
            1);
    }

    /// <summary>
    /// Transforms a position to Point3D at the near and far clipping planes.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="pointIn">The point to transform.</param>
    /// <param name="pointNear">The point at the near clipping plane.</param>
    /// <param name="pointFar">The point at the far clipping plane.</param>
    /// <returns>True if points were found.</returns>
    public static bool Point2DtoPoint3D(this Viewport3D viewport, Point pointIn, out Point3D pointNear, out Point3D pointFar)
    {
        pointNear = new Point3D();
        pointFar = new Point3D();
        var matrixProj = GetProjectionMatrix(viewport);
        var matrixView = (viewport.Camera is not null ? viewport.Camera.Transform.Value : Matrix3D.Identity) * GetViewMatrix(viewport);
        if (viewport.ActualWidth == 0 || viewport.ActualHeight == 0)
        {
            return false;
        }
        if (!matrixView.HasInverse)
        {
            return false;
        }
        matrixView.Invert();
        var v = new Point4D
        {
            X = (2 * pointIn.X / viewport.ActualWidth - 1) / matrixProj.M11,
            Y = -(2 * pointIn.Y / viewport.ActualHeight - 1) / matrixProj.M22,
            Z = 1 / matrixProj.M33,
            W = 1
        };
        var zf = matrixView.Transform(v);
        Point4D zn;
        if (viewport.Camera is PerspectiveCamera)
        {
            zn = new Point4D(matrixView.OffsetX, matrixView.OffsetY, matrixView.OffsetZ, 1);
        }
        else
        {
            v.Z = 0;
            zn = matrixView.Transform(v);
        }
        pointNear = new Point3D(zn.X / zn.W, zn.Y / zn.W, zn.Z / zn.W);
        pointFar = new Point3D(zf.X / zf.W, zf.Y / zf.W, zf.Z / zf.W);
        return true;
    }

    /// <summary>
    /// Transforms a 2D point to a ray.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="pointIn">The point.</param>
    /// <returns>The ray.</returns>
    public static Ray3D? Point2DtoRay3D(this Viewport3D viewport, Point pointIn)
    {
        Point3D pointNear, pointFar;
        if (!Point2DtoPoint3D(viewport, pointIn, out pointNear, out pointFar))
        {
            return null;
        }

        return new Ray3D(pointNear, pointFar);
    }

    /// <summary>
    /// Transforms the Point3D to a Point2D.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="point">The 3D point.</param>
    /// <returns>The point.</returns>
    public static Point Point3DtoPoint2D(this Viewport3D viewport, Point3D point)
    {
        var matrix = GetTotalTransform(viewport);
        var pointTransformed = matrix.Transform(point);
        var pt = new Point(pointTransformed.X, pointTransformed.Y);
        return pt;
    }

    /// <summary>
    /// Transforms the set of Point3D to a set of Point2D.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="points">The set of 3D points.</param>
    /// <returns>The transformed points.</returns>
    public static IEnumerable<Point> Point3DtoPoint2D(this Viewport3D viewport, IEnumerable<Point3D> points)
    {
        var matrix = GetTotalTransform(viewport);
        var pointsTransformed = points.Select(point => matrix.Transform(point));
        return pointsTransformed.Select(point => new Point(point.X, point.Y));
    }

    /// <summary>
    /// Prints the specified viewport.
    /// </summary>
    /// <param name="vp">
    /// The viewport.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    public static void Print(this Viewport3D vp, string description)
    {
        var dlg = new PrintDialog();
        if (dlg.ShowDialog().GetValueOrDefault())
        {
            dlg.PrintVisual(vp, description);
        }
    }

    /// <summary>
    /// Renders the viewport to a bitmap.
    /// </summary>
    /// <param name="view">The viewport.</param>
    /// <param name="background">The background.</param>
    /// <param name="m">The oversampling multiplier.</param>
    /// <returns>A bitmap.</returns>
    public static BitmapSource RenderBitmap(this Viewport3D view, Brush background, int m = 1)
    {
        var target = new WriteableBitmap((int)view.ActualWidth * m, (int)view.ActualHeight * m, 96, 96, PixelFormats.Pbgra32, null);

        var originalCamera = view.Camera;
        var vm = originalCamera.GetViewMatrix();
        double ar = view.ActualWidth / view.ActualHeight;

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < m; j++)
            {
                // change the camera viewport and scaling
                var pm = originalCamera.GetProjectionMatrix(ar);
                if (originalCamera is OrthographicCamera)
                {
                    pm.OffsetX = m - 1 - (i * 2);
                    pm.OffsetY = -(m - 1 - (j * 2));
                }
                else if (originalCamera is PerspectiveCamera)
                {
                    pm.M31 = -(m - 1 - (i * 2));
                    pm.M32 = m - 1 - (j * 2);
                }

                pm.M11 *= m;
                pm.M22 *= m;

                var mc = new MatrixCamera(vm, pm);
                view.Camera = mc;

                var partialBitmap = new RenderTargetBitmap((int)view.ActualWidth, (int)view.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                // render background
                var backgroundRectangle = new Rectangle { Width = partialBitmap.Width, Height = partialBitmap.Height, Fill = background };
                backgroundRectangle.Arrange(new Rect(0, 0, backgroundRectangle.Width, backgroundRectangle.Height));
                partialBitmap.Render(backgroundRectangle);

                // render 3d
                partialBitmap.Render(view);
                partialBitmap.Freeze();
                (partialBitmap.GetType().GetField("_renderTargetBitmap", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(partialBitmap) as IDisposable)?.Dispose(); //https://github.com/dotnet/wpf/issues/3067

                // copy to the target bitmap
                CopyBitmap(partialBitmap, target, (int)(i * view.ActualWidth), (int)(j * view.ActualHeight));
            }
        }

        // restore the camera
        view.Camera = originalCamera;
        return target;
    }

    /// <summary>
    /// Renders the viewport to a bitmap.
    /// </summary>
    /// <param name="view">The viewport.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="background">The background.</param>
    /// <param name="m">The oversampling multiplier.</param>
    /// <returns>A bitmap.</returns>
    public static BitmapSource RenderBitmap(this Viewport3D view, double width, double height, Brush background, int m = 1)
    {
        double w = view.Width;
        double h = view.Height;
        ResizeAndArrange(view, width, height);
        var rtb = RenderBitmap(view, background, m);
        ResizeAndArrange(view, w, h);
        return rtb;
    }

    /// <summary>
    /// Resizes and arranges the viewport.
    /// </summary>
    /// <param name="view">
    /// The viewport.
    /// </param>
    /// <param name="width">
    /// The width.
    /// </param>
    /// <param name="height">
    /// The height.
    /// </param>
    public static void ResizeAndArrange(this Viewport3D view, double width, double height)
    {
        view.Width = width;
        view.Height = height;
        if (double.IsNaN(width) || double.IsNaN(height))
        {
            return;
        }

        view.Measure(new Size(width, height));
        view.Arrange(new Rect(0, 0, width, height));
    }

    /// <summary>
    /// Saves the viewport to a file.
    /// </summary>
    /// <param name="view">The viewport.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="background">The background brush.</param>
    /// <param name="m">The oversampling multiplier.</param>
    /// <param name="format">The output format.</param>
    public static void SaveBitmap(this Viewport3D view, string fileName, Brush? background = null, int m = 1, BitmapExporter.OutputFormat format = BitmapExporter.OutputFormat.Png)
    {
        using (var stream = System.IO.File.Create(fileName))
        {
            SaveBitmap(view, stream, background, m, format);
        }
    }

    /// <summary>
    /// Saves the <see cref="Viewport3D"/> to left/right bitmap files.
    /// </summary>
    /// <param name="view">The viewport.</param>
    /// <param name="fileName">Name of the file. "_L" and "_R" will be appended to the file name.</param>
    /// <param name="stereoBase">The stereo base.</param>
    /// <param name="background">The background brush.</param>
    /// <param name="m">The oversampling multiplier.</param>
    public static void SaveStereoBitmap(this Viewport3D view, string fileName, double stereoBase, Brush? background = null, int m = 1)
    {
        var extension = System.IO.Path.GetExtension(fileName);
        var directory = System.IO.Path.GetDirectoryName(fileName) ?? string.Empty;
        var name = System.IO.Path.GetFileNameWithoutExtension(fileName) ?? string.Empty;
        var leftFileName = System.IO.Path.Combine(directory, name) + "_L" + extension;
        var rightFileName = System.IO.Path.Combine(directory, name) + "_R" + extension;

        var centerCamera = view.Camera as PerspectiveCamera;
        var leftCamera = new PerspectiveCamera();
        var rightCamera = new PerspectiveCamera();
        StereoHelper.UpdateStereoCameras(centerCamera, leftCamera, rightCamera, stereoBase);

        // save the left image
        using (var stream = System.IO.File.Create(leftFileName))
        {
            view.Camera = leftCamera;
            view.SaveBitmap(stream, background, m);
        }

        // save the right image
        using (var stream = System.IO.File.Create(rightFileName))
        {
            view.Camera = rightCamera;
            view.SaveBitmap(stream, background, m);
        }

        // restore original camera
        view.Camera = centerCamera;
    }

    /// <summary>
    /// Saves the <see cref="Viewport3D" /> to a bitmap.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="background">The background brush.</param>
    /// <param name="m">The oversampling multiplier.</param>
    /// <param name="format">The output format.</param>
    public static void SaveBitmap(this Viewport3D view, System.IO.Stream stream, Brush? background = null, int m = 1, BitmapExporter.OutputFormat format = BitmapExporter.OutputFormat.Png)
    {
        var exporter = new BitmapExporter { Background = background, OversamplingMultiplier = m, Format = format };
        exporter.Export(view, stream);
    }

    /// <summary>
    /// Recursive search in a Visual3D collection for objects of given type T
    /// </summary>
    /// <typeparam name="T">The type to search for.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <returns>A list of models.</returns>
    public static IList<Model3D> SearchFor<T>(this IEnumerable<Visual3D> collection)
    {
        var output = new List<Model3D>();
        SearchFor(collection, typeof(T), output);
        return output;
    }

    /// <summary>
    /// Transforms a point from the screen (2D) to a point on plane (3D)
    /// </summary>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="p">
    /// The 2D point.
    /// </param>
    /// <param name="position">
    /// A point in the plane.
    /// </param>
    /// <param name="normal">
    /// The plane normal.
    /// </param>
    /// <returns>
    /// A 3D point.
    /// </returns>
    /// <remarks>
    /// Maps window coordinates to object coordinates like <code>gluUnProject</code>.
    /// </remarks>
    public static Point3D? UnProject(this Viewport3D viewport, Point p, Point3D position, Vector3D normal)
    {
        // https://paulbourke.net/geometry/pointlineplane/
        if (!viewport.Point2DtoPoint3D(p, out Point3D pointNear, out Point3D pointFar))
        {
            return null;
        }
        Vector3D direction = pointFar - pointNear;
        double dn = Vector3D.DotProduct(normal, direction);
        if (dn.Equals(0))
        {
            return null;
        }
        double u = Vector3D.DotProduct(normal, position - pointNear) / dn;
        Point3D intersection = pointNear + (Math.Abs(u) * direction);
        return intersection;
    }

    /// <summary>
    /// Transforms a point from the screen (2D) to a point on the plane trough the camera target point.
    /// </summary>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="p">
    /// The 2D point.
    /// </param>
    /// <returns>
    /// A 3D point.
    /// </returns>
    public static Point3D? UnProject(this Viewport3D viewport, Point p)
    {
        if (viewport.Camera is not ProjectionCamera pc)
        {
            return null;
        }

        return UnProject(viewport, p, pc.Position + pc.LookDirection, pc.LookDirection);
    }

    /// <summary>
    /// Gets the total number of triangles in the viewport.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <returns>The total number of triangles</returns>
    public static int GetTotalNumberOfTriangles(this Viewport3D viewport)
    {
        int count = 0;
        viewport.Children.Traverse<GeometryModel3D>((m, t) =>
        {
            if (m.Geometry is MeshGeometry3D geometry && geometry.TriangleIndices != null)
            {
                count += geometry.TriangleIndices.Count / 3;
            }
        });
        return count;
    }

    /// <summary>
    /// Copies the bitmap.
    /// </summary>
    /// <param name="source">The source bitmap.</param>
    /// <param name="target">The target bitmap.</param>
    /// <param name="x">The x offset.</param>
    /// <param name="y">The y offset.</param>
    private static void CopyBitmap(BitmapSource source, WriteableBitmap target, int x, int y)
    {
        // Calculate stride of source
        int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);

        // Create data array to hold source pixel data
        var data = new byte[stride * source.PixelHeight];

        // Copy source image pixels to the data array
        source.CopyPixels(data, stride, 0);

        // Write the pixel data to the bitmap.
        target.WritePixels(new Int32Rect(x, y, source.PixelWidth, source.PixelHeight), data, stride, 0);
    }

    /// <summary>
    /// Exports the model to a Kerkythea file.
    /// </summary>
    /// <param name="view">The viewport.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="background">The background.</param>
    private static void ExportKerkythea(this Viewport3D view, string fileName, Brush? background)
    {
        ExportKerkythea(view, fileName, background, (int)view.ActualWidth, (int)view.ActualHeight);
    }

    /// <summary>
    /// Exports the model to a Kerkythea file.
    /// </summary>
    /// <param name="view">
    /// The viewport.
    /// </param>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    /// <param name="background">
    /// The background.
    /// </param>
    /// <param name="width">
    /// The width.
    /// </param>
    /// <param name="height">
    /// The height.
    /// </param>
    private static void ExportKerkythea(this Viewport3D view, string fileName, Brush? background, int width, int height)
    {
        var scb = background as SolidColorBrush;
        var backgroundColor = scb != null ? scb.Color : Colors.White;
        var e = new KerkytheaExporter
        {
            Width = width,
            Height = height,
            BackgroundColor = backgroundColor,
            TexturePath = System.IO.Path.GetDirectoryName(fileName)
        };
        using (var stream = System.IO.File.Create(fileName))
        {
            e.Export(view, stream);
        }
    }

    /// <summary>
    /// Exports to an obj file.
    /// </summary>
    /// <param name="view">
    /// The viewport.
    /// </param>
    /// <param name="path">
    /// Name of the file.
    /// </param>
    private static void ExportObj(this Viewport3D view, string path)
    {
        var dir = System.IO.Path.GetDirectoryName(path) ?? ".";
        var filename = System.IO.Path.GetFileName(path);

        var e = new ObjExporter
        {
            TextureFolder = dir,
            FileCreator = f => System.IO.File.Create(System.IO.Path.Combine(dir, f))
        };

        using (var stream = System.IO.File.Create(path))
        {
            e.MaterialsFile = System.IO.Path.ChangeExtension(filename, ".mtl");
            e.Export(view, stream);
        }
    }

    /// <summary>
    /// Exports to an X3D file.
    /// </summary>
    /// <param name="view">
    /// The viewport.
    /// </param>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    private static void ExportX3D(this Viewport3D view, string fileName)
    {
        var e = new X3DExporter();
        using (var stream = System.IO.File.Create(fileName))
        {
            e.Export(view, stream);
        }
    }

    /// <summary>
    /// Exports to a COLLADA file.
    /// </summary>
    /// <param name="view">The viewport.</param>
    /// <param name="fileName">Name of the file.</param>
    private static void ExportCollada(this Viewport3D view, string fileName)
    {
        var e = new ColladaExporter();
        using (var stream = System.IO.File.Create(fileName))
        {
            e.Export(view, stream);
        }
    }

    /// <summary>
    /// Exports to a STL file.
    /// </summary>
    /// <param name="view">The viewport.</param>
    /// <param name="fileName">Name of the file.</param>
    private static void ExportStl(this Viewport3D view, string fileName)
    {
        var e = new StlExporter();
        using (var stream = System.IO.File.Create(fileName))
        {
            e.Export(view, stream);
        }
    }

    /// <summary>
    /// Exports to xaml.
    /// </summary>
    /// <param name="view">
    /// The viewport.
    /// </param>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    private static void ExportXaml(this Viewport3D view, string fileName)
    {
        var e = new XamlExporter();
        using (var stream = System.IO.File.Create(fileName))
        {
            e.Export(view, stream);
        }
    }

    /// <summary>
    /// Gets the hit position transformed to global (viewport) coordinates.
    /// </summary>
    /// <param name="rayHit">
    /// The hit structure.
    /// </param>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <returns>
    /// The 3D position of the hit.
    /// </returns>
    private static Point3D GetGlobalHitPosition(RayHitTestResult rayHit, Viewport3D viewport)
    {
        // PointHit is in Visual3D space
        var p = rayHit.PointHit;

        // transform the Visual3D hierarchy up to the Viewport3D ancestor
        var t = GetTransform(viewport, rayHit.VisualHit);
        if (t != null)
        {
            p = t.Transform(p);
        }

        return p;
    }

    /// <summary>
    /// Gets the normal for a hit test result.
    /// </summary>
    /// <param name="rayHit">
    /// The ray hit.
    /// </param>
    /// <returns>
    /// The normal.
    /// </returns>
    private static Vector3D? GetNormalHit(RayMeshGeometry3DHitTestResult rayHit)
    {
        if ((rayHit.MeshHit.Normals == null) || (rayHit.MeshHit.Normals.Count < 1))
        {
            return null;
        }

        return (rayHit.MeshHit.Normals[rayHit.VertexIndex1] * rayHit.VertexWeight1)
               + (rayHit.MeshHit.Normals[rayHit.VertexIndex2] * rayHit.VertexWeight2)
               + (rayHit.MeshHit.Normals[rayHit.VertexIndex3] * rayHit.VertexWeight3);
    }

    /// <summary>
    /// Recursive search for an object of a given type
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="type">The type.</param>
    /// <param name="output">The output.</param>
    private static void SearchFor(IEnumerable<Visual3D> collection, Type type, IList<Model3D> output)
    {
        // TODO: change to use Stack/Queue
        foreach (var visual in collection)
        {
            if (visual is ModelVisual3D modelVisual)
            {
                var model = modelVisual.Content;
                if (model != null)
                {
                    if (type.IsInstanceOfType(model))
                    {
                        output.Add(model);
                    }

                    // recursive
                    SearchFor(modelVisual.Children, type, output);
                }

                if (model is Model3DGroup modelGroup)
                {
                    SearchFor(modelGroup.Children, type, output);
                }
            }
        }
    }

    /// <summary>
    /// Searches for models of the specified type.
    /// </summary>
    /// <param name="collection">
    /// The collection.
    /// </param>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    private static void SearchFor(IEnumerable<Model3D> collection, Type type, IList<Model3D> output)
    {
        foreach (var model in collection)
        {
            if (type.IsInstanceOfType(model))
            {
                output.Add(model);
            }

            var group = model as Model3DGroup;
            if (group != null)
            {
                SearchFor(group.Children, type, output);
            }
        }
    }
}
