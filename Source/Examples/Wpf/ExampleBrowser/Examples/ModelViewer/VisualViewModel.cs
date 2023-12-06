using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Windows;
using HelixToolkit.Wpf;
using System.Windows.Media;

namespace ModelViewer;

public sealed class VisualViewModel
{
    public IEnumerable<VisualViewModel> Children
    {
        get
        {
            if (this.element is ModelVisual3D mv)
            {
                if (mv.Content != null)
                {
                    yield return new VisualViewModel(mv.Content);
                }

                foreach (var mc in mv.Children)
                {
                    yield return new VisualViewModel(mc);
                }
            }

            if (this.element is Model3DGroup mg)
            {
                foreach (var mc in mg.Children)
                {
                    yield return new VisualViewModel(mc);
                }
            }

            if (this.element is GeometryModel3D gm)
            {
                yield return new VisualViewModel(gm.Geometry);
            }

            //int n = VisualTreeHelper.GetChildrenCount(element);
            //for (int i = 0; i < n; i++)
            //    yield return new VisualViewModel(VisualTreeHelper.GetChild(element, i));
            foreach (DependencyObject c in LogicalTreeHelper.GetChildren(this.element))
            {
                yield return new VisualViewModel(c);
            }
        }
    }

    public string Name
    {
        get
        {
            return this.element.GetName();
        }
    }

    public string TypeName
    {
        get
        {
            return this.element.GetType().Name;
        }
    }

    public Brush? Brush
    {
        get
        {
            if (this.element.GetType() == typeof(ModelVisual3D))
                return Brushes.Orange;
            if (this.element.GetType() == typeof(GeometryModel3D))
                return Brushes.Green;
            if (this.element.GetType() == typeof(Model3DGroup))
                return Brushes.Blue;
            if (this.element.GetType() == typeof(Visual3D))
                return Brushes.Gray;
            if (this.element.GetType() == typeof(Model3D))
                return Brushes.Black;
            return null;
        }
    }

    public override string ToString()
    {
        return this.element.GetType().ToString();
    }

    private DependencyObject element;

    public VisualViewModel(DependencyObject e)
    {
        this.element = e;
    }
}
