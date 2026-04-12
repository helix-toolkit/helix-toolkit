using HelixToolkit.SharpDX.Model.Scene;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
#endif

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
            return (bool)GetValue(EnableSortingProperty)!;
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
        HelixProperty.Register<SortingGroupModel3D, bool>("EnableSorting",
            true, (d, e) =>
        {
            if (d is Element3D { SceneNode: SortingGroupNode node })
            {
                node.EnableSorting = (bool)e.NewValue!;
            }
        });

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
            return (int)GetValue(SortingIntervalProperty)!;
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
        HelixProperty.Register<SortingGroupModel3D, int>("SortingInterval",
            500, (d, e) =>
        {
            if (d is Element3D { SceneNode: SortingGroupNode node })
            {
                node.SortingInterval = (int)e.NewValue!;
            }
        });

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
            return (bool)GetValue(SortTransparentOnlyProperty)!;
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
        HelixProperty.Register<SortingGroupModel3D, bool>("SortTransparentOnly",
            true, (d, e) =>
        {
            if (d is Element3D { SceneNode: SortingGroupNode node })
            {
                node.SortTransparentOnly = (bool)e.NewValue!;
            }
        });

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
            return (SortingMethod)GetValue(SortingMethodProperty)!;
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
        HelixProperty.Register<SortingGroupModel3D, SortingMethod>("SortingMethod",
            SortingMethod.BoundingBoxCorners, (d, e) =>
        {
            if (d is Element3D { SceneNode: SortingGroupNode node })
            {
                node.SortingMethod = (SortingMethod)e.NewValue!;
            }
        });

    protected override SceneNode OnCreateSceneNode()
    {
        return new SortingGroupNode();
    }
}
