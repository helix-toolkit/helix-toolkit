
using SharpDX;
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
    public class BoneSkinMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static DependencyProperty BoneMatricesProperty = DependencyProperty.Register("BoneMatrices", typeof(Matrix[]), typeof(BoneSkinMeshGeometryModel3D),
            new PropertyMetadata(BoneMatricesStruct.DefaultBones,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as BoneSkinMeshNode).BoneMatrices = (Matrix[])e.NewValue;
                }));

        public Matrix[] BoneMatrices
        {
            set
            {
                SetValue(BoneMatricesProperty, value);
            }
            get
            {
                return (Matrix[])GetValue(BoneMatricesProperty);
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new BoneSkinMeshNode();
        }
    }
}
