// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Model3DHelper.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides extension methods for Model3D objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides extension methods for <see cref="Model3D"/> objects.
    /// </summary>
    public static class Model3DHelper
    {
        /// <summary>
        /// Gets the transform.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="parentTransform">
        /// The parent transform.
        /// </param>
        /// <returns>
        /// The transform.
        /// </returns>
        public static GeneralTransform3D GetTransform(this Model3D current, Model3D model, Transform3D parentTransform)
        {
            var currentTransform = Transform3DHelper.CombineTransform(current.Transform, parentTransform);
            if (ReferenceEquals(current, model))
            {
                return currentTransform;
            }

            var mg = current as Model3DGroup;
            if (mg != null)
            {
                foreach (var m in mg.Children)
                {
                    var result = GetTransform(m, model, currentTransform);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Traverses the Model3D tree and invokes the specified action on each Model3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(this Model3D model, Action<T, Transform3D> action) where T : Model3D
        {
            Traverse(model, Transform3D.Identity, action);
        }

        /// <summary>
        /// Traverses the Model3D tree and invokes the specified action on each Model3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type.
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
        public static void Traverse<T>(this Model3D model, Transform3D transform, Action<T, Transform3D> action)
            where T : Model3D
        {
            var mg = model as Model3DGroup;
            if (mg != null)
            {
                var childTransform = Transform3DHelper.CombineTransform(model.Transform, transform);
                foreach (var m in mg.Children)
                {
                    Traverse(m, childTransform, action);
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
        /// Traverses the Model3D tree and invokes the specified action on each Model3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(this Model3D model, Visual3D visual, Transform3D transform, Action<T, Visual3D, Transform3D> action)
            where T : Model3D
        {
            var mg = model as Model3DGroup;
            if (mg != null)
            {
                var childTransform = Transform3DHelper.CombineTransform(model.Transform, transform);
                foreach (var m in mg.Children)
                {
                    Traverse(m, visual, childTransform, action);
                }
            }

            var gm = model as T;
            if (gm != null)
            {
                var childTransform = Transform3DHelper.CombineTransform(model.Transform, transform);
                action(gm, visual, childTransform);
            }
        }
    }
}