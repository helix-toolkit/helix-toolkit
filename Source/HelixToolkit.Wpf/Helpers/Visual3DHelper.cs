using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides extension methods for <see cref="Visual3D"/> objects.
/// </summary>
public static class Visual3DHelper
{
    /// <summary>
    /// The Visual3DModel property.
    /// </summary>
    private static readonly PropertyInfo? Visual3DModelPropertyInfo = typeof(Visual3D).GetProperty("Visual3DModel", BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// Finds the first child of the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type.
    /// </typeparam>
    /// <param name="parent">
    /// The parent.
    /// </param>
    /// <returns>
    /// The first child of the specified type.
    /// </returns>
    public static T? Find<T>(DependencyObject parent) where T : DependencyObject
    {
        // todo: use queue/stack, not recursion
        foreach (DependencyObject d in LogicalTreeHelper.GetChildren(parent))
        {
            var a = Find<T>(d);
            if (a != null)
            {
                return a;
            }
        }

        if (parent is ModelVisual3D model)
        {
            if (model.Content is Model3DGroup modelGroup)
            {
                return modelGroup.Children.OfType<T>().FirstOrDefault();
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
    /// A <see cref="Rect3D"/>.
    /// </returns>
    public static Rect3D FindBounds(this Visual3DCollection children)
    {
        var bounds = Rect3D.Empty;
        foreach (var visual in children)
        {
            if (visual is IBoundsIgnoredVisual3D)
            {
                continue;
            }

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
    /// The transform of the visual.
    /// </param>
    /// <returns>
    /// A <see cref="Rect3D"/>.
    /// </returns>
    public static Rect3D FindBounds(this Visual3D visual, Transform3D transform)
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
            if (child is IBoundsIgnoredVisual3D)
            {
                continue;
            }

            var b = FindBounds(child, childTransform);
            bounds.Union(b);
        }

        return bounds;
    }

    /// <summary>
    /// Gets the total transform for the specified visual.
    /// </summary>
    /// <param name="visual">
    /// The visual.
    /// </param>
    /// <returns>
    /// A <see cref="Matrix3D"/>.
    /// </returns>
    public static Matrix3D GetTransform(this Visual3D visual)
    {
        var totalTransform = Matrix3D.Identity;

        DependencyObject obj = visual;
        while (obj != null)
        {
            if (obj is Viewport3DVisual viewport3DVisual)
            {
                return totalTransform;
            }
            else if (obj is Visual3D mv && mv.Transform != null)
            {
                totalTransform.Append(mv.Transform.Value);
            }
            obj = VisualTreeHelper.GetParent(obj);
        }

        throw new InvalidOperationException("The visual is not added to a Viewport3D.");
    }

    /// <summary>
    /// Gets the parent <see cref="Viewport3D"/> from the specified visual.
    /// </summary>
    /// <param name="visual">
    /// The visual.
    /// </param>
    /// <returns>
    /// The Viewport3D
    /// </returns>
    public static Viewport3D? GetViewport3D(this Visual3D visual)
    {
        DependencyObject obj = visual;
        while (obj != null)
        {
            if (obj is Viewport3DVisual vis)
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
    public static Matrix3D GetViewportTransform(this Visual3D visual)
    {
        var totalTransform = Matrix3D.Identity;

        DependencyObject obj = visual;
        while (obj != null)
        {
            if (obj is Viewport3DVisual viewport3DVisual)
            {
                var viewportTotalTransform = viewport3DVisual.GetTotalTransform();
                totalTransform.Append(viewportTotalTransform);
                return totalTransform;
            }

            if (obj is ModelVisual3D mv)
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
    public static bool IsAttachedToViewport3D(this Visual3D visual)
    {
        DependencyObject obj = visual;
        while (obj != null)
        {
            if (obj is Viewport3DVisual vis)
            {
                return true;
            }

            obj = VisualTreeHelper.GetParent(obj);
        }

        return false;
    }

    /// <summary>
    /// Traverses the Visual3D/Model3D tree and invokes the specified action on each Model3D of the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type filter.
    /// </typeparam>
    /// <param name="visuals">
    /// The visuals.
    /// </param>
    /// <param name="action">
    /// The action.
    /// </param>
    public static void Traverse<T>(this Visual3DCollection visuals, Action<T, Transform3D> action) where T : Model3D
    {
        foreach (var child in visuals)
        {
            Traverse(child, action);
        }
    }

    /// <summary>
    /// Traverses the Visual3D/Model3D tree and invokes the specified action on each Model3D of the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type filter.
    /// </typeparam>
    /// <param name="visuals">
    /// The visuals.
    /// </param>
    /// <param name="action">
    /// The action.
    /// </param>
    public static void Traverse<T>(this Visual3DCollection visuals, Action<T, Visual3D, Transform3D> action) where T : Model3D
    {
        foreach (var child in visuals)
        {
            Traverse(child, action);
        }
    }

    /// <summary>
    /// Traverses the Visual3D/Model3D tree and invokes the specified action on each Model3D of the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type filter.
    /// </typeparam>
    /// <param name="visual">
    /// The visual.
    /// </param>
    /// <param name="action">
    /// The action.
    /// </param>
    public static void Traverse<T>(this Visual3D visual, Action<T, Transform3D> action) where T : Model3D
    {
        Traverse(visual, Transform3D.Identity, action);
    }

    /// <summary>
    /// Traverses the Visual3D/Model3D tree and invokes the specified action on each Model3D of the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type filter.
    /// </typeparam>
    /// <param name="visual">
    /// The visual.
    /// </param>
    /// <param name="action">
    /// The action.
    /// </param>
    public static void Traverse<T>(this Visual3D visual, Action<T, Visual3D, Transform3D> action) where T : Model3D
    {
        Traverse(visual, Transform3D.Identity, action);
    }

    /// <summary>
    /// Gets the transform from the specified Visual3D to the specified Model3D.
    /// </summary>
    /// <param name="visual">The source visual.</param>
    /// <param name="model">The target model.</param>
    /// <returns>The transform.</returns>
    public static GeneralTransform3D? GetTransformTo(this Visual3D visual, Model3D model)
    {
        var mc = GetModel(visual);
        if (mc != null)
        {
            return mc.GetTransform(model, Transform3D.Identity);
        }

        return null;
    }

    /// <summary>
    /// Gets the viewport for the specified visual.
    /// </summary>
    /// <param name="visual">The visual.</param>
    /// <returns>The parent <see cref="Viewport3D"/>.</returns>
    public static Viewport3D? GetViewport(this Visual3D visual)
    {
        DependencyObject parent = visual;
        while (parent != null)
        {
            if (parent is Viewport3DVisual vp)
            {
                return (Viewport3D)vp.Parent;
            }

            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }

    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <param name="parent">
    /// The parent visual.
    /// </param>
    /// <returns>
    /// A sequence of <see cref="Visual3D"/> objects.
    /// </returns>
    private static IEnumerable<Visual3D> GetChildren(this Visual3D parent)
    {
        int n = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < n; i++)
        {
            if (VisualTreeHelper.GetChild(parent, i) is not Visual3D child)
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
    /// A <see cref="Model3D"/>.
    /// </returns>
    private static Model3D? GetModel(this Visual3D visual)
    {
        Model3D? model;
        if (visual is ModelVisual3D mv)
        {
            model = mv.Content;
        }
        else
        {
            model = Visual3DModelPropertyInfo?.GetValue(visual, null) as Model3D;
        }

        return model;
    }

    /// <summary>
    /// Traverses the visual tree and invokes the specified action on each object of the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type filter.
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
            model?.Traverse(childTransform, action);

        foreach (var child in GetChildren(visual))
        {
            Traverse(child, childTransform, action);
        }
    }
    private static void Traverse<T>(Visual3D visual, Transform3D transform, Action<T, Visual3D, Transform3D> action)
        where T : Model3D
    {
        var childTransform = Transform3DHelper.CombineTransform(visual.Transform, transform);
        var model = GetModel(visual);
        model?.Traverse(visual, childTransform, action);

        foreach (var child in GetChildren(visual))
        {
            Traverse(child, childTransform, action);
        }
    }
}
