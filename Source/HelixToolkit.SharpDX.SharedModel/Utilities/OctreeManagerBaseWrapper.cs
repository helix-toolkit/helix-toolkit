using HelixToolkit.SharpDX;

#if false
#elif WINUI
using Windows.UI.Core;
#elif WPF
using System.Windows.Threading;
#elif AVALONIA
using Avalonia.Threading;
#else
#error Unknown framework
#endif

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

/// <summary>
/// 
/// </summary>
public abstract class OctreeManagerBaseWrapper : FrameworkContentElement, IOctreeManagerWrapper
{
    /// <summary>
    /// The octree property
    /// </summary>
    public static readonly DependencyProperty OctreeProperty
        = HelixProperty.Register<OctreeManagerBaseWrapper, IOctreeBasic?>("Octree", null);

    /// <summary>
    /// The enable octree output property
    /// </summary>
    public static readonly DependencyProperty EnableOctreeOutputProperty
        = HelixProperty.Register<OctreeManagerBaseWrapper, bool>("EnableOctreeOutput",
            false,
                (s, e) =>
                {
                    if (s is OctreeManagerBaseWrapper wrapper)
                    {
                        wrapper.enableOctreeOutput = (bool)e.NewValue!;
                    }
                });

    /// <summary>
    /// The minimum size property
    /// </summary>
    public static readonly DependencyProperty MinSizeProperty
        = HelixProperty.Register<OctreeManagerBaseWrapper, float>("MinSize",
            1f, (s, e) =>
            {
                if (s is OctreeManagerBaseWrapper wrapper)
                {
                    wrapper.Manager.Parameter.MinimumOctantSize = (float)e.NewValue!;
                }
            });

    /// <summary>
    /// The automatic delete if empty property
    /// </summary>
    public static readonly DependencyProperty AutoDeleteIfEmptyProperty
        = HelixProperty.Register<OctreeManagerBaseWrapper, bool>("AutoDeleteIfEmpty",
            true, (s, e) =>
            {
                if (s is OctreeManagerBaseWrapper wrapper)
                {
                    wrapper.Manager.Parameter.AutoDeleteIfEmpty = (bool)e.NewValue!;
                }
            });

    /// <summary>
    /// The cubify property property
    /// </summary>
    public static readonly DependencyProperty CubifyPropertyProperty
        = HelixProperty.Register<OctreeManagerBaseWrapper, bool>("Cubify",
            false, (s, e) =>
            {
                if (s is OctreeManagerBaseWrapper wrapper)
                {
                    wrapper.Manager.Parameter.Cubify = (bool)e.NewValue!;
                }
            });

    /// <summary>
    /// The record hit path bounding boxes property
    /// </summary>
    public static readonly DependencyProperty RecordHitPathBoundingBoxesProperty
        = HelixProperty.Register<OctreeManagerBaseWrapper, bool>("RecordHitPathBoundingBoxes",
            false, (s, e) =>
            {
                if (s is OctreeManagerBaseWrapper wrapper)
                {
                    wrapper.Manager.Parameter.RecordHitPathBoundingBoxes = (bool)e.NewValue!;
                }
            });

    /// <summary>
    /// The minimum object size to split property
    /// </summary>
    public static readonly DependencyProperty MinObjectSizeToSplitProperty
        = HelixProperty.Register<OctreeManagerBaseWrapper, int>("MinObjectSizeToSplit",
            0, (s, e) =>
            {
                if (s is OctreeManagerBaseWrapper wrapper)
                {
                    wrapper.Manager.Parameter.MinObjectSizeToSplit = (int)e.NewValue!;
                }
            });

    /// <summary>
    /// Gets or sets the octree.
    /// </summary>
    /// <value>
    /// The octree.
    /// </value>
    public IOctreeBasic? Octree
    {
        set
        {
            SetValue(OctreeProperty, value);
        }

        get
        {
            return (IOctreeBasic?)GetValue(OctreeProperty);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [enable octree output].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable octree output]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableOctreeOutput
    {
        set
        {
            SetValue(EnableOctreeOutputProperty, value);
        }
        get
        {
            return (bool)GetValue(EnableOctreeOutputProperty)!;
        }
    }

    /// <summary>
    /// Minimum octant size
    /// </summary>
    public float MinSize
    {
        set
        {
            SetValue(MinSizeProperty, value);
        }
        get
        {
            return (float)GetValue(MinSizeProperty)!;
        }
    }
    /// <summary>
    /// Delete octant node if its empty
    /// </summary>
    public bool AutoDeleteIfEmpty
    {
        set
        {
            SetValue(AutoDeleteIfEmptyProperty, value);
        }
        get
        {
            return (bool)GetValue(AutoDeleteIfEmptyProperty)!;
        }
    }
    /// <summary>
    /// Create cube octree
    /// </summary>
    public bool Cubify
    {
        set
        {
            SetValue(CubifyPropertyProperty, value);
        }
        get
        {
            return (bool)GetValue(CubifyPropertyProperty)!;
        }
    }
    /// <summary>
    /// Record the hit path bounding box for debugging
    /// </summary>
    public bool RecordHitPathBoundingBoxes
    {
        set
        {
            SetValue(RecordHitPathBoundingBoxesProperty, value);
        }
        get
        {
            return (bool)GetValue(RecordHitPathBoundingBoxesProperty)!;
        }
    }
    /// <summary>
    /// Minimum object in each octant to start splitting into smaller octant during build
    /// </summary>
    public int MinObjectSizeToSplit
    {
        set
        {
            SetValue(MinObjectSizeToSplitProperty, value);
        }
        get
        {
            return (int)GetValue(MinObjectSizeToSplitProperty)!;
        }
    }
#if false
#elif WINUI
    private IAsyncAction? octreeOpt;
#elif WPF
    private DispatcherOperation? octreeOpt;
#elif AVALONIA
    private DispatcherOperation? octreeOpt;
#else
#error Unknown framework
#endif
    private bool enableOctreeOutput = false;
    private IOctreeManager? manager;

    /// <summary>
    /// Gets the manager.
    /// </summary>
    /// <value>
    /// The manager.
    /// </value>
    public IOctreeManager Manager
    {
        get
        {
            if (manager == null)
            {
                manager = OnCreateManager();
                manager.OnOctreeCreated += (s, e) =>
                {
#if false
#elif WINUI
                    if (octreeOpt != null && octreeOpt.Status != AsyncStatus.Completed)
                    {
                        octreeOpt?.Cancel();
                    }
                    if (enableOctreeOutput)
                    {
                        octreeOpt = Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        {
                            this.Octree = null;
                            this.Octree = e.Octree;
                        });
                    }
#elif WPF
                    if (octreeOpt != null && octreeOpt.Status == DispatcherOperationStatus.Pending)
                    {
                        octreeOpt.Abort();
                    }
                    if (enableOctreeOutput)
                    {
                        octreeOpt = Dispatcher.BeginInvoke(DispatcherPriority.Background,
                            new Action(() =>
                            {
                                this.Octree = null;
                                this.Octree = e.Octree;
                            }));
                    }
#elif AVALONIA
                    if (octreeOpt != null && octreeOpt.Status == DispatcherOperationStatus.Pending)
                    {
                        octreeOpt.Abort();
                    }
                    if (enableOctreeOutput)
                    {
                        octreeOpt = Dispatcher.UIThread.InvokeAsync(
                            new Action(() =>
                            {
                                this.Octree = null;
                                this.Octree = e.Octree;
                            }),
                            DispatcherPriority.Background);
                    }
#else
#error Unknown framework
#endif
                };
            }
            return manager;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OctreeManagerBaseWrapper"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Enabled
    {
        set
        {
            if (manager is not null)
            {
                manager.Enabled = value;
            }
        }
        get
        {
            if (manager is not null)
            {
                return manager.Enabled;

            }

            return false;
        }
    }
    /// <summary>
    /// Gets or sets the parameter.
    /// </summary>
    /// <value>
    /// The parameter.
    /// </value>
    public OctreeBuildParameter Parameter
    {
        set
        {
            Manager.Parameter = value;
        }
        get
        {
            return Manager.Parameter;
        }
    }
    /// <summary>
    /// Called when [create manager].
    /// </summary>
    /// <returns></returns>
    protected abstract IOctreeManager OnCreateManager();
}
