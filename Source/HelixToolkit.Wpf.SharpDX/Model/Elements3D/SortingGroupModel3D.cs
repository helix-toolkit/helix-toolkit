using HelixToolkit.SharpDX.Model.Scene;

namespace HelixToolkit.Wpf.SharpDX;

public class SortingGroupModel3D : GroupModel3D
{
    /// <summary>
    /// Gets or sets a value indicating whether [enable sorting].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable sorting]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableSorting
    {
        get
        {
            return (bool)GetValue(EnableSortingProperty);
        }
        set
        {
            SetValue(EnableSortingProperty, value);
        }
    }

    /// <summary>
    /// The enable sorting property
    /// </summary>
    public static readonly DependencyProperty EnableSortingProperty =
        DependencyProperty.Register("EnableSorting", typeof(bool), typeof(SortingGroupModel3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3D { SceneNode: SortingGroupNode node })
            {
                node.EnableSorting = (bool)e.NewValue;
            }
        }));

    /// <summary>
    /// Gets or sets the sorting interval by milliseconds. Default is 500ms
    /// </summary>
    /// <value>
    /// The sorting interval.
    /// </value>
    public int SortingInterval
    {
        get
        {
            return (int)GetValue(SortingIntervalProperty);
        }
        set
        {
            SetValue(SortingIntervalProperty, value);
        }
    }

    /// <summary>
    /// The sorting interval property
    /// </summary>
    public static readonly DependencyProperty SortingIntervalProperty =
        DependencyProperty.Register("SortingInterval", typeof(int), typeof(SortingGroupModel3D), new PropertyMetadata(500, (d, e) =>
        {
            if (d is Element3D { SceneNode: SortingGroupNode node })
            {
                node.SortingInterval = (int)e.NewValue;
            }
        }));

    /// <summary>
    /// Gets or sets a value indicating whether [sort transparent only].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [sort transparent only]; otherwise, <c>false</c>.
    /// </value>
    public bool SortTransparentOnly
    {
        get
        {
            return (bool)GetValue(SortTransparentOnlyProperty);
        }
        set
        {
            SetValue(SortTransparentOnlyProperty, value);
        }
    }

    /// <summary>
    /// The sort transparent only property
    /// </summary>
    public static readonly DependencyProperty SortTransparentOnlyProperty =
        DependencyProperty.Register("SortTransparentOnly", typeof(bool), typeof(SortingGroupModel3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3D { SceneNode: SortingGroupNode node })
            {
                node.SortTransparentOnly = (bool)e.NewValue;
            }
        }));

    /// <summary>
    /// Gets or sets the sorting method.
    /// </summary>
    /// <value>
    /// The sorting method.
    /// </value>
    public SortingMethod SortingMethod
    {
        get
        {
            return (SortingMethod)GetValue(SortingMethodProperty);
        }
        set
        {
            SetValue(SortingMethodProperty, value);
        }
    }

    /// <summary>
    /// The sorting method property
    /// </summary>
    public static readonly DependencyProperty SortingMethodProperty =
        DependencyProperty.Register("SortingMethod", typeof(SortingMethod), typeof(SortingGroupModel3D), new PropertyMetadata(SortingMethod.BoundingBoxCorners, (d, e) =>
        {
            if (d is Element3D { SceneNode: SortingGroupNode node })
            {
                node.SortingMethod = (SortingMethod)e.NewValue;
            }
        }));

    protected override SceneNode OnCreateSceneNode()
    {
        return new SortingGroupNode();
    }
}
