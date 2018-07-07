// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewportExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides extension methods for Viewport3DX.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using Matrix = System.Numerics.Matrix4x4;
    using Plane = System.Numerics.Plane;
    using Ray = Mathematics.Ray;
    using Vector2 = System.Numerics.Vector2;
    using Vector3 = System.Numerics.Vector3;
    using BoundingBox = Mathematics.BoundingBox;
    using Media3D = System.Windows.Media.Media3D;
    using Cameras;
    using Model.Scene;
    using global::SharpDX.Direct3D11;

    /// <summary>
    /// Provides extension methods for <see cref="Viewport3DX" />.
    /// </summary>
    public static class ViewportExtensions
    {
        /// <summary>
        /// Gets the total number of triangles in the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total number of triangles</returns>        
        public static int GetTotalNumberOfTriangles(this Viewport3DX viewport)
        {
            int count = 0;
            var totalModel = viewport.Renderables.PreorderDFT((x) => 
            {
                if(x is GeometryNode g)
                {
                    if (g.Visible && g.Geometry != null && g.Geometry.Indices != null)
                    {
                        count += g.Geometry.Indices.Count / 3;
                    }
                }
                return true;
            }).Count();
            return count;
        }

        /// <summary>
        /// Copies the specified viewport to the clipboard.
        /// </summary>
        public static void Copy(this Viewport3DX view)
        {
            Clipboard.SetImage(RenderBitmap(view));
        }

        /// <summary>
        /// Gets the camera transform.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>
        /// The camera transform.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetViewProjectionMatrix(this Viewport3DX viewport)
        {
            return viewport.RenderContext != null ? viewport.RenderContext.ViewMatrix * viewport.RenderContext.ProjectionMatrix
                : viewport.CameraCore.GetViewProjectionMatrix(viewport.ActualWidth / viewport.ActualHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetProjectionMatrix(this Viewport3DX viewport)
        {
            return viewport.RenderContext != null ? viewport.RenderContext.ProjectionMatrix
                : viewport.CameraCore.GetProjectionMatrix(viewport.ActualWidth / viewport.ActualHeight);
        }

        /// <summary>
        /// Gets the total transform for a Viewport3DX. 
        /// Old name of this function: GetTotalTransform
        /// New name of the function: GetScreenViewProjectionTransform
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total transform.</returns>       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetScreenViewProjectionMatrix(this Viewport3DX viewport)
        {
            return GetViewProjectionMatrix(viewport) * GetViewportMatrix(viewport);
        }

        /// <summary>
        /// Gets the viewport transform aka the screen-space transform.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>The transform.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetViewportMatrix(this Viewport3DX viewport)
        {
            return new Matrix(
                (float)(viewport.ActualWidth / 2),
                0,
                0,
                0,
                0,
                (float)(-viewport.ActualHeight / 2),
                0,
                0,
                0,
                0,
                1,
                0,
                (float)((viewport.ActualWidth - 1) / 2),
                (float)((viewport.ActualHeight - 1) / 2),
                0,
                1);
        }

        /// <summary>
        /// Finds the bounding box of the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox FindBounds(this Viewport3DX viewport)
        {
            if (viewport.RenderHost != null && viewport.RenderHost.IsRendering)
            {
                viewport.RenderHost.UpdateAndRender();
            }
            var maxVector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var firstModel = viewport.Renderables.PreorderDFT((r) =>
            {
                if (r.Visible && !(r is ScreenSpacedNode))
                {
                    return true;
                }
                return false;
            }).Where(x =>
            {
                if (x is IBoundable b)
                {
                    return b.HasBound && b.BoundsWithTransform.Maximum != b.BoundsWithTransform.Minimum 
                    && b.BoundsWithTransform.Maximum != Vector3.Zero && b.BoundsWithTransform.Maximum != maxVector;
                }
                else
                {
                    return false;
                }
            }).FirstOrDefault();
            if(firstModel == null)
            {
                return new BoundingBox();
            }
            var bounds = firstModel.BoundsWithTransform;
            
            foreach(var renderable in viewport.Renderables.PreorderDFT((r) =>
            {               
                if (r.Visible && !(r is ScreenSpacedNode))
                {
                    return true;
                }
                return false;
            }))
            {
                if(renderable is IBoundable r)
                {
                    if (r.HasBound && r.BoundsWithTransform.Maximum != maxVector)
                    {
                        bounds = BoundingBox.Merge(bounds, r.BoundsWithTransform);
                    }
                }
            }
            return bounds;
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type filter.
        /// </typeparam>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(this Viewport3DX viewport, Action<T, Media3D.Transform3D> action) where T : Element3D
        {
            viewport.Renderables.PreorderDFT((node) => 
            {
                if(node.WrapperSource is T element)
                {
                    action(element, element.Transform);
                }
                return true;
            });
        }

        /// <summary>
        /// Traverses the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewport">The viewport.</param>
        /// <param name="function">The function. Return true to continue traverse, otherwise stop at current node</param>
        public static void Traverse<T>(this Viewport3DX viewport, Func<T, bool> function) where T : Element3D
        {
            viewport.Renderables.PreorderDFT((node) =>
            {
                if (node.WrapperSource is T element)
                {
                    return function(element);
                }
                return true;
            });
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type filter.
        /// </typeparam>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(this Element3D element, Action<T, Media3D.Transform3D> action) where T : Element3D
        {
            var sceneNode = new SceneNode[] { element.SceneNode };
            Traverse(element, action);
        }

        public static readonly HitTestResult[] EmptyHits = new HitTestResult[0];
        /// <summary>
        /// Finds the hits for a given 2D viewport position.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <returns>
        /// List of hits, sorted with the nearest hit first.
        /// </returns>
        public static IList<HitTestResult> FindHits(this Viewport3DX viewport, Point position)
        {
            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
            {
                return EmptyHits;
            }

            var ray = UnProject(viewport, new Vector2((float)position.X, (float)position.Y));
            var hits = new List<HitTestResult>();

            foreach (var element in viewport.Renderables)
            {
                element.HitTest(viewport.RenderContext, ray, ref hits);
            }
            hits.Sort();
            return hits;
        }

        /// <summary>
        /// Finds the nearest point and its normal.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <param name="normal">
        /// The normal.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The find nearest.
        /// </returns>
        public static bool FindNearest(this Viewport3DX viewport, Point position,
            out Vector3 point, out Vector3 normal, out Element3D model)
        {
            point = new Vector3();
            normal = new Vector3();
            model = null;

            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
            {
                return false;
            }

            var hits = FindHits(viewport, position);
            if (hits.Count > 0)
            {
                point = hits[0].PointHit;
                normal = hits[0].NormalAtHit;
                model = hits[0].ModelHit as Element3D;
                return true;
            }
            else
            {
                // check for nearest points in the scene
                // TODO!!
                return false;
            }
        }

        /// <summary>
        /// Find the coordinates of the nearest point given a 2D position in the viewport
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="position">The position.</param>
        /// <returns>The nearest point, or null if no point was found.</returns>
        public static Vector3? FindNearestPoint(this Viewport3DX viewport, Point position)
        {
            Vector3 p;
            Vector3 n;
            Element3D obj;
            if (FindNearest(viewport, position, out p, out n, out obj))
            {
                return p;
            }
            return null;
        }

        /// <summary>
        /// Un-projects a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The input point.</param>
        /// <returns>The ray.</returns>
        public static Ray UnProject(this Viewport3DX viewport, Vector2 point2d)//, out Vector3 pointNear, out Vector3 pointFar)
        {
            var camera = viewport.CameraCore as ProjectionCameraCore;
            if (camera != null)
            {
                var px = (float)point2d.X;
                var py = (float)point2d.Y;

                var viewMatrix = camera.GetViewMatrix();

                
                var matrix = MatrixExtensions.PsudoInvert(ref viewMatrix);
                float w = (float)viewport.ActualWidth;
                float h = (float)viewport.ActualHeight;
                var aspectRatio = w / h;

                var projMatrix = camera.GetProjectionMatrix(aspectRatio);

                Vector3 v = new Vector3
                {
                    X = (2 * px / w - 1) / projMatrix.M11,
                    Y = -(2 * py / h - 1) / projMatrix.M22,
                    Z = 1 / projMatrix.M33
                };
                Vector3 zn;
                Mathematics.Vector3Helper.TransformCoordinate(ref v, ref matrix, out Vector3 zf);

                if (camera is PerspectiveCameraCore)
                {
                    zn = camera.Position;
                }
                else
                {
                    v.Z = 0;
                    Mathematics.Vector3Helper.TransformCoordinate(ref v, ref matrix, out zn);
                }
                Vector3 r = Vector3.Normalize(zf - zn);             

                return new Ray(zn + r * camera.NearPlaneDistance, r);
            }
            throw new HelixToolkitException("Unproject camera error.");
        }

        /// <summary>
        /// Un-projects a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The input point.</param>
        /// <returns>The ray.</returns>
        public static Ray UnProject(this Viewport3DX viewport, Point point2d)
        {
            //Vector3 p0, p1;
            var r = UnProject(viewport, point2d.ToVector2());//, out p0, out p1);
            return new Ray(r.Position, r.Direction);
        }

        /// <summary>
        /// Un-projects the specified 2D screen point to a ray.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The point.</param>
        /// <returns>The ray.</returns>
        public static Ray UnProjectToRay(this Viewport3DX viewport, Point point2d)
        {
            var r = viewport.UnProject(point2d.ToVector2());            
            return new Ray(r.Position, r.Direction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="point2d"></param>
        /// <returns></returns>
        public static Ray UnProjectToRay(this Viewport3DX viewport, Vector2 point2d)
        {
            var r = viewport.UnProject(point2d);
            return new Ray(r.Position, r.Direction);
        }

        /// <summary>
        /// Un-project a point from the screen (2D) to a point on plane (3D)
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="p">
        /// The 2D point.
        /// </param>
        /// <param name="position">
        /// plane position
        /// </param>
        /// <param name="normal">
        /// plane normal
        /// </param>
        /// <returns>
        /// A 3D point.
        /// </returns>
        public static Vector3? UnProjectOnPlane(this Viewport3DX viewport, Point p, Vector3 position, Vector3 normal)
        {
            var ray = UnProjectToRay(viewport, p);
            if (ray == null)
            {
                return null;
            }
            var plane = Mathematics.PlaneHelper.GetPlane(position, normal);
            if(ray.Intersects(ref plane, out Vector3 intersect))
            {
                return intersect;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="p"></param>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3? UnProjectOnPlane(this Viewport3DX viewport, Vector2 p, Vector3 position, Vector3 normal)
        {            
            var plane = Mathematics.PlaneHelper.GetPlane(position, normal);
            return UnProjectOnPlane(viewport, p, plane);
        }
          
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="p"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Vector3? UnProjectOnPlane(this Viewport3DX viewport, Vector2 p, Plane plane)
        {
            var ray = UnProjectToRay(viewport, p);
            Vector3 hitPoint;
            if (ray.Intersects(ref plane, out hitPoint))
            {
                return hitPoint;
            }
            return null;
        }

        /// <summary>
        /// Un-projects a point from the screen (2D) to a point on the plane trough the camera target point.
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
        public static Vector3? UnProjectOnPlane(this Viewport3DX viewport, Point p)
        {
            var pc = viewport.Camera as ProjectionCamera;
            if (pc == null)
            {
                return null;
            }
            return UnProjectOnPlane(viewport, p, pc.CameraInternal.Position + pc.CameraInternal.LookDirection, pc.CameraInternal.LookDirection);
        }

        /// <summary>
        /// Projects the specified 3D point to a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point">The 3D point.</param>
        /// <returns>The point.</returns>
        public static Point Project(this Viewport3DX viewport, Vector3 point)
        {
            var matrix = GetScreenViewProjectionMatrix(viewport);
            var pointTransformed = Mathematics.Vector3Helper.TransformCoordinate(point, matrix);
            var pt = new Point(pointTransformed.X, pointTransformed.Y);
            return pt;
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
        public static void Print(this Viewport3DX vp, string description)
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
        /// <returns>A bitmap.</returns>
        public static BitmapSource RenderBitmap(this Viewport3DX view)
        {
            if (view.RenderHost != null && view.RenderHost.IsRendering)
            {
                view.RenderHost.UpdateAndRender();
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    if (view.RenderHost != null && view.RenderHost.IsRendering)
                    {
                        Utilities.ScreenCapture.SaveWICTextureToBitmapStream(view.RenderHost.EffectsManager, view.RenderHost.RenderBuffer.BackBuffer.Resource as Texture2D, memoryStream);
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        memoryStream.Position = 0;
                        bitmap.StreamSource = memoryStream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Renders the viewport to a bitmap.
        /// </summary>
        /// <param name="view">The viewport.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>A bitmap.</returns>
        public static BitmapSource RenderBitmap(
            this Viewport3DX view, double width, double height)
        {
            double w = view.Width;
            double h = view.Height;
            ResizeAndArrange(view, width, height);
            var rtb = RenderBitmap(view);
            ResizeAndArrange(view, w, h);
            return rtb;
        }

        /// <summary>
        /// Resizes and arranges the viewport.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public static void ResizeAndArrange(this Viewport3DX view, double width, double height)
        {
            view.Width = width;
            view.Height = height;
            if (double.IsNaN(width) || double.IsNaN(height))
            {
                return;
            }

            view.Measure(new Size(width, height));
            view.Arrange(new Rect(0, 0, width, height));
            view.RenderHost.Resize(width, height);
        }


        /// <summary>
        /// Saves the bitmap.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void SaveScreen(this Viewport3DX view, string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName);
            Direct2DImageFormat format = Direct2DImageFormat.Bmp;
            switch (ext)
            {
                case "bmp":
                    format = Direct2DImageFormat.Bmp;
                    break;
                case "jpeg":
                case "jpg":
                    format = Direct2DImageFormat.Jpeg;
                    break;
                case "png":
                    format = Direct2DImageFormat.Png;
                    break;
                default:
                    break;
            }
            SaveScreen(view, fileName, format);
        }

        /// <summary>
        /// Saves the bitmap.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="format">The format.</param>
        public static void SaveScreen(this Viewport3DX view, string fileName, Direct2DImageFormat format)
        {
            using (var file = System.IO.File.OpenWrite(fileName))
            {
                if (!file.CanWrite)
                {
                    throw new AccessViolationException($"File cannot be written. {fileName}");
                }
            }
            if (view.RenderHost != null && view.RenderHost.IsRendering)
            {
                view.RenderHost.UpdateAndRender();
                Utilities.ScreenCapture.SaveWICTextureToFile(view.RenderHost.EffectsManager, view.RenderHost.RenderBuffer.BackBuffer.Resource as Texture2D, fileName, format);
            }
        }

        /// <summary>
        /// Zooms to the extents of the specified viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="animationTime">The animation time.</param>
        public static void ZoomExtents(this Viewport3DX viewport, double animationTime = 0)
        {
            var bounds = viewport.FindBounds();
            var diagonal = bounds.Maximum - bounds.Minimum;

            if (diagonal.LengthSquared().Equals(0))
            {
                return;
            }

            ZoomExtents(viewport, bounds, animationTime);
        }

        /// <summary>
        /// Zooms to the extents of the specified bounding box.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="bounds">The bounding rectangle.</param>
        /// <param name="animationTime">The animation time.</param>
        public static void ZoomExtents(this Viewport3DX viewport, BoundingBox bounds, double animationTime = 0)
        {
            var diagonal = bounds.Maximum - bounds.Minimum;
            var center = bounds.Center;
            double radius = diagonal.Length() * 0.5;
            ZoomExtents(viewport, center, radius, animationTime);
        }

        /// <summary>
        /// Zooms to the extents of the specified bounding sphere.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="animationTime">The animation time.</param>
        public static void ZoomExtents(this Viewport3DX viewport, Vector3 center, double radius, double animationTime = 0)
        {
            var camera = viewport.Camera;
            var pcam = camera as PerspectiveCamera;
            if (pcam != null)
            {
                double disth = radius / Math.Tan(0.5 * pcam.FieldOfView * Math.PI / 180);
                double vfov = pcam.FieldOfView / viewport.ActualWidth * viewport.ActualHeight;
                double distv = radius / Math.Tan(0.5 * vfov * Math.PI / 180);

                double dist = Math.Max(disth, distv);
                var dir = pcam.LookDirection;
                dir.Normalize();
                pcam.LookAt(center.ToPoint3D(), dir * dist, animationTime);
            }

            var ocam = camera as OrthographicCamera;
            if (ocam != null)
            {
                ocam.LookAt(center.ToPoint3D(), ocam.LookDirection, animationTime);
                double newWidth = radius * 2;

                if (viewport.ActualWidth > viewport.ActualHeight)
                {
                    newWidth = radius * 2 * viewport.ActualWidth / viewport.ActualHeight;
                }

                ocam.AnimateWidth(newWidth, animationTime);
            }
        }

        /// <summary>
        /// Zooms the viewport to the specified rectangle.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="rectangle">The rectangle.</param>
        public static void ZoomToRectangle(this Viewport3DX viewport, Rect rectangle)
        {
            var pcam = viewport.Camera as ProjectionCamera;
            if (pcam != null)
            {
                pcam.ZoomToRectangle(viewport, rectangle);
            }
        }

        /// <summary>
        /// Changes the field of view and tries to keep the scale fixed.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="delta">
        /// The relative change in fov.
        /// </param>
        public static void ZoomByChangingFieldOfView(this Viewport3DX viewport, double delta)
        {
            var pcamera = viewport.Camera as PerspectiveCamera;
            if (pcamera == null || !viewport.IsChangeFieldOfViewEnabled)
            {
                return;
            }

            double fov = pcamera.FieldOfView;
            double d = pcamera.LookDirection.Length;
            double r = d * Math.Tan(0.5 * fov / 180 * Math.PI);

            fov *= 1 + (delta * 0.5);
            if (fov < viewport.MinimumFieldOfView)
            {
                fov = viewport.MinimumFieldOfView;
            }

            if (fov > viewport.MaximumFieldOfView)
            {
                fov = viewport.MaximumFieldOfView;
            }

            pcamera.FieldOfView = fov;
            double d2 = r / Math.Tan(0.5 * fov / 180 * Math.PI);
            var newLookDirection = pcamera.LookDirection;
            newLookDirection.Normalize();
            newLookDirection *= d2;
            var target = pcamera.Position + pcamera.LookDirection;
            pcamera.Position = target - newLookDirection;
            pcamera.LookDirection = newLookDirection;
        }

        /// <summary>
        /// Copies the source bitmap to the specified position in the target bitmap.
        /// </summary>
        /// <param name="source">The source bitmap.</param>
        /// <param name="target">The target bitmap.</param>
        /// <param name="offsetx">The x offset.</param>
        /// <param name="offsety">The y offset.</param>
        private static void CopyBitmap(BitmapSource source, WriteableBitmap target, int offsetx, int offsety)
        {
            // Calculate stride of source
            int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);

            // Create data array to hold source pixel data
            var data = new byte[stride * source.PixelHeight];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // Write the pixel data to the WriteableBitmap.
            target.WritePixels(new Int32Rect(offsetx, offsety, source.PixelWidth, source.PixelHeight), data, stride, 0);
        }
    }
}