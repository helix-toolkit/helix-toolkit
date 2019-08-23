// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementSortingHelper.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Sorts element by opacity and distance from camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Sorts element by opacity and distance from camera.
    /// </summary>
    public static class ElementSortingHelper
    {
        // http://blogs.msdn.com/cfs-file.ashx/__key/CommunityServer-Components-PostAttachments/00-04-01-86-12/SceneSortingHelper_2E00_cs
        /// <summary>
        /// Sort Modelgroups in Farthest to Closest order, to enable transparency
        /// Should be applied whenever the scene is significantly re-oriented
        /// </summary>
        /// <param name="cameraPosition">
        /// The camera Position.
        /// </param>
        /// <param name="models">
        /// The models.
        /// </param>
        /// <param name="worldTransform">
        /// The world Transform.
        /// </param>
        public static void AlphaSort(Point3D cameraPosition, Model3DCollection models, Transform3D worldTransform)
        {
            var sortedList =
                models.OrderBy(
                    model => Point3D.Subtract(cameraPosition, worldTransform.Transform(model.Bounds.Location)).Length);
            models.Clear();
            foreach (var model in sortedList)
            {
                models.Add(model);
            }
        }

        /// <summary>
        /// Gets the distance squared.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <returns>
        /// The get distance squared.
        /// </returns>
        public static double GetDistanceSquared(Point3D position, Visual3D visual)
        {
            var bounds = Visual3DHelper.FindBounds(visual, Transform3D.Identity);
            return Point3D.Subtract(bounds.Location, position).LengthSquared;
        }

        /// <summary>
        /// Gets the distance squared.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The get distance squared.
        /// </returns>
        public static double GetDistanceSquared(Point3D position, GeometryModel3D model)
        {
            return Point3D.Subtract(model.Bounds.Location, position).LengthSquared;
        }

        /// <summary>
        /// Determines whether the specified visual is transparent.
        /// </summary>
        /// <param name="v">
        /// The v.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified visual is transparent; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTransparent(Visual3D v)
        {
            var mv3D = v as ModelVisual3D;
            if (mv3D != null)
            {
                // check if Model3D is transparent
                if (IsTransparent(mv3D.Content))
                {
                    return true;
                }

                // check if any child Visual3D are transparent
                return mv3D.Children.Any(IsTransparent);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified model is transparent.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified model is transparent; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTransparent(Model3D model)
        {
            var gm3D = model as GeometryModel3D;
            if (gm3D != null)
            {
                if (IsTransparent(gm3D))
                {
                    return true;
                }
            }

            var mg = model as Model3DGroup;
            if (mg != null)
            {
                return mg.Children.Any(IsTransparent);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified model is transparent.
        /// </summary>
        /// <param name="gm3D">
        /// The GM3 D.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified GM3 D is transparent; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTransparent(GeometryModel3D gm3D)
        {
            if (IsTransparent(gm3D.Material))
            {
                return true;
            }

            if (IsTransparent(gm3D.BackMaterial))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether any part of the specified material is transparent.
        /// </summary>
        /// <param name="material">
        /// The material.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified material is transparent; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTransparent(Material material)
        {
            var g = material as MaterialGroup;
            if (g != null)
            {
                if (g.Children.Any(IsTransparent))
                {
                    return true;
                }
            }

            var dm = material as DiffuseMaterial;
            if (dm != null)
            {
                if (IsTransparent(dm.Brush))
                {
                    return true;
                }

                if (dm.Color.A < 255)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified brush is transparent.
        /// </summary>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified brush is transparent; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTransparent(Brush brush)
        {
            if (brush.Opacity < 1)
            {
                return true;
            }

            var scb = brush as SolidColorBrush;
            if (scb != null)
            {
                return scb.Color.A < 255;
            }

            var gb = brush as GradientBrush;
            if (gb != null)
            {
                return gb.GradientStops.Any(gs => gs.Color.A < 255);
            }

            // todo: tilebrush and bitmapcachebrush
            return false;
        }

        /// <summary>
        /// Sort scene - first opaque objects, then transparent objects sorted by distance from camera
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        public static void SortModel(Point3D position, IList<Visual3D> model)
        {
            var opaqueObjects = new List<Visual3D>();
            var transparentObjects = new List<Visual3D>();
            foreach (var v in model)
            {
                if (IsTransparent(v))
                {
                    transparentObjects.Add(v);
                }
                else
                {
                    opaqueObjects.Add(v);
                }
            }

            model.Clear();

            // Sort transparent objects by distance
            var sortedTransparentObjects = transparentObjects.OrderBy(visual => GetDistanceSquared(position, visual));

            foreach (var v in opaqueObjects)
            {
                model.Add(v);
            }

            foreach (var v in sortedTransparentObjects)
            {
                model.Add(v);
            }
        }

    }
}