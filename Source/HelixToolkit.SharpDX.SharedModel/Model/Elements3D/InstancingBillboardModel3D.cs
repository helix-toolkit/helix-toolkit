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

/// <summary>
/// 
/// </summary>
public class InstancingBillboardModel3D : BillboardTextModel3D
{
    #region Dependency Properties
    /// <summary>
    /// List of instance parameter. 
    /// </summary>
    public static readonly DependencyProperty InstanceAdvArrayProperty =
        DependencyProperty.Register("InstanceParamArray", typeof(IList<BillboardInstanceParameter>), typeof(InstancingBillboardModel3D),
            new PropertyMetadata(null, (d, e) =>
            {
                if (d is Element3DCore { SceneNode: InstancingBillboardNode node })
                {
                    node.InstanceParamArray = e.NewValue as IList<BillboardInstanceParameter>;
                }
            }));

    /// <summary>
    /// List of instance parameters. 
    /// </summary>
    public IList<BillboardInstanceParameter> InstanceParamArray
    {
        get
        {
            return (IList<BillboardInstanceParameter>)this.GetValue(InstanceAdvArrayProperty);
        }
        set
        {
            this.SetValue(InstanceAdvArrayProperty, value);
        }
    }
    #endregion

    /// <summary>
    /// Called when [create scene node].
    /// </summary>
    /// <returns></returns>
    protected override SceneNode OnCreateSceneNode()
    {
        return new InstancingBillboardNode() { Material = material };
    }
}
