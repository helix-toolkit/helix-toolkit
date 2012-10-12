// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Visual3DHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Helper methods for <see cref="Visual3D"/> objects.
    /// </summary>
    public static class Visual3DHelper
    {
        /// <summary>
        /// The visual 3 d model property info.
        /// </summary>
        private static readonly PropertyInfo Visual3DModelPropertyInfo = typeof(Visual3D).GetProperty(
            "Visual3DModel", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Finds a child of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <returns>
        /// </returns>
        public static T Find<T>(DependencyObject parent) where T : DependencyObject
        {
            // todo: this should be improved
            foreach (DependencyObject d in LogicalTreeHelper.GetChildren(parent))
            {
                var a = Find<T>(d);
                if (a != null)
                {
                    return a;
                }
            }

            var model = parent as ModelVisual3D;
            if (model != null)
            {
                var modelgroup = model.Content as Model3DGroup;
                if (modelgroup != null)
                {
                    return modelgroup.Children.OfType<T>().FirstOrDefault();
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the bounding box for a collection of Visual3Ds.
        /// </summary>
        /// <param name="children">
        /// The children.
        /// </param>
        /// <returns>
        /// </returns>
        public static Rect3D FindBounds(Visual3DCollection children)
        {
            var bounds = Rect3D.Empty;
            foreach (var visual in children)
            {
                var b = FindBounds(visual, Transform3D.Identity);
                bounds.Union(b);
            }

            return bounds;
        }

        /// <summary>
        /// Finds the bounding box for the specified visual.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <param name="transform">
        /// The transform of visual.
        /// </param>
        /// <returns>
        /// </returns>
        public static Rect3D FindBounds(Visual3D visual, Transform3D transform)
        {
            var bounds = Rect3D.Empty;
            var childTransform = Transform3DHelper.CombineTransform(visual.Transform, transform);
            var model = GetModel(visual);
            if (model != null)
            {
                // apply transform
                var transformedBounds = childTransform.TransformBounds(model.Bounds);
                if (!double.IsNaN(transformedBounds.X))
                {
                    bounds.Union(transformedBounds);
                }
            }

            foreach (var child in GetChildren(visual))
            {
                var b = FindBounds(child, childTransform);
                bounds.Union(b);
            }

            return bounds;
        }

        /// <summary>
        /// Gets the transform for the specified visual.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <returns>
        /// </returns>
        public static Matrix3D GetTransform(Visual3D visual)
        {
            var totalTransform = Matrix3D.Identity;

            DependencyObject obj = visual;
            while (obj != null)
            {
                var viewport3DVisual = obj as Viewport3DVisual;
                if (viewport3DVisual != null)
                {
                    return totalTransform;
                }

                var mv = obj as ModelVisual3D;
                if (mv != null)
                {
                    if (mv.Transform != null)
                    {
                        totalTransform.Append(mv.Transform.Value);
                    }
                }

                obj = VisualTreeHelper.GetParent(obj);
            }

            throw new InvalidOperationException("The visual is not added to a Viewport3D.");
        }

        /// <summary>
        /// Gets the Viewport3D from the specified visual.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <returns>
        /// The Viewport3D
        /// </returns>
        public static Viewport3D GetViewport3D(Visual3D visual)
        {
            DependencyObject obj = visual;
            while (obj != null)
            {
                var vis = obj as Viewport3DVisual;
                if (vis != null)
                {
                    return VisualTreeHelper.GetParent(obj) as Viewport3D;
                }

                obj = VisualTreeHelper.GetParent(obj);
            }

            return null;
        }

        /// <summary>
        /// Gets the transform to viewport space.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <returns>
        /// A transformation matrix.
        /// </returns>
        public static Matrix3D GetViewportTransform(Visual3D visual)
        {
            var totalTransform = Matrix3D.Identity;

            DependencyObject obj = visual;
            while (obj != null)
            {
                var viewport3DVisual = obj as Viewport3DVisual;
                if (viewport3DVisual != null)
                {
                    var matxViewport = Viewport3DHelper.GetTotalTransform(viewport3DVisual);
                    totalTransform.Append(matxViewport);
                    return totalTransform;
                }

                var mv = obj as ModelVisual3D;
                if (mv != null)
                {
                    if (mv.Transform != null)
                    {
                        totalTransform.Append(mv.Transform.Value);
                    }
                }

                obj = VisualTreeHelper.GetParent(obj);
            }

            throw new InvalidOperationException("The visual is not added to a Viewport3D.");

            // At this point, we know obj is Viewport3DVisual
        }

        /// <summary>
        /// Determines whether the visual is attached to a Viewport3D.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <returns>
        /// The is attached to viewport 3 d.
        /// </returns>
        public static bool IsAttachedToViewport3D(Visual3D visual)
        {
            DependencyObject obj = visual;
            while (obj != null)
            {
                var vis = obj as Viewport3DVisual;
                if (vis != null)
                {
                    return true;
                }

                obj = VisualTreeHelper.GetParent(obj);
            }

            return false;
        }

        /// <summary>
        /// Traverses the Visual3D/Model3D tree. Run the specified action for each Model3D.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="visuals">
        /// The visuals.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(Visual3DCollection visuals, Action<T, Transform3D> action) where T : Model3D
        {
            foreach (var child in visuals)
            {
                Traverse(child, action);
            }
        }

        /// <summary>
        /// Traverses the Visual3D/Model3D tree. Run the specified action for each Model3D.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(Visual3D visual, Action<T, Transform3D> action) where T : Model3D
        {
            Traverse(visual, Transform3D.Identity, action);
        }

        /// <summary>
        /// Traverses the Model3D tree. Run the specified action for each Model3D.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void TraverseModel<T>(Model3D model, Action<T, Transform3D> action) where T : Model3D
        {
            TraverseModel(model, Transform3D.Identity, action);
        }

        /// <summary>
        /// Traverses the Model3D tree. Run the specified action for each Model3D.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void TraverseModel<T>(Model3D model, Transform3D transform, Action<T, Transform3D> action)
            where T : Model3D
        {
            var mg = model as Model3DGroup;
            if (mg != null)
            {
                var childTransform = Transform3DHelper.CombineTransform(model.Transform, transform);
                foreach (var m in mg.Children)
                {
                    TraverseModel(m, childTransform, action);
                }
            }

            var gm = model as T;
            if (gm != null)
            {
                var childTransform = Transform3DHelper.CombineTransform(model.Transform, transform);
                action(gm, childTransform);
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <returns>
        /// </returns>
        private static IEnumerable<Visual3D> GetChildren(Visual3D visual)
        {
            int n = VisualTreeHelper.GetChildrenCount(visual);
            for (int i = 0; i < n; i++)
            {
                var child = VisualTreeHelper.GetChild(visual, i) as Visual3D;
                if (child == null)
                {
                    continue;
                }

                yield return child;
            }
        }

        /// <summary>
        /// Gets the model for the specified Visual3D.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <returns>
        /// </returns>
        private static Model3D GetModel(Visual3D visual)
        {
            Model3D model;
            var mv = visual as ModelVisual3D;
            if (mv != null)
            {
                model = mv.Content;
            }
            else
            {
                model = Visual3DModelPropertyInfo.GetValue(visual, null) as Model3D;
            }

            return model;
        }

        /// <summary>
        /// Traverses the specified visual.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        private static void Traverse<T>(Visual3D visual, Transform3D transform, Action<T, Transform3D> action)
            where T : Model3D
        {
            var childTransform = Transform3DHelper.CombineTransform(visual.Transform, transform);
            var model = GetModel(visual);
            if (model != null)
            {
                TraverseModel(model, childTransform, action);
            }

            foreach (var child in GetChildren(visual))
            {
                Traverse(child, childTransform, action);
            }
        }

    }
}