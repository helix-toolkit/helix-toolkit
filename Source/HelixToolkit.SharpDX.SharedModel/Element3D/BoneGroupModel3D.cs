using SharpDX;

#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
    /// <summary>
    /// Used to share bone matrices for multiple <see cref="BoneSkinMeshGeometryModel3D"/>
    /// </summary>
    public sealed class BoneGroupModel3D : GroupModel3D
    {
        /// <summary>
        /// Gets or sets the bone matrices.
        /// </summary>
        /// <value>
        /// The bone matrices.
        /// </value>
        public Matrix[] BoneMatrices
        {
            get
            {
                return (Matrix[])GetValue(BoneMatricesProperty);
            }
            set
            {
                SetValue(BoneMatricesProperty, value);
            }
        }

        /// <summary>
        /// The bone matrices property
        /// </summary>
        public static readonly DependencyProperty BoneMatricesProperty =
            DependencyProperty.Register("BoneMatrices", typeof(Matrix[]), typeof(BoneGroupModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3D).SceneNode as BoneGroupNode).BoneMatrices = (Matrix[])e.NewValue;
            }));

        protected override SceneNode OnCreateSceneNode()
        {
            return new BoneGroupNode();
        }
    }
}
