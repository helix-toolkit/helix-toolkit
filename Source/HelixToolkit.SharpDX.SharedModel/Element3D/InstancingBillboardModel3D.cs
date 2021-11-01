using System.Collections.Generic;

#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF && !WINUI
    using Model.Scene;
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
                    ((d as Element3DCore).SceneNode as InstancingBillboardNode).InstanceParamArray = e.NewValue as IList<BillboardInstanceParameter>;
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
}
