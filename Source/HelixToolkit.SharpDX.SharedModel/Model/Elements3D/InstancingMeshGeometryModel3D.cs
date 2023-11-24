using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;

#if WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#else
using HelixToolkit.Wpf.SharpDX.Model;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public class InstancingMeshGeometryModel3D : MeshGeometryModel3D
{
    #region DependencyProperties
    /// <summary>
    /// If bind to identifiers, hit test returns identifier as Tag in HitTestResult.
    /// </summary>
    public static readonly DependencyProperty InstanceIdentifiersProperty = DependencyProperty.Register("InstanceIdentifiers", typeof(IList<System.Guid>),
        typeof(InstancingMeshGeometryModel3D), new PropertyMetadata(null, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: InstancingMeshNode node })
            {
                node.InstanceIdentifiers = e.NewValue as IList<System.Guid>;
            }
        }));

    /// <summary>
    /// Add octree manager to use octree hit test.
    /// </summary>
    public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
        typeof(IOctreeManagerWrapper), typeof(InstancingMeshGeometryModel3D), new PropertyMetadata(null, (s, e) =>
        {
            if (s is not InstancingMeshGeometryModel3D d)
            {
                return;
            }

#if WINUI
            d.AttachChild(null);
            if(e.NewValue is Element3D elem)
            {
                d.AttachChild(elem);
            }
#else
            if (e.OldValue != null)
            {
                d.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                d.AddLogicalChild(e.NewValue);
            }
#endif
            if (d.SceneNode is InstancingMeshNode node)
            {
                node.OctreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper)?.Manager;
            }
        }));

    /// <summary>
    /// List of instance parameter. 
    /// </summary>
    public static readonly DependencyProperty InstanceAdvArrayProperty =
        DependencyProperty.Register("InstanceParamArray", typeof(IList<InstanceParameter>), typeof(InstancingMeshGeometryModel3D),
            new PropertyMetadata(null, (d, e) =>
            {
                if (d is Element3DCore { SceneNode: InstancingMeshNode node })
                {
                    node.InstanceParamArray = e.NewValue as IList<InstanceParameter>;
                }
            }));

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
    public IList<InstanceParameter> InstanceParamArray
    {
        get
        {
            return (IList<InstanceParameter>)this.GetValue(InstanceAdvArrayProperty);
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

#if WINUI
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (OctreeManager is Element3D elem)
        {
            AttachChild(elem);
        }
    }
#endif
}
