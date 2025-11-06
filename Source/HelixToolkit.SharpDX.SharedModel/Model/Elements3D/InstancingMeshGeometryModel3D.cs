using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;


#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Model;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Model;
using Avalonia.LogicalTree;
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

public class InstancingMeshGeometryModel3D : MeshGeometryModel3D
{
    #region DependencyProperties
    /// <summary>
    /// If bind to identifiers, hit test returns identifier as Tag in HitTestResult.
    /// </summary>
    public static readonly DependencyProperty InstanceIdentifiersProperty =
        HelixProperty.Register<InstancingMeshGeometryModel3D, IList<System.Guid>?>("InstanceIdentifiers",
            null, (d, e) =>
            {
                if (d is Element3DCore { SceneNode: InstancingMeshNode node })
                {
                    node.InstanceIdentifiers = e.NewValue as IList<System.Guid>;
                }
            });

    /// <summary>
    /// Add octree manager to use octree hit test.
    /// </summary>
    public static readonly DependencyProperty OctreeManagerProperty =
        HelixProperty.Register<InstancingMeshGeometryModel3D, IOctreeManagerWrapper?>("OctreeManager",
            null, (s, e) =>
            {
                if (s is not InstancingMeshGeometryModel3D d)
                {
                    return;
                }

#if false
#elif WINUI
                d.AttachChild(null);
                if(e.NewValue is Element3D elem)
                {
                    d.AttachChild(elem);
                }
#elif WPF
                if (e.OldValue != null)
                {
                    d.RemoveLogicalChild(e.OldValue);
                }

                if (e.NewValue != null)
                {
                    d.AddLogicalChild(e.NewValue);
                }
#elif AVALONIA
                if (e.OldValue != null)
                {
                    d.RemoveLogicalChild(e.OldValue as ILogical);
                }

                if (e.NewValue != null)
                {
                    d.AddLogicalChild(e.NewValue as ILogical);
                }
#else
#error Unknown framework
#endif
                if (d.SceneNode is InstancingMeshNode node)
                {
                    node.OctreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper)?.Manager;
                }
            });

    /// <summary>
    /// List of instance parameter. 
    /// </summary>
    public static readonly DependencyProperty InstanceAdvArrayProperty =
        HelixProperty.Register<InstancingMeshGeometryModel3D, IList<InstanceParameter>?>("InstanceParamArray",
            null, (d, e) =>
            {
                if (d is Element3DCore { SceneNode: InstancingMeshNode node })
                {
                    node.InstanceParamArray = e.NewValue as IList<InstanceParameter>;
                }
            });

    /// <summary>
    /// If bind to identifiers, hit test returns identifier as Tag in HitTestResult.
    /// </summary>        
    public IList<System.Guid>? InstanceIdentifiers
    {
        set
        {
            SetValue(InstanceIdentifiersProperty, value);
        }
        get
        {
            return (IList<System.Guid>?)GetValue(InstanceIdentifiersProperty);
        }
    }

    public IOctreeManagerWrapper? OctreeManager
    {
        set
        {
            SetValue(OctreeManagerProperty, value);
        }
        get
        {
            return (IOctreeManagerWrapper?)GetValue(OctreeManagerProperty);
        }
    }

    /// <summary>
    /// List of instance parameters. 
    /// </summary>
    public IList<InstanceParameter>? InstanceParamArray
    {
        get
        {
            return (IList<InstanceParameter>?)this.GetValue(InstanceAdvArrayProperty);
        }
        set
        {
            this.SetValue(InstanceAdvArrayProperty, value);
        }
    }

    #endregion

    protected override SceneNode OnCreateSceneNode()
    {
        return new InstancingMeshNode();
    }

#if false
#elif WINUI
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (OctreeManager is Element3D elem)
        {
            AttachChild(elem);
        }
    }
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
}
